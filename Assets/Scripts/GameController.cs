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
    private IPlayer[] players;
    private int rows;
    private int cols;
    private int winSequence;
    private int squarePiecesPerPlayer;
    private int roundPiecesPerPlayer;

    private const string rules = "Key B shows the board in the console.";

    private GameView view;

    private bool setupDone = false;
    private bool gameOver = false;

    private Board board;

    // TODO Remove this
    private StringBuilder boardText = new StringBuilder();

    private void Awake()
    {
        players = new IPlayer[2];
        view = GameObject.Find("UI")?.GetComponent<GameView>();
    }

    internal void SetupController(IPlayer player1White, IPlayer player2Red,
        int rows, int cols, int winSequence,
        int squarePiecesPerPlayer, int roundPiecesPerPlayer)
    {
        if (setupDone)
            throw new InvalidOperationException(
                "Game controller setup can only be performed once");

        players[(int)PColor.White] = player1White;
        players[(int)PColor.Red] = player2Red;
        this.rows = rows;
        this.cols = cols;

        board = new Board(rows, cols, winSequence,
            roundPiecesPerPlayer, squarePiecesPerPlayer);

        view.SetupView(board, players);

        setupDone = true;
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
        if (!setupDone)
            throw new InvalidOperationException(
                "Game controller setup needs to be performed before Start()");

        Debug.Log(rules);
        Debug.Log($"It's {board.Turn} turn");
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver) return;

        if (!players[(int)board.Turn].IsHuman)
        {
            IThinker thinker = players[(int)board.Turn].Thinker;
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
        for (int r = rows - 1; r >= 0; r--)
        {
            for (int c = 0; c < cols; c++)
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
