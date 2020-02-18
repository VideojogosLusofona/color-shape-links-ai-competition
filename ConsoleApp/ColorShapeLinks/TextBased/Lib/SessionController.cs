/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.App.SessionController class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    public class SessionController : IMatchDataProvider, ISessionSubject
    {
        private IMatchConfig matchConfig;
        private ISessionConfig sessionConfig;
        private IEnumerable<IThinkerPrototype> thinkerPrototypes;
        private IEnumerable<IThinkerListener> thinkerListeners;
        private IEnumerable<IMatchListener> matchListeners;
        private IList<IThinker> currentThinkers;
        private Board board;

        public SessionController(
            IMatchConfig matchConfig,
            ISessionConfig sessionConfig,
            IEnumerable<IThinkerPrototype> thinkerPrototypes,
            IEnumerable<IThinkerListener> thinkerListeners,
            IEnumerable<IMatchListener> matchListeners,
            IEnumerable<ISessionListener> sessionListeners)
        {
            // Keep parameters
            this.matchConfig = matchConfig;
            this.sessionConfig = sessionConfig;
            this.thinkerPrototypes = thinkerPrototypes;
            this.thinkerListeners = thinkerListeners;
            this.matchListeners = matchListeners;

            // Register session listeners
            foreach (ISessionListener listener in sessionListeners)
                listener.ListenTo(this);

            // Instantiate list of current thinkers
            currentThinkers = new IThinker[2];
        }

        public ExitStatus Run(bool complete)
        {
            ExitStatus exitStatus = ExitStatus.Session;
            Session session = new Session(
                thinkerPrototypes, sessionConfig, complete);

            foreach (Match match in session)
            {
                board = new Board(matchConfig.Rows, matchConfig.Cols,
                    matchConfig.WinSequence, matchConfig.RoundPiecesPerPlayer,
                    matchConfig.SquarePiecesPerPlayer);

                currentThinkers[(int)PColor.White] = match.thinker1.Create();
                currentThinkers[(int)PColor.Red] = match.thinker2.Create();

                foreach (IThinkerListener listener in thinkerListeners)
                {
                    foreach (IThinker thinker in currentThinkers)
                    {
                        if (thinker is AbstractThinker)
                        {
                            listener.ListenTo((AbstractThinker)thinker);
                        }
                    }
                }

                MatchController mc = new MatchController(matchConfig, this);

                foreach (IMatchListener listener in matchListeners)
                {
                    listener.ListenTo(mc);
                }

                exitStatus = mc
                    .Run()
                    .ToExitStatus();
            }

            return complete ? ExitStatus.Session : exitStatus;
        }

        // //////////////////////////////////// //
        // Implementation of IMatchDataProvider //
        // //////////////////////////////////// //

        /// <summary>The game board.</summary>
        /// <value>The game board.</value>
        public Board Board => board;

        /// <summary>The current thinker.</summary>
        /// <value>The current thinker.</value>
        public IThinker CurrentThinker => currentThinkers[(int)board.Turn];

        /// <summary>Get thinker of the specified color.</summary>
        /// <param name="thinkerColor">Color of the thinker to get.</param>
        /// <returns>Thinker of the specified color.</returns>
        public IThinker GetThinker(PColor thinkerColor) =>
            currentThinkers[(int)thinkerColor];
    }
}
