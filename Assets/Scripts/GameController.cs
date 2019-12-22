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

    private const string rules =
        "Key T toggles the piece/shape to play. " +
        "Key U undoes the last move.";

    private PShape selectedShape;

    private bool setupDone = false;
    private bool gameOver = false;

    public Board Board { get; private set; }

    public void Awake()
    {
        players = new IPlayer[2];
    }

    public void SetupGame(IPlayer player1White, IPlayer player2Red,
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
        this.winSequence = winSequence;
        this.squarePiecesPerPlayer = squarePiecesPerPlayer;
        this.roundPiecesPerPlayer = roundPiecesPerPlayer;

        setupDone = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!setupDone)
            throw new InvalidOperationException(
                "Game controller setup needs to be performed before Start()");

        Board = new Board(rows, cols, winSequence,
            roundPiecesPerPlayer, squarePiecesPerPlayer);

        Debug.Log(rules);
        Debug.Log($"It's {Board.Turn} turn");
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver) return;

        if (players[(int)Board.Turn].IsHuman)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                selectedShape = selectedShape == PShape.Round
                    ? PShape.Square
                    : PShape.Round;
                Debug.Log($"Selected shape is {selectedShape}");
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                Move move = Board.UndoMove();
                Debug.Log("Undid last move");
                Debug.Log($"It's {Board.Turn} turn");
                OnBoardUpdate(move.row, move.col);
            }
        }
        else
        {
            IThinker thinker = (players[(int)Board.Turn] as AIPlayer).Thinker;
            FutureMove futureMove = thinker.Think(Board);
            PShape aux = selectedShape;
            selectedShape = futureMove.shape;
            MakeAMove(futureMove.column);
            selectedShape = aux;
        }
    }

    public void MakeAMove(int col)
    {
        int row = Board.DoMove(selectedShape, col);
        if (row >= 0)
        {
            Winner winner = Board.CheckWinner();
            if (winner != Winner.None)
            {
                Debug.Log("Game Over, " +
                    (winner == Winner.Draw ? "it's a draw" : winner + " won"));
                OnGameOver();
            }
            else
            {
                Debug.Log($"It's {Board.Turn} turn");
            }
            OnBoardUpdate(row, col);
        }
        else
        {
            Debug.Log($"Column {col + 1} is full, try another one.");
        }
    }

    private void OnBoardUpdate(int row, int col)
    {
        BoardUpdate?.Invoke(row, col);
    }

    private void OnGameOver()
    {
        gameOver = true;
        GameOver?.Invoke();
    }

    public event Action<int, int> BoardUpdate;
    public event Action GameOver;
}
