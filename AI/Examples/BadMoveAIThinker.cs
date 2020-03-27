/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.AI.Examples.BadMoveAIThinker class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;

namespace ColorShapeLinks.Common.AI.Examples
{
    /// <summary>
    /// This thinker always tries to make an invalid or illogical move. Used
    /// for testing purposes only.
    /// </summary>
    public class BadMoveAIThinker : AbstractThinker
    {
        /// <summary>
        /// Types of invalid or illogical moves this thinker can make.
        /// </summary>
        public enum BadMove
        {
            /// <summary>
            /// Place a piece on a column above the number of existing columns.
            /// </summary>
            AboveColumn,

            /// <summary>
            /// Place a piece on a column below the number of existing columns,
            /// i.e., at -1.
            /// </summary>
            BelowColumn,

            /// <summary>
            /// Always try to place a piece in the same column.
            /// </summary>
            Repeat,

            /// <summary>
            /// Return a <see cref="FutureMove.NoMove"/>.
            /// </summary>
            NoMove,

            /// <summary>
            /// Always terminates with a timeout.
            /// </summary>
            Timeout,

            /// <summary>
            /// Refuses to terminate execution.
            /// </summary>
            Uncooperative,

            /// <summary>
            /// Throw an exception.
            /// </summary>
            Exception
        }

        // The type of invalid move the thinker will make
        private BadMove badMove;

        /// <summary>
        /// Specify type of invalid move the thinker will make.
        /// </summary>
        /// <param name="str">
        /// A string representation of <see cref="BadMove"/> values.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when an unknown option is given in <paramref name="str"/>.
        /// </exception>
        /// <seealso cref="AbstractThinker.Setup"/>
        public override void Setup(string str)
        {
            if (!Enum.TryParse<BadMove>(str, true, out badMove))
            {
                throw new ArgumentException(String.Format(
                    "Unknown option '{0}' for setting up {1}",
                    str, nameof(BadMoveAIThinker)));
            }
        }

        /// @copydoc IThinker.Think
        /// <seealso cref="IThinker.Think"/>
        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Bad move to return
            FutureMove futureMove;

            // Determine which bad move to return
            switch (badMove)
            {
                case BadMove.AboveColumn:
                    // Make move above highest column
                    futureMove =
                        new FutureMove(board.cols, board.Turn.Shape());
                    break;
                case BadMove.BelowColumn:
                    // Make move below lowest column
                    futureMove = new FutureMove(-1, board.Turn.Shape());
                    break;
                case BadMove.Repeat:
                    // Always places the same shape in the same column
                    futureMove = new FutureMove(0, board.Turn.Shape());
                    break;
                case BadMove.NoMove:
                    // Return a "no move"
                    futureMove = FutureMove.NoMove;
                    break;
                case BadMove.Timeout:
                    // Always timeout
                    futureMove = FutureMove.NoMove;
                    while (true)
                    {
                        // Wait enough millisseconds to lose
                        Thread.Sleep(TimeLimitMillis + 1);

                        // The task will eventually be cancelled due to a
                        // timeout
                        if (ct.IsCancellationRequested) break;
                    }
                    break;
                case BadMove.Uncooperative:
                    // Refuse to terminate
                    while (true) { }
                case BadMove.Exception:
                    // Throw an exception on purpose
                    throw new Exception(String.Format(
                        "{0} throwing an exception for testing purposes.",
                        nameof(BadMoveAIThinker)));
                default:
                    // By default thrown an exception letting the caller
                    // know this class was used unproperly
                    throw new InvalidOperationException(
                        $"Invalid use of {nameof(BadMoveAIThinker)}");
            }

            // Return the bad move
            return futureMove;
        }
    }
}
