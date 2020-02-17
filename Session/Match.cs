/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Tournament.Match
/// class.
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
        public readonly IThinker thinker1;

        /// <summary>Second thinker in this match (red).</summary>
        public readonly IThinker thinker2;

        /// <summary>Indexer for getting a thinker based on his color.</summary>
        /// <param name="color">Thinker color.</param>
        /// <returns>The thinker associated with the given color.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when an invalid color is given.
        /// </exception>
        public IThinker this[PColor color] => color == PColor.White ? thinker1
                : color == PColor.Red ? thinker2
                    : throw new InvalidOperationException(
                        $"Invalid thinker color");

        /// <summary>A match with the thinkers swapped.</summary>
        /// <value>A new match instance.</value>
        public Match Swapped => new Match(thinker2, thinker1);

        /// <summary>Create a new match.</summary>
        /// <param name="thinker1">First thinker in match (white).</param>
        /// <param name="thinker2">Second thinker in match (red).</param>
        public Match(IThinker thinker1, IThinker thinker2)
        {
            // Keep references to thinkers
            this.thinker1 = thinker1;
            this.thinker2 = thinker2;
        }

        /// <summary>Returns a string representation of this match.</summary>
        /// <returns>A string representation of this match.</returns>
        public override string ToString() => $"{thinker1} vs {thinker2}";
    }
}
