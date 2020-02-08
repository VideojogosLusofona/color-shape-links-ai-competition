/// @file
/// @brief This file contains the ::MatchController class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script which controls *ColorShapeLinks* matches.
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

    // Reference to the game board
    private Board board;

    // Reference to a solution
    private Pos[] solution;

    // Task to execute AI search in a separate thread
    private Task<FutureMove> aiTask;

    // The AI time limit
    private TimeSpan aiTimeLimit;

    // The cancellation token to pass the AI
    private CancellationTokenSource ts;

    // Task start time (native C# system time)
    private DateTime taskStartSysTime;

    // Task start time and duration (Unity time)
    private float taskStartGameTime, lastTaskDuration = float.NaN;

    // Name and color of current player
    private string CurrPlrNameColor => PlrNameColor(board.Turn);

    /// <summary>Name and color of given player.</summary>
    /// <param name="color">Color of player to get name/color of.</param>
    /// <returns>A string with the name and color of player.</returns>
    public string PlrNameColor(PColor color) =>
        $"{matchData.GetPlayer(color).PlayerName} ({color})";

    /// <summary>Name and color of winner.</summary>
    /// <returns>A string with the name and color of the winner.</returns>
    public string WinnerString => PlrNameColor(Result.ToPColor());

    /// <summary>Result of the match being controlled.</summary>
    /// <value>One of the possible values in <see cref="Winner"/>.</value>
    public Winner Result { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Instantiate the unity event for notifying end of match
        MatchOver = new UnityEvent();

        // Get reference to the match data provider
        matchData = GetComponentInParent<IMatchDataProvider>();

        // Get reference to the game board
        board = matchData.Board;

        // Instantiate the array where to place the solution
        solution = new Pos[board.piecesInSequence];

        // Get reference to the match view object
        view = GameObject.Find("UI")?.GetComponent<MatchView>();

        // Get the AI time limit as a native C# TimeSpan
        aiTimeLimit = new TimeSpan(
            (long)(matchData.AITimeLimit * TimeSpan.TicksPerSecond));

        // Setup the two players, if they are AIs
        (matchData.GetPlayer(PColor.White) as AIPlayer)?.Setup();
        (matchData.GetPlayer(PColor.Red) as AIPlayer)?.Setup();
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Listen for moves initiated by the UI, which can only be human moves
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

        // Is the current player human? If so, let's see if we're supposed to
        // show him a message or if we've done so already
        if (matchData.CurrentPlayer.IsHuman)
        {
            if (showHumanTurnMessage)
            {
                view.SubmitMessage(
                    $"Attention {CurrPlrNameColor}, it's your turn");
                showHumanTurnMessage = false;
            };
        }

        // If the current player is an AI, let's see if we have to start
        // the AI thinking task, if the task is running, if it's completed, etc.
        else
        {
            // If the AI task is null, we need to start an AI thinking task
            if (aiTask == null)
            {
                // Submit a message informing the user the AI is thinking
                view.SubmitMessage(
                    $"{CurrPlrNameColor} is thinking, please wait...");

                // Keep note of task start time (both system time and game time)
                taskStartSysTime = DateTime.Now;
                taskStartGameTime = Time.time;

                // Create a new task cancellation token, so task can be
                // interrupted
                ts = new CancellationTokenSource();

                // Get this AI's thinker
                IThinker thinker = matchData.CurrentPlayer.Thinker;

                // Start task in a separate thread using a copy of the board
                aiTask = Task.Run(() => thinker.Think(board.Copy(), ts.Token));
            }
            else // This else will run if the task is not null
            {
                // Is the AI thinking task completed?
                if (aiTask.IsCompleted)
                {
                    // Register task duration, if we haven't done so yet
                    if (float.IsNaN(lastTaskDuration))
                    {
                        lastTaskDuration =
                            (float)((DateTime.Now - taskStartSysTime).Ticks /
                            (double)(TimeSpan.TicksPerSecond));
                    }

                    // Did we pass the minimum time between AI moves?
                    if (Time.time >
                        taskStartGameTime + matchData.MinAIGameMoveTime)
                    {
                        // If so, submit a message informing of the move
                        // chosen and the system time it took the AI to think
                        view.SubmitMessage(string.Format(
                            "{0} placed a {1} piece at column {2} after {3}",
                            CurrPlrNameColor,
                            aiTask.Result.shape.ToString().ToLower(),
                            aiTask.Result.column,
                            $"thinking for {lastTaskDuration:f4}s"));

                        // Perform the actual move
                        MakeAMove(aiTask.Result);

                        // Set the task to null, so it can be started again
                        aiTask = null;

                        // Reset the last task duration
                        lastTaskDuration = float.NaN;
                    }
                }
                // Did the task throw an exception?
                else if (aiTask.IsFaulted)
                {
                    // If so, notify user
                    view.SubmitMessage(
                        aiTask.Exception.InnerException.Message);

                    // Log exception as an error
                    Debug.LogError(aiTask.Exception.InnerException.Message);

                    // Send a cancellation token to the task in the hope it
                    // might actually terminate
                    ts.Cancel();

                    // Set task to null
                    aiTask = null;

                    // The AI player that throwed the exception will lose the
                    // game, sorry
                    this.Result = board.Turn == PColor.White
                        ? Winner.Red : Winner.White;
                    OnMatchOver();
                }
                // Is the task overdue?
                else if (DateTime.Now - taskStartSysTime > aiTimeLimit)
                {
                    // If so, notify user
                    view.SubmitMessage(
                        $"Time limit exceeded for {CurrPlrNameColor}!");

                    // Inform the task it should cancel its thinking
                    ts.Cancel();

                    // Set task to null
                    aiTask = null;

                    // The AI player that was overdue loses the game
                    this.Result = board.Turn == PColor.White
                        ? Winner.Red : Winner.White;
                    OnMatchOver();
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
                new Move(row, move.column, new Piece(whoPlayed, move.shape)),
                winner,
                solution);

            // If the game is over...
            if (winner != Winner.None)
            {
                // Keep result
                Result = winner;

                // Invoke MatchOver event
                OnMatchOver();
            }
        }
        else // If we get here, column didn't have space for the move
        {
            view.SubmitMessage(
                $"Column {move.column + 1} is full, try another one.");
        }
    }

    // Method for invoking the MatchOver event
    private void OnMatchOver()
    {
        // Send a message to the UI with the match result
        view.SubmitMessage(string.Format("Game Over, {0}",
            Result == Winner.Draw
                ? "it's a draw"
                : $"{PlrNameColor(Result.ToPColor())} won"));

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
