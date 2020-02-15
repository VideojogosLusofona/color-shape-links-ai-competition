/// @file
/// @brief This file contains the ::ColorShapeLinks.TextBased.App.HumanThinker
/// class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.TextBased.App
{
    /// <summary>
    /// A human thinker for testing the console app.
    /// </summary>
    public class HumanThinker : AbstractThinker
    {
        // Currently selected column
        private int selectedCol;

        // Currently selected shape
        private PShape selectedShape;

        // The time limit for the human to play in the current turn
        private DateTime timeLimit;

        /// @copydoc ColorShapeLinks.Common.AI.AbstractThinker.Setup
        /// <seealso cref="ColorShapeLinks.Common.AI.AbstractThinker.Setup"/>
        public override void Setup(string str)
        {
            selectedCol = Cols / 2;
            selectedShape = PShape.Round;
            Console.Clear();
            Console.WriteLine("\n\n\n");
        }

        /// @copydoc ColorShapeLinks.Common.AI.IThinker.Think
        /// <remarks>
        /// This method asks the human to play.
        /// </remarks>
        /// <seealso cref="ColorShapeLinks.Common.AI.IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // By default, no move is performed in case of timeout
            FutureMove move = FutureMove.NoMove;

            // No thinking notification has taken place
            DateTime lastNotificationTime = DateTime.MinValue;

            // Set thinking notification interval between frames to 20ms
            TimeSpan notificationInterval = TimeSpan.FromMilliseconds(20);

            // Calculate the time limit
            timeLimit =
                DateTime.Now + TimeSpan.FromMilliseconds(TimeLimitMillis);

            // If a certain type of piece is not available, make the other
            // type the selected one
            if (board.PieceCount(board.Turn, PShape.Round) == 0)
                selectedShape = PShape.Square;
            if (board.PieceCount(board.Turn, PShape.Square) == 0)
                selectedShape = PShape.Round;

            // If the current column is full, iterate column selection until
            // one is not
            while (board.IsColumnFull(selectedCol))
            {
                selectedCol++;
                if (selectedCol >= Cols)
                    selectedCol = 0;
            }

            // Wait for human input at most until the cancellation token is
            // activated
            while (true)
            {
                // Was a key pressed?
                if (Console.KeyAvailable)
                {
                    // Retrieve key
                    ConsoleKey key = Console.ReadKey().Key;

                    // Check if it was a column increment key
                    if (key == ConsoleKey.RightArrow
                        || key == ConsoleKey.D
                        || key == ConsoleKey.NumPad6)
                    {
                        // Increment column...
                        do
                        {
                            // ...making sure a full column is not selectable,
                            // and wraping around
                            selectedCol++;
                            if (selectedCol >= Cols)
                                selectedCol = 0;
                        } while (board.IsColumnFull(selectedCol));
                    }
                    // Check if it was a column decrement key
                    else if (key == ConsoleKey.LeftArrow
                        || key == ConsoleKey.A
                        || key == ConsoleKey.NumPad4)
                    {
                        // Decrement column...
                        do
                        {
                            // ...making sure a full column is not selectable,
                            // and wraping around
                            selectedCol--;
                            if (selectedCol < 0)
                                selectedCol = Cols - 1;
                        } while (board.IsColumnFull(selectedCol));
                    }
                    // Check if it was a piece toggle key
                    else if (key == ConsoleKey.T)
                    {
                        // Toggle piece if the other type of piece is available
                        if (selectedShape == PShape.Round
                            && board.PieceCount(board.Turn, PShape.Square) > 0)
                        {
                            selectedShape = PShape.Square;
                        }
                        else if (selectedShape == PShape.Square
                            && board.PieceCount(board.Turn, PShape.Round) > 0)
                        {
                            selectedShape = PShape.Round;
                        }
                    }
                    // Check if it was a piece drop key
                    else if (key == ConsoleKey.Enter)
                    {
                        // Drop piece and get out of the input loop
                        move = new FutureMove(selectedCol, selectedShape);
                        Dialog(true);
                        break;
                    }
                }

                // If cancellation token is activated, terminate input loop
                if (ct.IsCancellationRequested)
                {
                    Dialog(true);
                    break;
                }

                // Is it time for another thinking notification?
                if (DateTime.Now > lastNotificationTime + notificationInterval)
                {
                    // Show dialog
                    Dialog();

                    // Update last notification time
                    lastNotificationTime = DateTime.Now;
                }
            }

            // Return chosen move
            return move;
        }

        // Show or clear the human input dialog
        private void Dialog(bool clear = false)
        {
            // Information to show in the dialog
            string dialog1 =
                $"  < > : Column [{selectedCol,8}] selected  ";
            string dialog2 =
                $"   T  : Piece  [{selectedShape,8}] selected  ";
            string dialog3 =
                $"   Time to play: {timeLimit - DateTime.Now,14}   ";

            // Keep where the cursor was before the dialog is shown or cleared
            int prevPosLeft = Console.CursorLeft;
            int prevPosTop = Console.CursorTop;

            // Determine where to place the dialog
            int dialogLeft = Console.WindowWidth / 2
                - dialog3.Length / 2;
            int dialogTop = Console.WindowHeight / 2 - 1;

            // Show we clear the dialog or show it?
            if (clear)
            {
                // Clear the dialog
                string blank = "".PadRight(dialog1.Length);
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(dialogLeft, dialogTop + i);
                    Console.Write(blank);
                }
            }
            else
            {
                // Show the dialog

                // Keep previous foreground and background colors
                ConsoleColor prevFg = Console.ForegroundColor;
                ConsoleColor prevBg = Console.BackgroundColor;

                // Set dialog colors
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;

                // Draw the dialog
                Console.SetCursorPosition(dialogLeft, dialogTop);
                Console.Write(dialog1);
                Console.SetCursorPosition(dialogLeft, dialogTop + 1);
                Console.Write(dialog2);
                Console.SetCursorPosition(dialogLeft, dialogTop + 2);
                Console.Write(dialog3);

                // Set the original foreground and background colors
                Console.BackgroundColor = prevFg;
                Console.ForegroundColor = prevBg;
            }

            // Set the cursor to its original position
            Console.SetCursorPosition(prevPosLeft, prevPosTop);
        }
    }
}
