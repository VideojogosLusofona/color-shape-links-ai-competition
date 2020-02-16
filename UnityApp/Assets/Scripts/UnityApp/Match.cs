/// @file
/// @brief This file contains the ::ColorShapeLinks.UnityApp.Match struct.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Represents one match in a session.
    /// </summary>
    public struct Match : IComparable<Match>
    {
        // Used for getting the next match ID
        private static int nextId = 0;

        // The match ID
        private readonly int id;

        /// <summary>First thinker in this match (white).</summary>
        public readonly IThinker thinker1;

        /// <summary>Second thinker in this match (ref).</summary>
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
            // Each new match will have increasing IDs
            id = nextId++;
            // Keep references to thinkers
            this.thinker1 = thinker1;
            this.thinker2 = thinker2;
        }

        /// <summary>Returns a string representation of this match.</summary>
        /// <returns>A string representation of this match.</returns>
        public override string ToString() => $"{thinker1} vs {thinker2}";

        /// <summary>
        /// Compares this match with another match. Comparisons are made using
        /// an internal ID, except if the thinkers are the same and in the same
        /// order, in which case matches are considered equal, even if they
        /// have different internal IDs.
        /// </summary>
        /// <param name="other">
        /// The match to compare the current match with.
        /// </param>
        /// <returns>
        /// * Zero if thinkers are the same and are in the same order.
        /// * Negative value if current match was created before match given in
        /// <paramref name="other"/>.
        /// * Positive value if current match was created after match given in
        /// <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Match other) => Equals(other) ? 0 : id - other.id;

        /// <summary>Returns a hash code for this match.</summary>
        /// <returns>
        /// Hash code obtain from the string concatenation of the thinker's
        /// names.
        /// </returns>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>
        /// Is the current match equal to the given object?
        /// Equality is verified if <paramref name="obj"/> is a
        /// <see cref="Match"/> object, and if it contains the same thinkers in
        /// the same positions.
        /// </summary>
        /// <param name="obj">
        /// Object to check for equality with this match.
        /// </param>
        /// <returns>
        /// `true` if this match is equal to <paramref name="obj"/>, `false`
        /// otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            Match other;
            if (obj == null || !(obj is Match)) return false;
            other = (Match)obj;
            return thinker1 == other.thinker1 && thinker2 == other.thinker2;
        }
    }
}
