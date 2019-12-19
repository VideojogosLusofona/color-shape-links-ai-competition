/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;

// Represents the game board
public class Board
{
    // Internal struct for representing the pair
    // (piece check function, player associated with piece)
    private struct PieceFuncPlayer
    {
        public readonly Func<Piece, bool> checkPieceFunc;
        public readonly Winner player;
        public PieceFuncPlayer(
            Func<Piece, bool> checkPieceFunc, Winner player)
        {
            this.checkPieceFunc = checkPieceFunc;
            this.player = player;
        }
    }

    // Number of rows in the board
    public int Rows =>  board.GetLength(1);

    // Number of columns in the board
    public int Cols =>  board.GetLength(0);

    // How many pieces in sequence to find a winner
    public int PiecesInSequence { get; }

    // Internal representation of the game board
    private readonly Piece?[,] board;

    // Array of pairs (piece check function, player associated with piece)
    private readonly PieceFuncPlayer[] pieceFuncsPlayers;

    // Number of moves performed so far
    private int numMoves;

    // Creates a new board
    public Board(int rows, int cols, int piecesInSequence)
    {
        // Number of moves initially zero
        numMoves = 0;

        // Keep number of pieces in sequence to find winner
        PiecesInSequence = piecesInSequence;

        // Instantiate the array representing the board
        board = new Piece?[cols, rows];

        // Initialize the array of functions for checking pieces
        pieceFuncsPlayers = new PieceFuncPlayer[]
        {
            // Shape must come before color
            new PieceFuncPlayer(p => p.shape == Shape.Round, Winner.Player1),
            new PieceFuncPlayer(p => p.shape == Shape.Square, Winner.Player2),
            new PieceFuncPlayer(p => p.color == Color.White, Winner.Player1),
            new PieceFuncPlayer(p => p.color == Color.Red, Winner.Player2)
        };
    }

    // Make a move
    public bool MakeAMove(Piece piece, int col)
    {
        // The row were to place the piece, initially assumed to be the top row
        int row = Rows - 1;

        // If we already found a winner, there is a client code bug, so let's
        // throw an exception
        if (CheckWinner() != Winner.None)
        {
            throw new InvalidOperationException(
                "Game is over, unable to make further moves.");
        }

        // If the column is not a valid column, there is a client code bug,
        // so let's throw an exception
        if (col < 0 || col >= Cols)
        {
            throw new InvalidOperationException(
                $"Invalid board column: {col}");
        }

        // If column is already full, return false, indicating the move is
        // invalid
        if (board[col, row].HasValue) return false;

        //
        // If we get here, move is valid, so let's do it
        //

        // Find row where to place the piece
        for (int r = row - 1; r >= 0 && !board[col, r].HasValue; r--)
        {
            row = r;
        }

        // Place the piece
        board[col, row] = piece;

        // Increment number of moves and return true, indicating the move was
        // successful
        numMoves++;
        return true;
    }

    // Is there a winner?
    public Winner CheckWinner()
    {
        // Is the board full? Then we have a draw
        if (numMoves == Cols * Rows) return Winner.Draw;

        // Check for all different pieces
        foreach (PieceFuncPlayer funcPlayer in  pieceFuncsPlayers)
        {
            // Check horizontally
            for (int r = 0; r < Rows; r++)
            {
                int count = 0;
                for (int c = 0; c < Cols; c++)
                {


                }
            }

            // Check vertically
            for (int c = 0; c < Cols; c++)
            {
                int count = 0;
                for (int r = 0; r < Rows; r++)
                {


                }
            }

            // Check diagonally
        }

        return Winner.None;
    }
}
