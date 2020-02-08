using System;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.ConsoleApp
{
    class Program
    {
        private static int rows = 6;
        private static int cols = 7;
        private static int winSequence = 4;
        private static int roundPiecesPerPlayer = 10;
        private static int squarePiecesPerPlayer = 11;
        private static double aiTimeLimit = double.MaxValue;
        private static double minAIMoveTime = 0.5;
        private static IThinker player1;
        private static IThinker player2;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
