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

namespace ColorShapeLinks.Common.Tournament
{
    public class Tournament : IEnumerable<Match>
    {
        // Internal auxiliary class used for match making
        private struct DummyThinker : IThinker
        {
            public FutureMove Think(Board board, CancellationToken ct)
                => throw new InvalidOperationException(
                    "This is just a dummy thinker");
        }

        private IList<Match> matches;

        private IDictionary<Match, Winner> results;

        // Current points for each thinker
        private IDictionary<IThinker, int> thinkerPoints;

        private int pointsPerWin, pointsPerLoss, pointsPerDraw;

        public Tournament(IEnumerable<IThinker> thinkers,
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

        public IEnumerable<KeyValuePair<Match, Winner>> GetResults()
        {
            foreach (Match m in matches)
            {
                if (results.ContainsKey(m))
                    yield return new KeyValuePair<Match, Winner>(m, results[m]);
            }
        }

        public IEnumerable<KeyValuePair<IThinker, int>> GetStandings()
        {
            KeyValuePair<IThinker, int>[] standings =
                new KeyValuePair<IThinker, int>[thinkerPoints.Count];

            thinkerPoints.CopyTo(standings, 0);

            Array.Sort(standings, (a, b) => b.Value - a.Value);

            foreach (KeyValuePair<IThinker, int> kvp in standings)
                yield return kvp;
        }

        public IEnumerator<Match> GetEnumerator()
        {
            foreach (Match m in matches)
                yield return m;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void SetResult(Match match, Winner result)
        {
            results.Add(match, result);

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
