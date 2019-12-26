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
    private DateTime taskStart;
    private TimeSpan aiTimeLimit;
    private CancellationTokenSource ts;
    private float taskStartGameTime;
    private string CurrPlrNameColor => PlrNameColor(board.Turn);

    public string PlrNameColor(PColor color) =>
        $"{sessionData.GetPlayer(color).PlayerName} ({color})";

    public Winner Result { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
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
        if (gameOver) return;

        if (!sessionData.CurrentPlayer.IsHuman)
        {
            if (aiTask == null)
            {
                view.SubmitMessage(
                    $"{CurrPlrNameColor} is thinking, please wait...");
                taskStart = DateTime.Now;
                taskStartGameTime = Time.time;
                ts = new CancellationTokenSource();
                IThinker thinker = sessionData.CurrentPlayer.Thinker;
                aiTask = Task.Run(() => thinker.Think(board, ts.Token));
            }
            else
            {
                if (aiTask.IsCompleted)
                {
                    if (Time.time >
                        taskStartGameTime + sessionData.TimeBetweenAIMoves)
                    {
                        view.SubmitMessage(string.Format(
                            "{0} placed a {1} piece at column {2}",
                            CurrPlrNameColor,
                            aiTask.Result.shape.ToString().ToLower(),
                            aiTask.Result.column));
                        MakeAMove(aiTask.Result);
                        aiTask = null;
                    }
                }
                else if (aiTask.IsFaulted)
                {
                    view.SubmitMessage(
                        aiTask.Exception.InnerException.Message);
                    Debug.LogError(aiTask.Exception.InnerException.Message);
                    aiTask = null;
                }
                else if (DateTime.Now - taskStart > aiTimeLimit)
                {
                    view.SubmitMessage(
                        $"Time limit exceeded for {CurrPlrNameColor}!");
                    ts.Cancel();
                    aiTask = null;
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
