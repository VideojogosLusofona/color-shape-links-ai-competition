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

    public Board Board => board;

    private Board board;

    private const string rules =
        "Key T toggles the piece/shape to play. " +
        "Key U undoes the last move.";

    private PShape selectedShape;

    private StringBuilder boardText;

    private void Awake()
    {
        boardText = new StringBuilder(rows * cols + rows + 1);
        board = new Board(rows, cols, winSequence);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(rules);
        Debug.Log($"It's {board.Turn} turn");
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
            Move move = board.UndoMove();
            Debug.Log("Undid last move");
            Debug.Log($"It's {board.Turn} turn");
            OnBoardUpdate(move.row, move.col);
        }
    }

    public void MakeAMove(int col)
    {
        int row = board.DoMove(selectedShape, col);
        if (row >= 0)
        {
            Winner winner = board.CheckWinner();
            if (winner != Winner.None)
            {
                Debug.Log("Game Over, " +
                    (winner == Winner.Draw ? "it's a draw" : winner + " won"));
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Debug.Log($"It's {board.Turn} turn");
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
