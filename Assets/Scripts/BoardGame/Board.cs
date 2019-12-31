/// @file
/// @brief This file contains the ::Board class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;

/// <summary>Represents the game board.</summary>
public class Board
{
    /// <summary>
    /// Read-only indexer for allowing the board contents to be accessed by
    /// external code.
    /// </summary>
    /// <param name="row">Board row to access.</param>
    /// <param name="col">Board column to access.</param>
    /// <returns>
    /// The piece in the specified board position, or `null` if no piece is in
    /// the given position.
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">
    /// Thrown when the given position is invalid (outside the board).
    /// </exception>
    public Piece? this[int row, int col]
    {
        get
        {
            // If client requested an invalid position, this is a bug in
            // client code, so let's throw an exception
            if (row < 0 || row >= rows || col < 0 || col >= cols)
            {
                throw new IndexOutOfRangeException(
                    $"Position {new Pos(row, col)} is out of bounds. " +
                    $"Board dimensions are {new Pos(rows, cols)}.");
            }

            // Return piece
            return board[col, row];
        }
    }

    /// <summary>Who's turn is it?</summary>
    /// <value>The color of the player who's currently playing.</value>
    public PColor Turn { get; private set; }

    /// <summary>Number of rows in the board.</summary>
    public readonly int rows;

    /// <summary>Number of columns in the board.</summary>
    public readonly int cols;

    /// <summary>How many pieces in sequence to find a winner.</summary>
    public readonly int piecesInSequence;

    /// <summary>Initial number of round pieces for each player.</summary>
    public readonly int roundPieces;

    /// <summary>Initial number of square pieces for each player.</summary>
    public readonly int squarePieces;

    /// <summary>
    /// Array of win corridors, which can be used for building an heuristic.
    /// </summary>
    public readonly IEnumerable<IEnumerable<Pos>> winCorridors;

    // Internal struct for representing the pair
    // (piece check function, player associated with piece)
    private struct PieceFuncPlayer
    {
        // Function that checks if a piece belong to a player
        public readonly Func<Piece, bool> checkPieceFunc;
        // The player to be associated with a piece
        public readonly PColor player;
        // Create an new pair (piece check function, associated player)
        public PieceFuncPlayer(
            Func<Piece, bool> checkPieceFunc, PColor player)
        {
            this.checkPieceFunc = checkPieceFunc;
            this.player = player;
        }
    }

    // Array of pairs (piece check function, player associated with piece)
    private readonly PieceFuncPlayer[] pieceFuncsPlayers;

    // How many pieces left?
    private int PiecesLeft
    {
        get
        {
            int total = 0;
            IEnumerable<int> counts = numberOfPieces.Values;
            foreach (int count in counts) total += count;
            return total;
        }
    }

    // Internal representation of the game board
    private Piece?[,] board;

    // Sequence of moves (for undo purposes)
    private Stack<Pos> moveSequence;

    // Number of pieces
    private IDictionary<Piece, int> numberOfPieces;

    // Number of moves performed so far
    private int numMoves;

