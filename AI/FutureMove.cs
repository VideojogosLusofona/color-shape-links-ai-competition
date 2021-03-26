/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.AI.FutureMove
/// struct.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.AI
{
    /// <summary>
    /// Represents a move to be performed in the future.
    /// </summary>
    public struct FutureMove
    {
        /// <summary>
        /// The column where to drop the piece.
        /// </summary>
        public readonly int column;

        /// <summary>
        /// The piece to use in the move.
        /// </summary>
        public readonly PShape shape;

        /// <summary>
        /// Represent a "no move" decision, when an AI is unable to decide
        /// which move to perform.
        /// </summary>
        /// <value>A "no move" decision.</value>
        public static FutureMove NoMove => new FutureMove(-1, (PShape)(-1));

        /// <summary>
        /// Is this move a <see cref="NoMove"/>?
        /// </summary>
        /// <value>
        /// `true` if this is a <see cref="NoMove"/>, `false` otherwise.
        /// </value>
        public bool IsNoMove =>
            column == NoMove.column && shape == NoMove.shape;

        /// <summary>
        /// Create a future move.
        /// </summary>
        /// <param name="column">The column where to drop the piece.</param>
        /// <param name="shape">The piece to use in the move.</param>
        public FutureMove(int column, PShape shape)
        {
            this.column = column;
            this.shape = shape;
        }

        /// <summary>
        /// Provides a string representation of the future move in the form
        /// &quot;&lt;round|square&gt; piece at column &lt;col&gt;&quot;.
        /// </summary>
        /// <returns>A string representation of the future move.</returns>
        public override string ToString() =>
            $"{shape.ToString().ToLower()} piece at column {column}";
    }
}
