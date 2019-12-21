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
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 7;
    [SerializeField] private int winSequence = 4;
    [SerializeField] private int squarePiecesPerPlayer = 11;
    [SerializeField] private int roundPiecesPerPlayer = 10;

    public Board Board { get; private set; }

    private int player1SquarePieces;
    private int player1RoundPieces;
    private int player2SquarePieces;
    private int player2RoundPieces;

    private const string rules =
        "Key T toggles the piece/shape to play. " +
        "Key U undoes the last move.";

    private PShape selectedShape;

    public bool IsOver { get; set; }

    private void Awake()
    {
        IsOver = false;
        Board = new Board(rows, cols, winSequence);
        player1SquarePieces = squarePiecesPerPlayer;
        player1RoundPieces = roundPiecesPerPlayer;
        player2SquarePieces = squarePiecesPerPlayer;
        player2RoundPieces = roundPiecesPerPlayer;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(rules);
        Debug.Log($"It's {Board.Turn} turn");
    }

    // Update is called once per frame
    private void Update()
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
                IsOver = true;
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

    public event Action<int, int> BoardUpdate;
}