    /// <summary>
    /// Creates a new board.
    /// </summary>
    /// <param name="rows">Number of rows.</param>
    /// <param name="cols">Number of columns.</param>
    /// <param name="piecesInSequence">
    /// Number of required pieces in a row for player to win.
    /// </param>
    /// <param name="roundPieces">
    /// Initial number of round pieces per player.
    /// </param>
    /// <param name="squarePieces">
    /// Initial number of square pieces per player.
    /// </param>
    public Board(int rows = 7, int cols = 7, int piecesInSequence = 4,
        int roundPieces = 10, int squarePieces = 11)
    {
        // Aux. variables for determining win corridors
        List<Pos> aCorridor;
        List<IEnumerable<Pos>> corridors;

        // Is it possible to win?
        if (rows < piecesInSequence && cols < piecesInSequence)
        {
            throw new InvalidOperationException(
                "Invalid parameters, since it is not possible to win");
        }

        // Keep number of rows
        this.rows = rows;

        // Keep number of columns
        this.cols = cols;

        // Keep number of pieces in sequence to find winner
        this.piecesInSequence = piecesInSequence;

        // Keep initial number of round pieces
        this.roundPieces = roundPieces;

        // Keep initial number of square pieces
        this.squarePieces = squarePieces;

        // Number of moves initially zero
        numMoves = 0;

        // Initially, it's player 1 turn
        Turn = PColor.White;

        // Instantiate the array representing the board
        board = new Piece?[cols, rows];

        // Initialize the array of functions for checking pieces
        pieceFuncsPlayers = new PieceFuncPlayer[]
        {
            // Shape must come before color
            new PieceFuncPlayer(p => p.shape == PShape.Round, PColor.White),
            new PieceFuncPlayer(p => p.shape == PShape.Square, PColor.Red),
            new PieceFuncPlayer(p => p.color == PColor.White, PColor.White),
            new PieceFuncPlayer(p => p.color == PColor.Red, PColor.Red)
        };

        // Create and populate array of win corridors
        corridors = new List<IEnumerable<Pos>>();
        aCorridor = new List<Pos>(Math.Max(rows, cols));

        // Initialize move sequence with initial capacity for all possible
        // moves
        moveSequence = new Stack<Pos>(rows * cols);

        // Setup initial number of pieces
        numberOfPieces = new Dictionary<Piece, int>()
        {
            { new Piece(PColor.White, PShape.Round), roundPieces },
            { new Piece(PColor.White, PShape.Square), squarePieces },
            { new Piece(PColor.Red, PShape.Round), roundPieces },
            { new Piece(PColor.Red, PShape.Square), squarePieces },
        };

        //
        // Horizontal corridors
        //
        if (cols >= piecesInSequence)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    aCorridor.Add(new Pos(r, c));
                }
                corridors.Add(aCorridor.ToArray());
                aCorridor.Clear();
            }
        }

        //
        // Vertical corridors
        //
        if (rows >= piecesInSequence)
        {
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    aCorridor.Add(new Pos(r, c));
                }
                corridors.Add(aCorridor.ToArray());
                aCorridor.Clear();
            }
        }

        //
        // Diagonal corridors /
        //
        // Down
        for (int row = rows - 1; row >= 0; row--)
        {
            int r = row;
            int c = 0;
            while (r < rows && c < cols)
            {
                aCorridor.Add(new Pos(r, c));
                r++;
                c++;
            }
            if (aCorridor.Count >= this.piecesInSequence)
                corridors.Add(aCorridor.ToArray());
            aCorridor.Clear();
        }
        // Right
        for (int col = 1; col < cols; col++)
        {
            int r = 0;
            int c = col;
            while (r < rows && c < cols)
            {
                aCorridor.Add(new Pos(r, c));
                r++;
                c++;
            }
            if (aCorridor.Count >= this.piecesInSequence)
                corridors.Add(aCorridor.ToArray());
            aCorridor.Clear();
        }

        //
        // Diagonal corridors \
        //
        // Down
        for (int row = rows - 1; row >= 0; row--)
        {
            int r = row;
            int c = cols - 1;
            while (r < rows && c >= 0)
            {
                aCorridor.Add(new Pos(r, c));
                r++;
                c--;
            }
            if (aCorridor.Count >= this.piecesInSequence)
                corridors.Add(aCorridor.ToArray());
            aCorridor.Clear();
        }
        // Left
        for (int col = cols - 2; col >= 0; col--)
        {
            int r = 0;
            int c = col;
            while (r < rows && c >= 0)
            {
                aCorridor.Add(new Pos(r, c));
                r++;
                c--;
            }
            if (aCorridor.Count >= this.piecesInSequence)
                corridors.Add(aCorridor.ToArray());
            aCorridor.Clear();
        }

        // Keep the final list of win corridors
        winCorridors = corridors.ToArray();
    }

    /// <summary>
    /// Make a move, return row
    /// </summary>
    /// <param name="shape">
    /// Shape of piece used in move (color is obtained from
    /// <see cref="Board.Turn"/>).
    /// </param>
    /// <param name="col">Column where to drop piece.</param>
    /// <returns>Row where piece was placed or -1 if move is invalid.</returns>
    public int DoMove(PShape shape, int col)
    {
        // The row were to place the piece, initially assumed to be the top row
        int row = rows - 1;

        // The color of the piece to place, depends on who's playing
        PColor color = Turn;

        // The piece to place
        Piece piece = new Piece(color, shape);

        // If the column is not a valid column, there is a client code bug,
        // so let's throw an exception
        if (col < 0 || col >= cols)
        {
            throw new InvalidOperationException($"Invalid board column: {col}");
        }

        // If we already found a winner, there is a client code bug, so let's
        // throw an exception
        if (CheckWinner() != Winner.None)
        {
            throw new InvalidOperationException(
                "Game is over, unable to make further moves.");
        }

        // If there are no more pieces of the specified kind, there is a client
        // bug, so let's throw an exception
        if (numberOfPieces[piece] == 0)
        {
            throw new InvalidOperationException(
                $"No more {piece} pieces available");
        }

        // If column is already full, return negative value, indicating the
        // move is invalid
        if (board[col, row].HasValue) return -1;

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

        // Decrease the piece count
        numberOfPieces[piece]--;

        // Remember the move
        moveSequence.Push(new Pos(row, col));

        // Increment number of moves
        numMoves++;

        // Update turn
        Turn = Turn == PColor.White ? PColor.Red : PColor.White;

        // Return true, indicating the move was successful
        return row;
    }

    /// <summary>Undo last move.</summary>
    /// <returns>The <see cref="Move"/> that was undone.</returns>
    public Move UndoMove()
    {
        Pos pos;
        Piece piece;

        // If no undo is possible, there is a bug in client code, so let's
        // throw an exception
        if (moveSequence.Count == 0)
        {
            throw new InvalidOperationException("No moves to undo.");
        }

        // Get last move position
        pos = moveSequence.Pop();

        // If there is no piece in last moves' position, there is a bug
        // somewhere
        if (!board[pos.col, pos.row].HasValue)
        {
            throw new InvalidOperationException(
                $"No piece in undo position {pos}. Board in invalid state.");
        }

        // Get the piece from the board
        piece = board[pos.col, pos.row].Value;
        board[pos.col, pos.row] = null;

        // Decrement number of moves
        numMoves--;

        // Swap turns
        Turn = Turn == PColor.White ? PColor.Red : PColor.White;

        // Return move that was undone
        return new Move(pos.row, pos.col, piece);
    }

    /// <summary>
    /// Check if there is a winner.
    /// </summary>
    /// <param name="solution">
    /// Optional parameter where solution will be placed if there is a winner.
    /// </param>
    /// <returns>The <see cref="Winner"/> of the current game, if any.</returns>
    public Winner CheckWinner(Pos[] solution = null)
    {
        // If an array for placing the solution was given, make sure it has
        // the required capacity
        if (solution != null && solution.Length < piecesInSequence)
            throw new InvalidOperationException("Array for placing solution " +
                $"must have at least length {piecesInSequence}");

        // Is the board full? Then we have a draw
        if (numMoves == cols * rows || PiecesLeft == 0) return Winner.Draw;

        // Check for all different pieces
        foreach (PieceFuncPlayer funcPlayer in pieceFuncsPlayers)
        {
            // Check all possible corridors
            foreach (IEnumerable<Pos> corridor in winCorridors)
            {
                // Reset count for this corridor
                int count = 0;

                // Check positions in this corridor
                foreach (Pos p in corridor)
                {
                    // Update solution array
                    if (solution != null) solution[count] = p;

                    // Does position contain the appropriate piece?
                    if (board[p.col, p.row].HasValue &&
                        funcPlayer.checkPieceFunc(board[p.col, p.row].Value))
                        // Yes it does, increment counter
                        count++;
                    else
                        // No it doesn't, reset count
                        count = 0;
                    // Did we find enough pieces in a row?
                    if (count == piecesInSequence)
                        // If so, return winner
                        return funcPlayer.player == PColor.White
                            ? Winner.White
                            : Winner.Red;
                }
            }
        }

        // No winner found
        return Winner.None;
    }

    /// <summary>
    /// Returns a copy of this board which can be freely modified by the AI
    /// in a different thread.
    /// </summary>
    /// <returns>A copy of this board.</returns>
    public Board Copy()
    {
        // The move sequence in the new board
        Pos[] newBoardMoveSequence;

        // Create a shallow copy of the board
        Board newBoard = (Board)MemberwiseClone();

        // Deep-clone the board array
        newBoard.board = board.Clone() as Piece?[,];

        // Deep-clone the move sequence
        newBoardMoveSequence = moveSequence.ToArray();
        Array.Reverse(newBoardMoveSequence);
        newBoard.moveSequence = new Stack<Pos>(newBoardMoveSequence);

        // Deep-clone the number of pieces
        newBoard.numberOfPieces = new Dictionary<Piece, int>(numberOfPieces);

        // Return a deep copy of the board
        return newBoard;
    }

    /// <summary>
    /// Get number of remaining pieces with the given color and shape.
    /// </summary>
    /// <param name="color">Pieces color.</param>
    /// <param name="shape">Pieces shape.</param>
    /// <returns>
    /// Number of remaining pieces with the given color and shape.
    /// </returns>
    public int PieceCount(PColor color, PShape shape) =>
        numberOfPieces[new Piece(color, shape)];

    /// <summary>
    /// Is the specified column full?
    /// </summary>
    /// <param name="col">The column to check if it's full.</param>
    /// <returns>`true` if the column is full, `false` otherwise.</returns>
    public bool IsColumnFull(int col) => board[col, rows - 1].HasValue;
}
