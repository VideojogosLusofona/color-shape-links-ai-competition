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

public class GameController : MonoBehaviour
{
    private GameView view;

    private bool gameOver = false;
    private ISessionDataProvider sessionData;
    private Board board;
    private Pos[] solution;

    private Task<FutureMove> aiTask;
    private TimeSpan aiTimeLimit;
    private CancellationTokenSource ts;
    private DateTime taskStartSysTime;
    private float taskStartGameTime, lastTaskDuration;
    private string CurrPlrNameColor => PlrNameColor(board.Turn);

    public string PlrNameColor(PColor color) =>
        $"{sessionData.GetPlayer(color).PlayerName} ({color})";

    public string WinnerString => PlrNameColor(Result.ToPColor());

    public Winner Result { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        lastTaskDuration = float.NaN;
        GameOver = new UnityEvent();
        sessionData = GetComponentInParent<ISessionDataProvider>();
        board = sessionData.Board;
        solution = new Pos[board.piecesInSequence];
        view = GameObject.Find("UI")?.GetComponent<GameView>();
        aiTimeLimit = new TimeSpan(
            (long)(sessionData.AITimeLimit * TimeSpan.TicksPerSecond));
    }

    private void OnEnable()
    {
        view.MoveSelected += MakeAMove;
    }

    private void OnDisable()
    {
        view.MoveSelected -= MakeAMove;
    }

    // Start is called before the first frame update
    private void Start()
    {
        view.SubmitMessage($"It's {CurrPlrNameColor} turn");
    }

    // Update is called once per frame
    private void Update()
    {
        // Don't run update if the game's over
        if (gameOver) return;

        // Is the current player an AI? If so, let's see if we're supposed to
        // start the AI thinking task
        if (!sessionData.CurrentPlayer.IsHuman)
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
                IThinker thinker = sessionData.CurrentPlayer.Thinker;

                // Start task in a separate thread
                aiTask = Task.Run(() => thinker.Think(board, ts.Token));
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
                        taskStartGameTime + sessionData.TimeBetweenAIMoves)
                    {
                        // If so, do the move
                        MakeAMove(aiTask.Result);

                        // Submit a message informing of the move performed
                        // and the system time it took the AI to think
                        view.SubmitMessage(string.Format(
                            "{0} placed a {1} piece at column {2} after {3}",
                            CurrPlrNameColor,
                            aiTask.Result.shape.ToString().ToLower(),
                            aiTask.Result.column,
                            $"thinking for {lastTaskDuration:f4}s"));

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
                    OnGameOver();
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
                    OnGameOver();
                }
            }
        }
    }

    private void MakeAMove(FutureMove move)
    {
        PColor whoPlayed = board.Turn;
        int row = board.DoMove(move.shape, move.column);
        if (row >= 0)
        {
            Winner winner = board.CheckWinner(solution);
            if (winner != Winner.None)
            {
                PColor winColor = winner.ToPColor();
                Result = winner;
                OnGameOver();
            }
            else
            {
                view.SubmitMessage($"It's {CurrPlrNameColor} turn");
            }
            view.UpdateBoard(
                new Move(row, move.column, new Piece(whoPlayed, move.shape)),
                winner,
                solution);
        }
        else
        {
            view.SubmitMessage(
                $"Column {move.column + 1} is full, try another one.");
        }
    }

    private void OnGameOver()
    {
        view.SubmitMessage(string.Format("Game Over, {0}",
            Result == Winner.Draw
                ? "it's a draw"
                : $"{PlrNameColor(Result.ToPColor())} won"));
        gameOver = true;
        GameOver?.Invoke();
    }

    public UnityEvent GameOver { get; private set; }
}
