/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Move struct.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common
{
    /// <summary>Represents a move already performed.</summary>
    public struct Move
    {
        /// <summary>Board row where the piece was placed.</summary>
        public readonly int row;

        /// <summary>Board column where the piece was placed.</summary>
        public readonly int col;

        /// <summary>Piece used for the move.</summary>
        public readonly Piece piece;

        /// <summary>Create a move.</summary>
        /// <param name="row">Board row where the piece was placed.</param>
        /// <param name="col">Board column where the piece was placed.</param>
        /// <param name="piece">Piece used for the move.</param>
        public Move(int row, int col, Piece piece)
        {
            this.row = row;
            this.col = col;
            this.piece = piece;
        }
    }
}
