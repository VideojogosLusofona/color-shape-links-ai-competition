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
            NoMove
        }

        // The type of invalid move the thinker will make
        private BadMove badMove;

        /// <summary>
        /// Specify type of invalid move the thinker will make.
        /// </summary>
        /// <param name="str">
        /// A string representation of BadMoveAIThinker.BadMove values.
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
                    futureMove =
                        new FutureMove(board.cols, board.Turn.Shape());
                    break;
                case BadMove.BelowColumn:
                    futureMove = new FutureMove(-1, board.Turn.Shape());
                    break;
                case BadMove.Repeat:
                    futureMove = new FutureMove(0, board.Turn.Shape());
                    break;
                case BadMove.NoMove:
                    futureMove = FutureMove.NoMove;
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Invalid use of {nameof(BadMoveAIThinker)}");
            }

            // Return the bad move
            return futureMove;
        }
    }
}
