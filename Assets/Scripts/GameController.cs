/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    private const string rules = "Key B shows the board in the console.";

    private GameView view;

    private bool gameOver = false;
    private ISessionDataProvider sessionData;
    private Board board;
    private Pos[] solution;

    private Task<FutureMove> aiTask;
    private DateTime taskStart;
    private TimeSpan aiTimeLimit;
    private CancellationTokenSource ts;
    private float timeLastAIMove;
    public Winner Result { get; private set; }

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
        Debug.Log(rules);
        Debug.Log($"It's {sessionData.Board.Turn} turn");
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver) return;

        if (!sessionData.CurrentPlayer.IsHuman)
        {
            if (aiTask == null)
            {
                if (Time.time > timeLastAIMove + sessionData.TimeBetweenAIMoves)
                {
                    taskStart = DateTime.Now;
                    ts = new CancellationTokenSource();
                    IThinker thinker = sessionData.CurrentPlayer.Thinker;
                    aiTask = Task.Run(() => thinker.Think(board, ts.Token));
                }
            }
            else
            {
                if (aiTask.IsCompleted)
                {
                    MakeAMove(aiTask.Result);
                    aiTask = null;
                    timeLastAIMove = Time.time;
                }
                else if (aiTask.IsFaulted)
                {
                    Debug.LogError(aiTask.Exception.InnerException.Message);
                    aiTask = null;
                }
                else if (DateTime.Now - taskStart > aiTimeLimit)
                {
                    ts.Cancel();
                    aiTask = null;
                    this.Result = board.Turn == PColor.White
                        ? Winner.Red : Winner.White;
                    OnGameOver();
                }
            }
        }
    }

    private void OnGUI()
    {
        void DrawAIThinkingWindow(int id)
        {
            if (id == 0)
            {
                GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);
                guiLabelStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(0, 0, 200, 100), "AI thinking", guiLabelStyle);
            }
        }

        if (aiTask?.Status == TaskStatus.Running)
        {
            GUI.ModalWindow(0,
                new Rect(
                    Screen.width / 2 - 100,
                    Screen.height / 2 - 50, 200, 100),
                DrawAIThinkingWindow,
                "AI thinking");
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
                Result = winner;
                Debug.Log("Game Over, " +
                    (winner == Winner.Draw ? "it's a draw" : winner + " won"));
                OnGameOver();
            }
            else
            {
                Debug.Log($"It's {board.Turn} turn");
            }
            view.UpdateBoard(
                new Move(row, move.column, new Piece(whoPlayed, move.shape)),
                winner,
                solution);
        }
        else
        {
            Debug.Log($"Column {move.column + 1} is full, try another one.");
        }
    }

    private void OnGameOver()
    {
        gameOver = true;
        GameOver?.Invoke();
    }

    public UnityEvent GameOver { get; private set; }
}
