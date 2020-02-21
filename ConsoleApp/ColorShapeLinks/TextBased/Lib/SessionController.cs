/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.TextBased.Lib.SessionController class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.TextBased.Lib
{
    /// <summary>
    /// Controls a session of ColorShapeLinks matches.
    /// </summary>
    public class SessionController :
        IMatchDataProvider, ISessionSubject, ISessionDataProvider
    {
        // The session controlled by this controller (i.e., the model)
        private Session session;

        // Match configuration
        private IMatchConfig matchConfig;

        // Session configuration
        private ISessionConfig sessionConfig;

        // List of thinker prototypes for thinkers participating in
        // this session
        private IEnumerable<IThinkerPrototype> thinkerPrototypes;

        // List of thinker listeners
        private IEnumerable<IThinkerListener> thinkerListeners;

        // List of match listeners
        private IEnumerable<IMatchListener> matchListeners;

        // List of thinkers in the current match
        private IList<IThinker> currentThinkers;

        // Reference to the current match
        private Match currentMatch;

        // Reference to the game board in the current match
        private Board board;

        // Result of last match
        private Winner lastMatchResult = Winner.None;

        /// <summary>
        /// Create a new session controller.
        /// </summary>
        /// <param name="matchConfig">Match configuration.</param>
        /// <param name="sessionConfig">Session configuration.</param>
        /// <param name="thinkerPrototypes">
        /// List of thinker prototypes for thinkers participating in
        /// this session.
        /// </param>
        /// <param name="thinkerListeners">List of thinker listeners.</param>
        /// <param name="matchListeners">List of match listeners.</param>
        /// <param name="sessionListeners">List of session listeners.</param>
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

        /// <summary>
        /// Run a session of ColorShapeLinks matches.
        /// </summary>
        /// <param name="complete">
        /// Is the session complete, i.e., should thinkers compete against each
        /// other two times, home and away?
        /// </param>
        /// <returns>
        /// An exit status according to what is defined in
        /// <see cref="ExitStatus"/>.
        /// </returns>
        public ExitStatus Run(bool complete)
        {
            // The exit status
            ExitStatus exitStatus = default;

            // Create a new session (the model in MVC)
            session = new Session(thinkerPrototypes, sessionConfig, complete);

            // Notify listeners that session is about to start
            BeforeSession?.Invoke(this);

            // Play each match defined by the session
            while (session.NextMatch(out currentMatch))
            {
                // Create a new board for the current match
                board = new Board(matchConfig.Rows, matchConfig.Cols,
                    matchConfig.WinSequence, matchConfig.RoundPiecesPerPlayer,
                    matchConfig.SquarePiecesPerPlayer);

                // Instantiate thinkers for the current match
                currentThinkers[(int)PColor.White] =
                    currentMatch.thinkerWhite.Create();
                currentThinkers[(int)PColor.Red] =
                    currentMatch.thinkerRed.Create();

                // Add registered listeners to the thinker instances
                foreach (IThinkerListener listener in thinkerListeners)
                {
                    foreach (IThinker thinker in currentThinkers)
                    {
                        listener.ListenTo(thinker);
                    }
                }

                // Create a match controller for the current match
                MatchController mc = new MatchController(matchConfig, this);

                // Register match listeners with the match controller
                foreach (IMatchListener listener in matchListeners)
                {
                    listener.ListenTo(mc);
                }

                // Notify listeners that a match is about to start
                BeforeMatch?.Invoke(currentMatch);

                // Ask the controller to run the match and keep the result
                lastMatchResult = mc.Run();

                // Update variables and properties related to the match result
                exitStatus = lastMatchResult.ToExitStatus();

                // Update the winner string
                if (lastMatchResult != Winner.Draw)
                {
                    PColor winnerColor = lastMatchResult.ToPColor();
                    WinnerString = winnerColor.FormatName(
                        currentThinkers[(int)winnerColor].ToString());
                }
                else
                {
                    WinnerString = lastMatchResult.ToString();
                }

                // Notify result to session
                session.SetResult(lastMatchResult);

                // Notify listeners that a match is over
                AfterMatch?.Invoke(currentMatch, this);
            }

            // Notify listeners that sessions is about to end
            AfterSession?.Invoke(this);

            // The exit status will either be for a complete session of
            // associated with the match result
            return complete ? ExitStatus.Session : exitStatus;
        }

        // //////////////////////////////////// //
        // Implementation of IMatchDataProvider //
        // //////////////////////////////////// //

        /// @copydoc ColorShapeLinks.Common.Session.IMatchDataProvider.Board
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchDataProvider.Board"/>
        public Board Board => board;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchDataProvider.CurrentThinker
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchDataProvider.CurrentThinker"/>
        public IThinker CurrentThinker => currentThinkers[(int)board.Turn];

        /// @copydoc ColorShapeLinks.Common.Session.IMatchDataProvider.GetThinker
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchDataProvider.GetThinker"/>
        public IThinker GetThinker(PColor thinkerColor) =>
            currentThinkers[(int)thinkerColor];

        // ///////////////////////////////// //
        // Implementation of ISessionSubject //
        // ///////////////////////////////// //

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionSubject.BeforeSession
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionSubject.BeforeSession"/>
        public event Action<ISessionDataProvider> BeforeSession;

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionSubject.AfterSession
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionSubject.AfterSession"/>
        public event Action<ISessionDataProvider> AfterSession;

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionSubject.BeforeMatch
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionSubject.BeforeMatch"/>
        public event Action<Match> BeforeMatch;

        /// @copydoc ColorShapeLinks.TextBased.Lib.ISessionSubject.AfterMatch
        /// <seealso cref="ColorShapeLinks.TextBased.Lib.ISessionSubject.AfterMatch"/>
        public event Action<Match, ISessionDataProvider> AfterMatch;

        // ////////////////////////////////////// //
        // Implementation of ISessionDataProvider //
        // ////////////////////////////////////// //

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.State
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.State"/>
        /// <exception cref="System.NotImplementedException">
        /// Always thrown, since this implementation doesn't track an explicit
        /// session state.
        /// </exception>
        public SessionState State =>
            throw new NotImplementedException("Session state not implemented");

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.SessionConfig
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.SessionConfig"/>
        public ISessionConfig SessionConfig => sessionConfig;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.MatchConfig
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.MatchConfig"/>
        public IMatchConfig MatchConfig => matchConfig;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.CurrentMatch
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.CurrentMatch"/>
        public Match CurrentMatch => currentMatch;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.Matches
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.Matches"/>
        public IEnumerable<Match> Matches => session;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.Results
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.Results"/>
        public IEnumerable<KeyValuePair<Match, Winner>> Results =>
            session.GetResults();

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.Standings
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.Standings"/>
        public IEnumerable<KeyValuePair<string, int>> Standings =>
            session.GetStandings();

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.LastMatchResult
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.LastMatchResult"/>
        public Winner LastMatchResult => lastMatchResult;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.WinnerString
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.WinnerString"/>
        public string WinnerString { get; private set; }
    }
}
