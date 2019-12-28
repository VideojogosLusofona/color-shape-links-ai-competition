/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class MatchController : MonoBehaviour
{
    private MatchView view;

    private bool gameOver = false;
    private bool showHumanTurnMessage = true;
    private IMatchDataProvider matchData;
    private Board board;
    private Pos[] solution;

    private Task<FutureMove> aiTask;
    private TimeSpan aiTimeLimit;
    private CancellationTokenSource ts;
    private DateTime taskStartSysTime;
    private float taskStartGameTime, lastTaskDuration = float.NaN;
    private string CurrPlrNameColor => PlrNameColor(board.Turn);

    public string PlrNameColor(PColor color) =>
        $"{matchData.GetPlayer(color).PlayerName} ({color})";

    public string WinnerString => PlrNameColor(Result.ToPColor());

    public Winner Result { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        MatchOver = new UnityEvent();
        matchData = GetComponentInParent<IMatchDataProvider>();
        board = matchData.Board;
        solution = new Pos[board.piecesInSequence];
        view = GameObject.Find("UI")?.GetComponent<MatchView>();
        aiTimeLimit = new TimeSpan(
            (long)(matchData.AITimeLimit * TimeSpan.TicksPerSecond));
    }

    private void OnEnable()
    {
        view.MoveSelected += HumanMove;
    }

    private void OnDisable()
    {
        view.MoveSelected -= HumanMove;
    }

    // Update is called once per frame
    private void Update()
    {
        // Don't run update if the game's over
        if (gameOver) return;

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
                        taskStartGameTime + matchData.TimeBetweenAIMoves)
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

    private void MakeAMove(FutureMove move)
    {
        PColor whoPlayed = board.Turn;
        int row = board.DoMove(move.shape, move.column);
        if (row >= 0)
        {
            Winner winner = board.CheckWinner(solution);
            view.UpdateBoard(
                new Move(row, move.column, new Piece(whoPlayed, move.shape)),
                winner,
                solution);
            if (winner != Winner.None)
            {
                PColor winColor = winner.ToPColor();
                Result = winner;
                OnMatchOver();
            }
        }
        else
        {
            view.SubmitMessage(
                $"Column {move.column + 1} is full, try another one.");
        }
    }

    private void OnMatchOver()
    {
        view.SubmitMessage(string.Format("Game Over, {0}",
            Result == Winner.Draw
                ? "it's a draw"
                : $"{PlrNameColor(Result.ToPColor())} won"));
        gameOver = true;
        MatchOver?.Invoke();
    }

    public UnityEvent MatchOver { get; private set; }
}
