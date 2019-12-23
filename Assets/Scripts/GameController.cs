/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Text;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private const string rules = "Key B shows the board in the console.";

    private GameView view;

    private bool gameOver = false;
    private ISessionDataProvider sessionData;
    private Board board;


    // TODO Remove this
    private StringBuilder boardText = new StringBuilder();

    private void Awake()
    {
        sessionData = GetComponentInParent<ISessionDataProvider>();
        board = sessionData.Board;
        view = GameObject.Find("UI")?.GetComponent<GameView>();
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
            IThinker thinker = sessionData.CurrentPlayer.Thinker;
            FutureMove futureMove = thinker.Think(board);
            MakeAMove(futureMove);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            DrawBoard();
        }
    }

    public void MakeAMove(FutureMove move)
    {
        PColor whoPlayed = board.Turn;
        int row = board.DoMove(move.shape, move.column);
        if (row >= 0)
        {
            Winner winner = board.CheckWinner();
            if (winner != Winner.None)
            {
                Debug.Log("Game Over, " +
                    (winner == Winner.Draw ? "it's a draw" : winner + " won"));
                OnGameOver();
            }
            else
            {
                Debug.Log($"It's {board.Turn} turn");
            }
            view.UpdateBoard(
                new Move(row, move.column, new Piece(whoPlayed, move.shape)));
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

    // TODO Remove this
    private void DrawBoard()
    {
        boardText.Clear();
        boardText.Append('\n');
        for (int r = board.rows - 1; r >= 0; r--)
        {
            for (int c = 0; c < board.cols; c++)
            {
                char pc = '.';
                Piece? p = board[r, c];
                if (p.HasValue)
                {
                    if (p.Value.Is(PColor.White, PShape.Round))
                        pc = 'w';
                    else if (p.Value.Is(PColor.White, PShape.Square))
                        pc = 'W';
                    else if (p.Value.Is(PColor.Red, PShape.Round))
                        pc = 'r';
                    else if (p.Value.Is(PColor.Red, PShape.Square))
                        pc = 'R';
                    else
                        Debug.LogError($"Invalid piece {p.Value}");
                }
                boardText.Append(pc);
            }
            boardText.Append('\n');
        }
        Debug.Log(boardText.ToString());
    }

    public event Action GameOver;
}
