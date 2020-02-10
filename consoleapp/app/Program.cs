using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.AI.Examples;

namespace ColorShapeLinks.ConsoleApp
{
    class Program : IGameConfig
    {
        private int rows = 6;
        private int cols = 7;
        private int winSequence = 4;
        private int roundPiecesPerPlayer = 10;
        private int squarePiecesPerPlayer = 11;
        private int timeLimitMillis = int.MaxValue;
        private double minAIMoveTime = 0.5;
        private IThinker player1;
        private IThinker player2;

        public int Rows => rows;
        public int Cols => cols;
        public int WinSequence => winSequence;
        public int SquarePiecesPerPlayer => squarePiecesPerPlayer;
        public int RoundPiecesPerPlayer => roundPiecesPerPlayer;
        public int TimeLimitMillis => timeLimitMillis;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        private void Run()
        {
            IThinker thinker1 = AIManager.Instance.NewThinker(
                typeof(RandomAIThinker).FullName, this, "");
            IThinker thinker2 = AIManager.Instance.NewThinker(
                typeof(SequentialAIThinker).FullName, this, "");

            Board board = new Board(
                rows, cols, winSequence, roundPiecesPerPlayer, squarePiecesPerPlayer);

            Winner winner = Winner.None;

            Pos[] solution = new Pos[winSequence];

            CancellationTokenSource ts = new CancellationTokenSource();

            while (true)
            {
                int row;
                FutureMove move;

                // Determine move for current
                move = thinker1.Think(board, ts.Token);

                // Perform move in game board, get column where move was performed
                row = board.DoMove(move.shape, move.column);

                // If the column had space for the move...
                if (row >= 0)
                {
                    // Update board view
                    Render(board);

                    // Get possible winner and solution
                    winner = board.CheckWinner(solution);

                    if (winner != Winner.None) break;
                }
                else // If we get here, column didn't have space for the move
                {
                    throw new InvalidOperationException("Invalid move");
                }

                // Determine move for current
                move = thinker2.Think(board, ts.Token);

                // Perform move in game board, get column where move was performed
                row = board.DoMove(move.shape, move.column);

                // If the column had space for the move...
                if (row >= 0)
                {
                    // Update board view
                    Render(board);

                    // Get possible winner and solution
                    winner = board.CheckWinner(solution);

                    if (winner != Winner.None) break;
                }
                else // If we get here, column didn't have space for the move
                {
                    throw new InvalidOperationException("Invalid move");
                }
            }

            Console.WriteLine("Winner is " + winner);
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
