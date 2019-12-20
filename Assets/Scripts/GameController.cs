/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 7;
    [SerializeField] private int winSequence = 4;

    private Board board;

    private const string rules =
        "Key T toggles the piece/shape to play. " +
        "Keys 1-7 make a move with the selected piece. " +
        "Key U undoes the last move.";

    private Shape selectedShape;

    private StringBuilder boardText;

    // Start is called before the first frame update
    private void Start()
    {
        boardText = new StringBuilder(rows * cols + rows + 1);
        board = new Board(rows, cols, winSequence);
        Debug.Log(rules);
        DrawBoard();
        Debug.Log($"It's {board.Turn} turn");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MakeAMove(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MakeAMove(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MakeAMove(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MakeAMove(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MakeAMove(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MakeAMove(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MakeAMove(6);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            selectedShape = selectedShape == Shape.Round
                ? Shape.Square
                : Shape.Round;
            Debug.Log($"Selected shape is {selectedShape}");
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            board.UndoMove();
            DrawBoard();
            Debug.Log($"It's {board.Turn} turn");
        }
    }

    private void MakeAMove(int col)
    {
        if (board.DoMove(selectedShape, col))
        {
            Winner winner = board.CheckWinner();
            DrawBoard();
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
        }
        else
        {
            Debug.Log($"Column {col + 1} is full, try another one.");
        }
    }

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
                    if (p.Value.Is(Color.White, Shape.Round))
                        pc = 'w';
                    else if (p.Value.Is(Color.White, Shape.Square))
                        pc = 'W';
                    else if (p.Value.Is(Color.Red, Shape.Round))
                        pc = 'r';
                    else if (p.Value.Is(Color.Red, Shape.Square))
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
}
