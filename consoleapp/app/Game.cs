/// @file
/// @brief This file contains the ::Game class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using System.Threading.Tasks;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.ConsoleApp
{
    public class Game
    {
        private Options options;
        private Board board;
        private IThinker[] thinkers;

        public Game(Options options)
        {
            this.options = options;
        }

        public void Run()
        {

            Winner winner = Winner.None;
            PColor winColor;
            Pos[] solution;

            thinkers = new IThinker[2];

            thinkers[0] = AIManager.Instance.NewThinker(
                options.Player1, options, options.Player1Params);
            thinkers[1] = AIManager.Instance.NewThinker(
                options.Player2, options, options.Player2Params);

            board = new Board(options.Rows, options.Cols,
                options.WinSequence, options.RoundPiecesPerPlayer,
                options.SquarePiecesPerPlayer);

            while (true)
            {
                (winner, solution) = Play(PColor.White);
                if (winner != Winner.None) break;
                (winner, solution) = Play(PColor.Red);
                if (winner != Winner.None) break;
            }

            Render(board);

            if (winner == Winner.Draw)
            {
                Console.WriteLine("Game ended in a draw");
            }
            else
            {
                int winPlayer = (int)winner.ToPColor();
                winColor = winner.ToPColor();
                Console.WriteLine($"Winner is player {winPlayer + 1} ({winner}, {thinkers[winPlayer]})");
                if (solution != null)
                {
                    Console.Write("Solution=");
                    foreach (Pos pos in solution)
                    {
                        Console.Write(pos);
                    }
                    Console.WriteLine();
                }
            }
        }

        private (Winner, Pos[]) Play(PColor color)
        {
            int row;
            FutureMove move;
            CancellationTokenSource ts = new CancellationTokenSource();
            IThinker thinker = thinkers[(int)color];
            Winner winner = Winner.None;
            Pos[] solution = new Pos[options.WinSequence];

            // Task to execute the thinker in a separate thread
            Task<FutureMove> thinkTask;

            string player = $"Player {(int)color + 1} ({color}, {thinker})";

            // Update board view
            Render(board);

            Console.WriteLine($"{player} turn");

            thinkTask = Task.Run(
                    () => thinker.Think(board.Copy(), ts.Token));

            if (!thinkTask.Wait(options.TimeLimitMillis))
            {
                ts.Cancel();
                winner = color == PColor.Red ? Winner.White : Winner.Red;
                solution = null;
                Console.WriteLine($"{player} took too long to play!");
            }
            else
            {
                // Get move for current player
                move = thinkTask.Result;

                // Perform move in game board, get column where move was
                // performed
                row = board.DoMove(move.shape, move.column);

                // If the column had space for the move...
                if (row >= 0)
                {
                    Console.WriteLine(
                        $"{player} placed a {move.shape} piece at column {move.column}");

                    // Get possible winner and solution
                    winner = board.CheckWinner(solution);
                }
                else // If we get here, column didn't have space for the move
                {
                    throw new InvalidOperationException("Invalid move");
                }
            }
            return (winner, solution);
        }

        private void Render(Board board)
        {
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
                            Console.Error.WriteLine($"Invalid piece {p.Value}");
                    }
                    Console.Write(pc);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }
}