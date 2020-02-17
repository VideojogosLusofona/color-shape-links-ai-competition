/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.Tournament.Tournament class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using ColorShapeLinks.Common.AI;

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// Implements a session of ColorShapeLinks matches, setting up matches
    /// and keeping track of the results and classifications.
    /// </summary>
    public class Session : IEnumerable<Match>
    {
        // Internal auxiliary class used for match making
        private struct DummyThinker : IThinker
        {
            public FutureMove Think(Board board, CancellationToken ct)
                => throw new InvalidOperationException(
                    "This is just a dummy thinker");
        }

        // List of matches
        private IList<Match> matches;

        // Table of results
        private IDictionary<Match, Winner> results;

        // Current points for each thinker
        private IDictionary<IThinker, int> thinkerPoints;

        // Points per win, loss and draw
        private int pointsPerWin, pointsPerLoss, pointsPerDraw;

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <param name="thinkers">
        /// Thinkers participating in the session.
        /// </param>
        /// <param name="pointsPerWin">Points per win.</param>
        /// <param name="pointsPerLoss">Points per loss.</param>
        /// <param name="pointsPerDraw">Points per draw.</param>
        /// <param name="complete">
        /// Is this a complete tournament, i.e., do thinkers play against
        /// opponents home and away (two games)?
        /// </param>
        public Session(IEnumerable<IThinker> thinkers,
            int pointsPerWin, int pointsPerLoss, int pointsPerDraw,
            bool complete = false)
        {
            // Create a list of thinkers from the given enumerable
            IList<IThinker> thinkersList = new List<IThinker>(thinkers);

            // Number of thinkers
            int numThinkers = thinkersList?.Count ?? 0;

            // Check if there are enough thinkers
            if (numThinkers < 2)
                throw new InvalidOperationException(
                    "A tournament must have at least two thinkers, "
                    + $"but only {numThinkers} where specified");

            // Keep note of points per win, draw and loss
            this.pointsPerWin = pointsPerWin;
            this.pointsPerLoss = pointsPerLoss;
            this.pointsPerDraw = pointsPerDraw;

            // Create and populate the list of thinker points
            thinkerPoints = new Dictionary<IThinker, int>(numThinkers);
            foreach (IThinker t in thinkersList) thinkerPoints.Add(t, 0);

            // Initialize the list of matches
            matches = new List<Match>();

            // Initialize the list of results
            results = new Dictionary<Match, Winner>();

            // In this mode we need an even number of players to set up the
            // matches, so add a fake one if necessary
            if (thinkersList.Count % 2 != 0)
                thinkersList.Add(new DummyThinker());

            // Setup matches using the round-robin method
            // https://en.wikipedia.org/wiki/Round-robin_tournament
            for (int i = 1; i < thinkersList.Count; i++)
            {
                // This will be the thinker to swap position after each round
                IThinker thinkerToSwapPosition;

                // Set up matches for current round i
                for (int j = 0; j < thinkersList.Count / 2; j++)
                {
                    // This is match j for current round i
                    Match match = new Match(
                        thinkersList[j],
                        thinkersList[thinkersList.Count - 1 - j]);
                    // Only add match to match list if it's not a dummy
                    // match
                    if (!(match.thinker1 is DummyThinker
                        || match.thinker2 is DummyThinker))
                    {
                        // Add match to match list
                        matches.Add(match);

                        // If the tournament is complete, also add a match with
                        // the same thinkers but with reversed roles
                        if (complete)
                        {
                            matches.Add(match.Swapped);
                        }
                    }
                }
                // Swap AI positions for next round
                thinkerToSwapPosition =
                    thinkersList[thinkersList.Count - 1];
                thinkersList.RemoveAt(thinkersList.Count - 1);
                thinkersList.Insert(1, thinkerToSwapPosition);
            }
        }

        /// <summary>
        /// Returns the current results.
        /// </summary>
        /// <returns>The current results.</returns>
        public IEnumerable<KeyValuePair<Match, Winner>> GetResults()
        {
            // Loop through all matches
            foreach (Match m in matches)
            {
                // If there is a result for the current match...
                if (results.ContainsKey(m))
                    // ...return it
                    yield return new KeyValuePair<Match, Winner>(m, results[m]);
            }
        }

        /// <summary>
        /// Return the current standings/classification.
        /// </summary>
        /// <returns>The current standings/classification.</returns>
        public IEnumerable<KeyValuePair<IThinker, int>> GetStandings()
        {
            // Create an array to place thinkers and their points
            KeyValuePair<IThinker, int>[] standings =
                new KeyValuePair<IThinker, int>[thinkerPoints.Count];

            // Populate the array with thinkers and their points
            thinkerPoints.CopyTo(standings, 0);

            // Sort the array in descending order according to thinker points
            Array.Sort(standings, (a, b) => b.Value - a.Value);

            // Return each thinker and its points
            foreach (KeyValuePair<IThinker, int> kvp in standings)
                yield return kvp;
        }

        /// <summary>
        /// Return all scheduled matches, completed or otherwise.
        /// </summary>
        /// <returns>All scheduled matches.</returns>
        public IEnumerator<Match> GetEnumerator()
        {
            foreach (Match m in matches)
                yield return m;
        }

        /// <summary>
        /// Explicit implementation of the
        /// <see cref="System.Collections.IEnumerable"/> interface.
        /// </summary>
        /// <returns>An enumerator with all scheduled matches.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Set the result of a given match.
        /// </summary>
        /// <param name="match">Match to set the result of.</param>
        /// <param name="result">Result of the given match.</param>
        public void SetResult(Match match, Winner result)
        {
            // Add to results table
            results.Add(match, result);

            // Update thinker points
            switch (result)
            {
                // White won
                case Winner.White:
                    thinkerPoints[match.thinker1] += pointsPerWin;
                    thinkerPoints[match.thinker2] += pointsPerLoss;
                    break;
                // Red won
                case Winner.Red:
                    thinkerPoints[match.thinker2] += pointsPerWin;
                    thinkerPoints[match.thinker1] += pointsPerLoss;
                    break;
                // Game ended in a draw
                case Winner.Draw:
                    thinkerPoints[match.thinker1] += pointsPerDraw;
                    thinkerPoints[match.thinker2] += pointsPerDraw;
                    break;
                // Invalid situation
                default:
                    throw new InvalidOperationException(
                        "Invalid end of match result");
            }
        }
    }
}
