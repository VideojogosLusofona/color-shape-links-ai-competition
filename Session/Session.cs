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
        private class DummyThinkerPrototype : IThinkerPrototype
        {
            public string ThinkerName => "Dummy";
            public IThinker Create() => null;
        }

        // List of matches
        private IList<Match> matches;

        // Table of results
        private IDictionary<Match, Winner> results;

        // Current points for each thinker
        private IDictionary<string, int> thinkerPoints;

        // Points per win, loss and draw
        private int pointsPerWin, pointsPerLoss, pointsPerDraw;

        // Match enumerator
        private IEnumerator<Match> matchEnumerator;

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <param name="thinkerPrototypes">
        /// Prototypes of thinkers participating in the session.
        /// </param>
        /// <param name="sessionConfig">Session configuration.</param>
        /// <param name="complete">
        /// Is this a complete tournament, i.e., do thinkers play against
        /// opponents home and away (two games)?
        /// </param>
        public Session(IEnumerable<IThinkerPrototype> thinkerPrototypes,
            ISessionConfig sessionConfig, bool complete = false)
        {
            // Create a list of thinkers from the given enumerable
            IList<IThinkerPrototype> thinkerProtList =
                new List<IThinkerPrototype>(thinkerPrototypes);

            // Number of thinkers
            int numThinkers = thinkerProtList?.Count ?? 0;

            // Check if there are enough thinkers
            if (numThinkers < 2)
                throw new InvalidOperationException(
                    "A tournament must have at least two thinkers, "
                    + $"but only {numThinkers} where specified");

            // Keep note of points per win, draw and loss
            this.pointsPerWin = sessionConfig.PointsPerWin;
            this.pointsPerLoss = sessionConfig.PointsPerLoss;
            this.pointsPerDraw = sessionConfig.PointsPerDraw;

            // Initialize the table of thinker points
            thinkerPoints = new Dictionary<string, int>(numThinkers);

            // Initialize the list of matches
            matches = new List<Match>();

            // Initialize the list of results
            results = new Dictionary<Match, Winner>();

            // We need an even number of players to set up the
            // matches, so add a fake one if necessary
            if (thinkerProtList.Count % 2 != 0)
                thinkerProtList.Add(new DummyThinkerPrototype());

            // Setup matches using the round-robin method
            // https://en.wikipedia.org/wiki/Round-robin_tournament
            for (int i = 1; i < thinkerProtList.Count; i++)
            {
                // This will be the thinker to swap position after each round
                IThinkerPrototype thinkerToSwapPosition;

                // Set up matches for current round i
                for (int j = 0; j < thinkerProtList.Count / 2; j++)
                {
                    // This is match j for current round i
                    Match match = new Match(
                        thinkerProtList[j],
                        thinkerProtList[thinkerProtList.Count - 1 - j]);
                    // Only add match to match list if it's not a dummy
                    // match
                    if (!(match.thinker1 is DummyThinkerPrototype
                        || match.thinker2 is DummyThinkerPrototype))
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
                    thinkerProtList[thinkerProtList.Count - 1];
                thinkerProtList.RemoveAt(thinkerProtList.Count - 1);
                thinkerProtList.Insert(1, thinkerToSwapPosition);
            }

            // Initialize the match enumerator
            matchEnumerator = GetEnumerator();
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
        public IEnumerable<KeyValuePair<string, int>> GetStandings()
        {
            // Create an array to place thinkers and their points
            KeyValuePair<string, int>[] standings =
                new KeyValuePair<string, int>[thinkerPoints.Count];

            // Populate the array with thinkers and their points
            thinkerPoints.CopyTo(standings, 0);

            // Sort the array in descending order according to thinker points
            Array.Sort(standings, (a, b) => b.Value - a.Value);

            // Return current standings/classification
            return standings;
        }

        /// <summary>
        /// Return all scheduled matches, completed or otherwise.
        /// </summary>
        /// <returns>All scheduled matches.</returns>
        public IEnumerator<Match> GetEnumerator() => matches.GetEnumerator();

        /// <summary>
        /// Explicit implementation of the
        /// <see cref="System.Collections.IEnumerable"/> interface.
        /// </summary>
        /// <returns>An enumerator with all scheduled matches.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Get next match.
        /// </summary>
        /// <returns>The next match to play.</returns>
        public bool NextMatch(out Match match)
        {
            if (!matchEnumerator.MoveNext())
            {
                match = null;
                matchEnumerator.Dispose();
                return false;
            }
            else
            {
                match = matchEnumerator.Current;
                return true;
            }
        }

        /// <summary>
        /// Set the result of the last match.
        /// </summary>
        /// <param name="result">Result of the given match.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the last match already has a result or the
        /// <paramref name="result"/> is invalid.
        /// </exception>
        public void SetResult(Winner result)
        {
            // Get current match
            Match match = matchEnumerator.Current;

            // If table already contains a result for this match,
            // throw exception
            if (results.ContainsKey(match))
            {
                throw new InvalidOperationException(
                    $"Match '{match}' already has a result");
            }

            // Add to results table
            results.Add(match, result);

            // If these thinkers are not yet in the points table, add them
            if (!thinkerPoints.ContainsKey(match.thinker1.ThinkerName))
                thinkerPoints.Add(match.thinker1.ThinkerName, 0);
            if (!thinkerPoints.ContainsKey(match.thinker2.ThinkerName))
                thinkerPoints.Add(match.thinker2.ThinkerName, 0);

            // Update thinker points
            switch (result)
            {
                // White won
                case Winner.White:
                    thinkerPoints[match.thinker1.ThinkerName] += pointsPerWin;
                    thinkerPoints[match.thinker2.ThinkerName] += pointsPerLoss;
                    break;
                // Red won
                case Winner.Red:
                    thinkerPoints[match.thinker2.ThinkerName] += pointsPerWin;
                    thinkerPoints[match.thinker1.ThinkerName] += pointsPerLoss;
                    break;
                // Game ended in a draw
                case Winner.Draw:
                    thinkerPoints[match.thinker1.ThinkerName] += pointsPerDraw;
                    thinkerPoints[match.thinker2.ThinkerName] += pointsPerDraw;
                    break;
                // Invalid situation
                default:
                    throw new InvalidOperationException(
                        "Invalid end of match result");
            }
        }
    }
}
