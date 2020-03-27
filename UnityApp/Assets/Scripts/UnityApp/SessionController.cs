/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.UnityApp.SessionController class.
///
/// @author Nuno Fachada
/// @date 2019, 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace ColorShapeLinks.UnityApp
{
    /// <summary>
    /// Script which controls ColorShapeLinks sessions, which can include one
    /// or more matches.
    /// </summary>
    /// <remarks>
    /// Based on the MVC design pattern, composed in this case by the following
    /// classes:
    /// * *Model* - A <see cref="ColorShapeLinks.Common.Session.Session"/>
    /// instance, containing all the matches, results and standings.
    /// * *View* - <see cref="SessionView"/>.
    /// * *Controller* - This class.
    /// </remarks>
    public class SessionController : MonoBehaviour,
        IMatchDataProvider, IMatchConfig, IMatchViewConfig,
        IUnitySessionDataProvider, ISessionConfig
    {

        // ///////////////////////////////////////////////// //
        // Match properties configurable in the Unity editor //
        // ///////////////////////////////////////////////// //
        [Header("Match properties")]

        /// <summary>Number of rows in the game board.</summary>
        [SerializeField] private int rows = 6;

        /// <summary>Number of columns in the game board.</summary>
        [SerializeField] private int cols = 7;

        /// <summary>How many pieces in a row are required to win.</summary>
        [SerializeField] private int winSequence = 4;

        /// <summary>Initial number of round pieces per player.</summary>
        [SerializeField] private int roundPiecesPerPlayer = 10;

        /// <summary>Initial number of square pieces per player.</summary>
        [SerializeField] private int squarePiecesPerPlayer = 11;

        /// <summary>Last move animation length in seconds.</summary>
        [SerializeField] private float lastMoveAnimLength = 1f;

        // ////////////////////////////////////////////// //
        // AI properties configurable in the Unity editor //
        // ////////////////////////////////////////////// //
        [Header("AI properties")]

        /// <summary>Maximum time in seconds that AI can take to play.</summary>
        [Tooltip("Maximum real time in seconds that AI can take to play")]
        [SerializeField] private float aITimeLimit = 0.5f;

        /// <summary>Minimum apparent AI move time.</summary>
        /// <remarks>
        /// Even if the AI plays immediately, this time (in seconds) gives
        /// the illusion that the AI took some minimum time to play.
        /// </remarks>
        [Tooltip("Even if the AI plays immediately, this time (in seconds) "
            + "gives the illusion that the AI took some minimum time to play")]
        [SerializeField] private float minAIGameMoveTime = 0.25f;

        // ////////////////////////////////////////////////////// //
        // Tournament properties configurable in the Unity editor //
        // ////////////////////////////////////////////////////// //
        [Header("Tournament properties")]

        /// <summary>Tournament points per win.</summary>
        [SerializeField] private int pointsPerWin = 3;

        /// <summary>Tournament points per draw.</summary>
        [SerializeField] private int pointsPerDraw = 1;

        /// <summary>Tournament points per loss.</summary>
        [SerializeField] private int pointsPerLoss = 0;

        /// <summary>Press a button before tournament match starts?</summary>
        [SerializeField] private bool pressButtonBeforeMatch = false;

        /// <summary>Press a button after tournament match ends?</summary>
        [SerializeField] private bool pressButtonAfterMatch = false;

        /// <summary>
        /// Duration of a screen if no button needs to be pressed in order
        /// to continue.
        /// </summary>
        [SerializeField] private float unblockedScreenDuration = 1.5f;

        // The session state of the session state machine
        private SessionState state;

        // Reference to the session view
        private SessionView view;

        // Reference to the match prefab (match controller + view + board model)
        private GameObject matchPrefab = null;

        // Reference to the current match instance, created from the match
        // prefab
        private GameObject matchInstance = null;

        // Reference to the match controller associated with the current match
        // instance
        private MatchController matchController = null;

        // Reference to the current match (thinker 1 + thinker 2)
        private Match currentMatch;

        // Reference to the game board in the current match instance
        private Board currentBoard;

        // The session instance manages the matches to be played and the
        // tournament standings
        // It's the model in this MVC implementation
        private Session session;

        // Thinkers in current match
        private IThinker[] currentThinkers;

        // Variables which define how several session UI screens will behave
        private bool uiWhoPlaysFirst;
        private bool uiBlockStartNextMatch;
        private bool uiBlockShowResult;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Get all AIs and put them in a list
            List<AIPlayer> allAIs = new List<AIPlayer>();
            GetComponents(allAIs);

            // Get the thinkers associated with the active AIs
            IList<ThinkerPrototype> activeThinkers = allAIs
                .Where(ai => ai.IsActive)
                .Select(ai => ai.ThinkerPrototype)
                .ToList();

            // A human thinker prototype, in case there aren't enough
            // thinkers
            IThinkerPrototype humanPrototype = new HumanThinkerPrototype();

            // Instantiate the current thinkers array
            currentThinkers = new IThinker[2];

            // Load the match prefab
            matchPrefab = Resources.Load<GameObject>("Prefabs/Match");

            // Get reference to the UI (session view)
            view = GetComponent<SessionView>();

            // Set the session state to Begin
            state = SessionState.Begin;

            // Setup session depending on how many AIs will play
            if (activeThinkers.Count == 0)
            {
                // A game between human players
                uiWhoPlaysFirst = false;
                uiBlockStartNextMatch = true;
                uiBlockShowResult = true;
                session = new Session(
                    new IThinkerPrototype[] {
                        humanPrototype,
                        humanPrototype },
                    this);
            }
            else if (activeThinkers.Count == 1)
            {
                // A game between a human and an AI
                uiWhoPlaysFirst = true;
                uiBlockStartNextMatch = true;
                uiBlockShowResult = true;
                session = new Session(
                    new IThinkerPrototype[] {
                        humanPrototype,
                        activeThinkers[0] },
                    this);
            }
            else if (activeThinkers.Count == 2)
            {
                // A game between two AIs, ask who plays first
                uiWhoPlaysFirst = true;
                uiBlockStartNextMatch = true;
                uiBlockShowResult = true;
                session = new Session(
                    new IThinkerPrototype[] {
                        activeThinkers[0],
                        activeThinkers[1] },
                    this);
            }
            else
            {
                // Multiple AIs, run a complete tournament
                uiWhoPlaysFirst = false;
                uiBlockStartNextMatch = pressButtonBeforeMatch;
                uiBlockShowResult = pressButtonAfterMatch;
                session = new Session(activeThinkers, this, true);
            }

            // Get the next match
            if (!session.NextMatch(out currentMatch))
            {
                throw new InvalidOperationException(
                    "Session does not contain matches");
            }
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            // Register listener methods to UI events
            view.PreMatch += PreMatchCallback;
            view.SwapPlayers += SwapPlayersCallback;
            view.StartNextMatch += StartNextMatchCallback;
            view.MatchClear += DestroyAndIterateMatchCallback;
            view.EndSession += EndSessionCallback;
            Application.logMessageReceived += HandleFatalErrors;
        }

        // This function is called when the behaviour becomes disabled
        private void OnDisable()
        {
            // Unregister listener methods from UI events
            view.PreMatch -= PreMatchCallback;
            view.SwapPlayers -= SwapPlayersCallback;
            view.StartNextMatch -= StartNextMatchCallback;
            view.MatchClear -= DestroyAndIterateMatchCallback;
            view.EndSession -= EndSessionCallback;
            Application.logMessageReceived -= HandleFatalErrors;
        }

        // Terminate app if a fatal error or exception occurs
        private void HandleFatalErrors(
            string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        // Change the session state to PreMatch
        private void PreMatchCallback()
        {
            state = SessionState.PreMatch;
        }

        // Swap current match players
        private void SwapPlayersCallback()
        {
            currentMatch = currentMatch.Swapped;
        }

        // Start next match, changing session state to InMatch
        private void StartNextMatchCallback()
        {
            // Instantiate a board for the next match
            currentBoard = new Board(rows, cols, winSequence,
                roundPiecesPerPlayer, squarePiecesPerPlayer);

            // Instantiate thinkers for next match
            currentThinkers[(int)PColor.White] =
                currentMatch.thinkerWhite.Create();
            currentThinkers[(int)PColor.Red] =
                currentMatch.thinkerRed.Create();

            // Wire-up Debug.Log as thinker listener
            currentThinkers[(int)PColor.White].ThinkingInfo += Debug.Log;
            currentThinkers[(int)PColor.Red].ThinkingInfo += Debug.Log;

            // Instantiate the next match
            matchInstance = Instantiate(matchPrefab, transform);
            matchInstance.name = currentMatch.ToString();

            // Get a reference to the match controller of the next match
            matchController = matchInstance.GetComponent<MatchController>();

            // Add a listener for the match over event
            matchController.MatchOver.AddListener(EndCurrentMatchCallback);

            // Set session state to InMatch
            state = SessionState.InMatch;
        }

        // End current match an change session state to PostMatch
        private void EndCurrentMatchCallback()
        {
            // Keep result of current match
            session.SetResult(matchController.Result);

            // Set session state to PostMatch
            state = SessionState.PostMatch;
        }

        // Destroy old match instance and check if there are more matches to
        // play. If so, get next match and update state to PreMatch. Otherwise,
        // dispose of the match enumerator and update session state to End.
        private void DestroyAndIterateMatchCallback()
        {
            // Destroy old match instance
            Destroy(matchInstance);

            // Are there more matches to play?
            if (session.NextMatch(out currentMatch))
            {
                // If so, get next match and set session state to PreMatch
                state = SessionState.PreMatch;
            }
            else
            {
                // Otherwise, dispose of the match enumerator and set session
                // state to End
                state = SessionState.End;
            }
        }

        // Terminate the session
        private void EndSessionCallback()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        // //////////////////////////////////// //
        // Implementation of IMatchDataProvider //
        // //////////////////////////////////// //

        /// @copydoc Common.Session.IMatchDataProvider.Board
        /// <seealso cref="IMatchDataProvider.Board"/>
        public Board Board => currentBoard;

        /// @copydoc Common.Session.IMatchDataProvider.CurrentThinker
        /// <seealso cref="IMatchDataProvider.CurrentThinker"/>
        public IThinker CurrentThinker =>
            currentThinkers[(int)currentBoard.Turn];

        /// @copydoc Common.Session.IMatchDataProvider.GetThinker
        /// <seealso cref="IMatchDataProvider.GetThinker(PColor)"/>
        public IThinker GetThinker(PColor thinkerColor) =>
            currentThinkers[(int)thinkerColor];

        // ///////////////////////////// //
        // Implementation of IMatchConfig //
        // ///////////////////////////// //

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.Rows
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.Rows"/>
        public int Rows => rows;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.Cols
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.Cols"/>
        public int Cols => cols;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.WinSequence
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.WinSequence"/>
        public int WinSequence => winSequence;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.RoundPiecesPerPlayer
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.RoundPiecesPerPlayer"/>
        public int RoundPiecesPerPlayer => roundPiecesPerPlayer;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.SquarePiecesPerPlayer
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.SquarePiecesPerPlayer"/>
        public int SquarePiecesPerPlayer => squarePiecesPerPlayer;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitMillis
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitMillis"/>
        public int TimeLimitMillis => (int)(aITimeLimit * 1000);

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitSeconds
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.TimeLimitSeconds"/>
        public float TimeLimitSeconds => aITimeLimit;

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeMillis
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeMillis"/>
        public int MinMoveTimeMillis => (int)(minAIGameMoveTime * 1000);

        /// @copydoc ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeSeconds
        /// <seealso cref="ColorShapeLinks.Common.Session.IMatchConfig.MinMoveTimeSeconds"/>
        public float MinMoveTimeSeconds => minAIGameMoveTime;

        // ////////////////////////////////// //
        // Implementation of IMatchViewConfig //
        // ////////////////////////////////// //

        /// @copydoc IMatchViewConfig.LastMoveAnimLength
        /// <seealso cref="IMatchViewConfig.LastMoveAnimLength"/>
        public float LastMoveAnimLength => lastMoveAnimLength;

        // /////////////////////////////////////////// //
        // Implementation of IUnitySessionDataProvider //
        // /////////////////////////////////////////// //

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.State
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.State"/>
        public SessionState State => state;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.SessionConfig
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.SessionConfig"/>
        public ISessionConfig SessionConfig => this;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.MatchConfig
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.MatchConfig"/>
        public IMatchConfig MatchConfig => this;

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
        public Winner LastMatchResult => matchController?.Result ?? Winner.None;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionDataProvider.WinnerString
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionDataProvider.WinnerString"/>
        public string WinnerString => matchController?.WinnerString;

        /// @copydoc IUnitySessionDataProvider.WhoPlaysFirst
        /// <seealso cref="IUnitySessionDataProvider.WhoPlaysFirst"/>
        public bool WhoPlaysFirst => uiWhoPlaysFirst;

        /// @copydoc IUnitySessionDataProvider.BlockStartNextMatch
        /// <seealso cref="IUnitySessionDataProvider.BlockStartNextMatch"/>
        public bool BlockStartNextMatch => uiBlockStartNextMatch;

        /// @copydoc IUnitySessionDataProvider.BlockShowResult
        /// <seealso cref="IUnitySessionDataProvider.BlockShowResult"/>
        public bool BlockShowResult => uiBlockShowResult;

        /// @copydoc IUnitySessionDataProvider.UnblockedScreenDuration
        /// <seealso cref="IUnitySessionDataProvider.UnblockedScreenDuration"/>
        public float UnblockedScreenDuration => unblockedScreenDuration;

        // //////////////////////////////// //
        // Implementation of ISessionConfig //
        // //////////////////////////////// //

        /// @copydoc ColorShapeLinks.Common.Session.ISessionConfig.PointsPerWin
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionConfig.PointsPerWin"/>
        public int PointsPerWin => pointsPerWin;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionConfig.PointsPerLoss
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionConfig.PointsPerLoss"/>
        public int PointsPerLoss => pointsPerLoss;

        /// @copydoc ColorShapeLinks.Common.Session.ISessionConfig.PointsPerLoss
        /// <seealso cref="ColorShapeLinks.Common.Session.ISessionConfig.PointsPerLoss"/>
        public int PointsPerDraw => pointsPerDraw;
    }
}
