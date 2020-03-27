/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.MatchController
/// class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Script which controls ColorShapeLinks matches.
    /// </summary>
    /// <remarks>
    /// Based on the MVC design pattern, composed in this case by the following
    /// classes:
    /// * *Model* - <see cref="Board"/>.
    /// * *View* - <see cref="MatchView"/>.
    /// * *Controller* - This class.
    /// </remarks>
    public class MatchController : MonoBehaviour
    {
        // Reference to the MVC view for the match
        private MatchView view;

        // Is the match over?
        private bool matchOver = false;

        // Show next turn message to a human player?
        private bool showHumanTurnMessage = true;

        // Reference to the match data
        private IMatchDataProvider matchData;

        // Reference to the match configuration
        private IMatchConfig matchConfig;

        // Reference to the game board
        private Board board;

        // Reference to a solution
        private Pos[] solution;

        // Thinker currently thinking
        private IThinker thinker;

        // Task to execute AI search in a separate thread
        private Task<FutureMove> aiTask;

        // The AI time limit
        private TimeSpan aiTimeLimit;

        // The cancellation token to pass the AI
        private CancellationTokenSource ts;

        // Stopwatch for measuring native C# system time
        private Stopwatch stopwatch;

        // Stopwatch to measure time after a cancellation request was
        // submitted to a thinker
        private Stopwatch cancellationStopwatch;

        // Task start time and duration (Unity time)
        private float taskStartGameTime, lastTaskDuration = float.NaN;

        // Name and color of current player
        private string CurrPlrNameColor => PlrNameColor(board.Turn);

        /// <summary>Name and color of given player.</summary>
        /// <param name="color">Color of player to get name/color of.</param>
        /// <returns>A string with the name and color of player.</returns>
        public string PlrNameColor(PColor color) =>
            color.FormatName(matchData.GetThinker(color).ToString());

        /// <summary>Name and color of winner.</summary>
        /// <returns>A string with the name and color of the winner.</returns>
        public string WinnerString => PlrNameColor(Result.ToPColor());

        /// <summary>Result of the match being controlled.</summary>
        /// <value>One of the possible values in <see cref="Winner"/>.</value>
        public Winner Result { get; private set; }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Set result to none when the match starts
            Result = Winner.None;

            // Instantiate the unity event for notifying end of match
            MatchOver = new UnityEvent();

            // Get reference to the match data provider
            matchData = GetComponentInParent<IMatchDataProvider>();

            // Get reference to the match configuration
            matchConfig = GetComponentInParent<IMatchConfig>();

            // Get reference to the game board
            board = matchData.Board;

            // Instantiate the array where to place the solution
            solution = new Pos[board.piecesInSequence];

            // Get reference to the match view object
            view = GameObject.Find("UI")?.GetComponent<MatchView>();

            // Get the AI time limit as a native C# TimeSpan
            aiTimeLimit = new TimeSpan(
                (long)(matchConfig.TimeLimitMillis
                    * TimeSpan.TicksPerMillisecond));

            // Instantiate the stopwatch
            stopwatch = new Stopwatch();

            // Set cancellation stopwatch to null
            cancellationStopwatch = null;
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            // Listen for moves initiated by the UI, which can only be human
            // moves
            view.MoveSelected += HumanMove;
        }

        // This function is called when the behaviour becomes disabled
        private void OnDisable()
        {
            // Stop listening for moves initiated by the UI
            view.MoveSelected -= HumanMove;
        }

        // Update is called once per frame
        private void Update()
        {
            // Don't run update if the game's over
            if (matchOver) return;

            // Is the current player human? If so, let's see if we're supposed
            // to show him a message or if we've done so already
            if (matchData.CurrentThinker is HumanThinker)
            {
                if (showHumanTurnMessage)
                {
                    view.SubmitMessage(
                        $"Attention {CurrPlrNameColor}, it's your turn");
                    showHumanTurnMessage = false;
                };
            }

            // If the current player is an AI, let's see if we have to start
            // the AI thinking task, if the task is running, if it's completed,
            // etc.
            else
            {
                // If the AI task is null, we need to start an AI thinking task
                if (aiTask == null)
                {
                    // Submit a message informing the user the AI is thinking
                    view.SubmitMessage(
                        $"{CurrPlrNameColor} is thinking, please wait...");

                    // Keep note of task start time (both system time and game
                    // time)
                    stopwatch.Restart();
                    taskStartGameTime = Time.time;

                    // Create a new task cancellation token, so task can be
                    // interrupted
                    ts = new CancellationTokenSource();

                    // Get this AI's thinker
                    thinker = matchData.CurrentThinker;

                    // Start task in a separate thread using a copy of the
                    // board
                    aiTask = Task.Run(
                        () => thinker.Think(board.Copy(), ts.Token));
                }
                else // This else will run if the task is not null
                {
                    // Did the task throw an exception?
                    if (aiTask.IsFaulted)
                    {
                        // If so, notify user
                        view.SubmitMessage(string.Format(
                            "{0} exception: {1}",
                            CurrPlrNameColor,
                            aiTask.Exception.InnerException.Message));

                        // Send a cancellation token to the task in the hope it
                        // might actually terminate
                        ts.Cancel();

                        // Set task to null
                        aiTask = null;

                        // The AI player that throwed the exception will lose
                        // the game, sorry
                        OnMatchOver(board.Turn.Other().ToWinner());
                    }
                    // Is the AI thinking task completed in time?
                    else if (aiTask.IsCompleted
                        && cancellationStopwatch == null)
                    {
                        // Register task duration, if we haven't done so yet
                        if (float.IsNaN(lastTaskDuration))
                        {
                            lastTaskDuration = (float)(
                                    stopwatch.ElapsedTicks
                                    /
                                    (double)(TimeSpan.TicksPerSecond));
                        }

                        // Did we pass the minimum time between AI moves?
                        if (Time.time >
                            taskStartGameTime + matchConfig.MinMoveTimeSeconds)
                        {
                            // Get the move chosen by the thinker
                            FutureMove move = aiTask.Result;

                            // Was the thinker able to chose a move?
                            if (move.IsNoMove)
                            {
                                // Thinker was not able to chose a move,
                                // submit a message informing user of this
                                view.SubmitMessage(string.Format(
                                    "{0} unable to perform move",
                                    CurrPlrNameColor));

                                // The AI player unable to move will lose
                                // the game, sorry
                                OnMatchOver(board.Turn.Other().ToWinner());
                            }
                            else
                            {
                                try
                                {
                                    // If so, submit a message informing of
                                    // the move chosen and the system time it
                                    // took the AI to think
                                    view.SubmitMessage(string.Format(
                                        "{0} placed a {1} piece at column {2} after {3}",
                                        CurrPlrNameColor,
                                        aiTask.Result.shape.ToString().ToLower(),
                                        aiTask.Result.column,
                                        $"thinking for {lastTaskDuration:f4}s"));

                                    // Player was able to make a move decision,
                                    // let's perform the actual move
                                    MakeAMove(aiTask.Result);
                                }
                                catch (Exception e)
                                {
                                    // The act of making an actual move caused
                                    // an exception, which means the thinker
                                    // chose an invalid move, as such,
                                    // notify user of this
                                    view.SubmitMessage(string.Format(
                                        "{0} exception: {1}",
                                        CurrPlrNameColor, e.Message));

                                    // The AI player that caused the exception
                                    // will lose the game, sorry
                                    OnMatchOver(board.Turn.Other().ToWinner());
                                }
                            }

                            // Set the task to null, so it can be
                            // started again
                            aiTask = null;

                            // Reset the last task duration
                            lastTaskDuration = float.NaN;
                        }
                    }
                    // Is the task overdue?
                    else if (stopwatch.Elapsed > aiTimeLimit)
                    {
                        // If so, check the status of the thinking cancellation
                        // process
                        if (cancellationStopwatch is null)
                        {
                            // The thinking cancellation process has not yet
                            // been started, so let's start it

                            // Inform user that the time limit for
                            // the current thinker has been exceeded
                            view.SubmitMessage(
                                $"Time limit exceeded for {CurrPlrNameColor}!");

                            // Notify task it should cancel its thinking
                            ts.Cancel();

                            // Start cancellation stopwatch
                            cancellationStopwatch = Stopwatch.StartNew();
                        }
                        else if (aiTask.IsCompleted)
                        {
                            // The thinking task is completed after the
                            // cancelation request, terminate match normally

                            // Set task to null
                            aiTask = null;

                            // Set cancellation stopwatch to null
                            cancellationStopwatch = null;

                            // The AI player that was overdue loses the game
                            OnMatchOver(board.Turn.Other().ToWinner());
                        }
                        else if (cancellationStopwatch.ElapsedMilliseconds >
                            UncooperativeThinkerException.HardThinkingLimitMs)
                        {
                            UnityEngine.Debug.LogWarning($"{cancellationStopwatch.ElapsedMilliseconds}ms have passed :(");

                            // If the hard thinking process time limit has been
                            // reached, throw an exception to terminate the app
                            throw new UncooperativeThinkerException(thinker);
                        }
                    }
                }
            }
        }

        // Callback method to perform a human move
        private void HumanMove(FutureMove move)
        {
            // Show message indicating what move the human chose
            view.SubmitMessage(string.Format(
                "{0} placed a {1} piece at column {2}",
                CurrPlrNameColor,
                move.shape.ToString().ToLower(),
                move.column));

            // Make the move
            MakeAMove(move);

            // In the next human turn, show the human a message for him to play
            showHumanTurnMessage = true;
        }

        // Make an actual move
        private void MakeAMove(FutureMove move)
        {
            // Who is making this move?
            PColor whoPlayed = board.Turn;

            // Perform move in game board, get column where move was performed
            int row = board.DoMove(move.shape, move.column);

            // If the column had space for the move...
            if (row >= 0)
            {
                // Get possible winner and solution
                Winner winner = board.CheckWinner(solution);

                // Update UI board with performed move, and possibly the
                // winner/solution
                view.UpdateBoard(
                    new Move(
                        row, move.column, new Piece(whoPlayed, move.shape)),
                    winner,
                    solution);

                // If the game is over...
                if (winner != Winner.None)
                {
                    // Invoke MatchOver event
                    OnMatchOver(winner);
                }
            }
            else // If we get here, column didn't have space for the move
            {
                throw new InvalidOperationException(
                    $"Column {move.column + 1} is full.");
            }
        }

        // Method for invoking the MatchOver event
        private void OnMatchOver(Winner result)
        {
            // Keep result
            Result = result;

            // Send a message to the UI with the match result
            view.SubmitMessage(string.Format("Game Over, {0}",
                result == Winner.Draw
                    ? "it's a draw"
                    : $"{PlrNameColor(result.ToPColor())} won"));

            // Set internal matchOver variable to true; this will stop further
            // Update()s
            matchOver = true;

            // Notify MatchOver listeners
            MatchOver?.Invoke();
        }

        /// <summary>
        /// Unity event invoked when the match is over.
        /// </summary>
        /// <value>A parameterless Unity event.</value>
        public UnityEvent MatchOver { get; private set; }
    }
}
