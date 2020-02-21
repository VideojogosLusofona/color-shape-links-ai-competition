/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Session.Match class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// Represents one match in a tournament.
    /// </summary>
    public class Match
    {
        /// <summary>First thinker in this match (white).</summary>
        public readonly IThinkerPrototype thinkerWhite;

        /// <summary>Second thinker in this match (red).</summary>
        public readonly IThinkerPrototype thinkerRed;

        /// <summary>
        /// Indexer for getting the thinker's prototype based on his color.
        /// </summary>
        /// <param name="color">Thinker color.</param>
        /// <returns>
        /// The thinker's prototype associated with the given color.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when an invalid color is given.
        /// </exception>
        public IThinkerPrototype this[PColor color] =>
            color == PColor.White ? thinkerWhite
                : color == PColor.Red ? thinkerRed
                : throw new InvalidOperationException(
                    $"Invalid thinker color");

        /// <summary>A match with the thinkers swapped.</summary>
        /// <value>A new match instance.</value>
        public Match Swapped => new Match(thinkerRed, thinkerWhite);

        /// <summary>Create a new match.</summary>
        /// <param name="thinkerWhite">First thinker in match (white).</param>
        /// <param name="thinkerRed">Second thinker in match (red).</param>
        public Match(
            IThinkerPrototype thinkerWhite, IThinkerPrototype thinkerRed)
        {
            // Keep references to thinkers
            this.thinkerWhite = thinkerWhite;
            this.thinkerRed = thinkerRed;
        }

        /// <summary>Returns a string representation of this match.</summary>
        /// <returns>A string representation of this match.</returns>
        public override string ToString() =>
            $"{thinkerWhite.ThinkerName} vs {thinkerRed.ThinkerName}";
    }
}
