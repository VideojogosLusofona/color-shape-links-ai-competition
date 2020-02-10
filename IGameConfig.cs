/// @file
/// @brief This file contains the ::IGameConfig interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common
{
    /// <summary>
    /// Defines a game configuration, such as number of rows, number of
    /// columns, etc.
    /// </summary>
    public interface IGameConfig
    {
        /// <summary>Number of board rows.</summary>
        /// <value>Number of board rows.</value>
        int Rows { get; }

        /// <summary>Number of board columns.</summary>
        /// <value>Number of board columns.</value>
        int Cols { get; }

        /// <summary>How many pieces in sequence to find a winner.</summary>
        /// <value>How many pieces in sequence to find a winner.</value>
        int WinSequence { get; }

        /// <summary>Number of initial round pieces per player.</summary>
        /// <value>Number of initial round pieces per player.</value>
        int RoundPiecesPerPlayer { get; }

        /// <summary>Number of initial square round pieces per player</summary>
        /// <value>Number of initial square round pieces per player</value>
        int SquarePiecesPerPlayer { get; }

        /// <summary>Time limit for the AI to play in milliseconds.</summary>
        /// <value>Time limit for the AI to play in milliseconds.</value>
        int TimeLimitMillis { get; }
    }
}
