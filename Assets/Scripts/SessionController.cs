/// @file
/// @brief This file contains the ::SessionController class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which controls *ColorShapeLinks* sessions, which can include one or
/// more matches.
/// </summary>
/// <remarks>
/// Based on the MVC design pattern, composed in this case by the following
/// classes:
/// * *Model* - A list of <see cref="Match"/> instances can be considered the
/// model, although there isn't a well defined model in this case.
/// * *View* - <see cref="SessionView"/>.
/// * *Controller* - This class.
/// </remarks>
public class SessionController
    : MonoBehaviour, IMatchDataProvider, ISessionDataProvider
{

    // Internal auxiliary struct used for match making
    private struct DummyPlayer : IPlayer
    {
        public bool IsHuman => false;
        public string PlayerName => "Dummy";
        public IThinker Thinker => null;
    }

    // ///////////////////////////////////////////////// //
    // Match properties configurable in the Unity editor //
    // ///////////////////////////////////////////////// //
    [Header("Match properties")]

    /// <summary>Number of rows in the game board.</summary>
    [SerializeField] private int rows = 7;

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
    [Tooltip("Even if the AI plays immediately, this time (in seconds) gives "
        + "the illusion that the AI took some minimum time to play")]
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

    // Reference to the current match instance, created from the match prefab
    private GameObject matchInstance = null;

    // Reference to the match controller associated with the current match
    // instance
    private MatchController matchController = null;

    // Reference to the current match (player 1 + player 2)
    private Match currentMatch;

    // Reference to the game board in the current match instance
    private Board currentBoard;

    // Dictionary of all matches played and to be player, each match being
    // associated with a result
    private IDictionary<Match, Winner> allMatches;

    // Current standings / classification (important for tournament mode only)
    private IDictionary<IPlayer, int> standings;

    // Match enumerator, for going through each match one by one
    private IEnumerator<Match> matchEnumerator;

    // List of active AIs
    private IList<IPlayer> activeAIs = null;

    // Reference to a human player, in case there aren't enough AIs to create a
    // match
    private IPlayer humanPlayer;

    // Variables which define how several session UI screens will behave
    private bool uiWhoPlaysFirst;
    private bool uiBlockStartNextMatch;
    private bool uiBlockShowResult;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get all active AIs and put them in a list
        List<IPlayer> allAIs = new List<IPlayer>();
        GetComponents(allAIs);
        activeAIs = allAIs.FindAll(ai => (ai as AIPlayer).IsActive);

        // Load the match prefab
        matchPrefab = Resources.Load<GameObject>("Prefabs/Match");

        // Get reference to the UI (session view)
        view = GetComponent<SessionView>();

        // Set the session state to Begin
        state = SessionState.Begin;

        // Instantiate a human player if there are not enough AIs to do a match
        if (activeAIs.Count < 2)
            humanPlayer = new HumanPlayer();

        // Instantiate the matches table
        allMatches = new SortedList<Match, Winner>();

        // Instantiate the standings table and populate it with all AIs with
        // zero points
        standings = new Dictionary<IPlayer, int>();
        foreach (IPlayer ai in activeAIs) standings.Add(ai, 0);

        // Setup session depending on how many AIs will play
        if (activeAIs.Count == 0)
        {
            // A game between human players
            uiWhoPlaysFirst = false;
            uiBlockStartNextMatch = true;
            uiBlockShowResult = true;
            allMatches.Add(new Match(humanPlayer, humanPlayer), Winner.None);
        }
        else if (activeAIs.Count == 1)
        {
            // A game between a human and an AI
            uiWhoPlaysFirst = true;
            uiBlockStartNextMatch = true;
            uiBlockShowResult = true;
            allMatches.Add(new Match(humanPlayer, activeAIs[0]), Winner.None);
        }
        else if (activeAIs.Count == 2)
        {
            // A game between two AIs, ask who plays first
            uiWhoPlaysFirst = true;
            uiBlockStartNextMatch = true;
            uiBlockShowResult = true;
            allMatches.Add(new Match(activeAIs[0], activeAIs[1]), Winner.None);
        }
        else
        {
            // Multiple AIs, run a tournament
            uiWhoPlaysFirst = false;
            uiBlockStartNextMatch = pressButtonBeforeMatch;
            uiBlockShowResult = pressButtonAfterMatch;

            // In this mode we need an even number of players to set up the
            // matches, so add a fake one if necessary
            if (activeAIs.Count % 2 != 0) activeAIs.Add(new DummyPlayer());

            // Setup matches using the round-robin method
            // https://en.wikipedia.org/wiki/Round-robin_tournament
            for (int i = 1; i < activeAIs.Count; i++)
            {
                // This will be the AI to swap position after each round
                IPlayer aiToSwapPosition;

                // Set up matches for current round i
                for (int j = 0; j < activeAIs.Count / 2; j++)
                {
                    // This is match j for current round i
                    Match match = new Match(
                        activeAIs[j], activeAIs[activeAIs.Count - 1 - j]);
                    // Only add match to match list if it's not a dummy match
                    if (!(match.player1 is DummyPlayer
                        || match.player2 is DummyPlayer))
                    {
                        // Each match is in practice two matches, so both AIs
                        // can have a match where they are the first to play
                        allMatches.Add(match, Winner.None);
                        allMatches.Add(match.Swapped, Winner.None);
                    }
                }
                // Swap AI positions for next round
                aiToSwapPosition = activeAIs[activeAIs.Count - 1];
                activeAIs.RemoveAt(activeAIs.Count - 1);
                activeAIs.Insert(1, aiToSwapPosition);
            }
        }

        // Get the match enumerator and initialize it
        matchEnumerator = allMatches.Keys.ToList().GetEnumerator();
        matchEnumerator.MoveNext();
        currentMatch = matchEnumerator.Current;
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

        // Instantiate the next match
        matchInstance = Instantiate(matchPrefab, transform);
        matchInstance.name = $"Match{PlayerWhite}VS{PlayerRed}";

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
        allMatches[currentMatch] = matchController.Result;

        // If we are in tournament mode, update standings / classification
        if (activeAIs.Count > 2)
        {
            switch (matchController.Result)
            {
                // White won
                case Winner.White:
                    standings[currentMatch.player1] += pointsPerWin;
                    standings[currentMatch.player2] += pointsPerLoss;
                    break;
                // Red won
                case Winner.Red:
                    standings[currentMatch.player2] += pointsPerWin;
                    standings[currentMatch.player1] += pointsPerLoss;
                    break;
                // Game ended in a draw
                case Winner.Draw:
                    standings[currentMatch.player1] += pointsPerDraw;
                    standings[currentMatch.player2] += pointsPerDraw;
                    break;
                // Invalid situation
                default:
                    throw new InvalidOperationException(
                        "Invalid end of match result");
            }
        }

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
        if (matchEnumerator.MoveNext())
        {
            // If so, get next match and set session state to PreMatch
            currentMatch = matchEnumerator.Current;
            state = SessionState.PreMatch;
        }
        else
        {
            // Otherwise, dispose of the match enumerator and set session state
            // to End
            matchEnumerator.Dispose();
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

    /// @copydoc IMatchDataProvider.Board
    /// <seealso cref="IMatchDataProvider.Board"/>
    public Board Board => currentBoard;

    /// @copydoc IMatchDataProvider.CurrentPlayer
    /// <seealso cref="IMatchDataProvider.CurrentPlayer"/>
    public IPlayer CurrentPlayer => currentMatch[currentBoard.Turn];

    /// @copydoc IMatchDataProvider.AITimeLimit
    /// <seealso cref="IMatchDataProvider.AITimeLimit"/>
    public float AITimeLimit => aITimeLimit;

    /// @copydoc IMatchDataProvider.MinAIGameMoveTime
    /// <seealso cref="IMatchDataProvider.MinAIGameMoveTime"/>
    public float MinAIGameMoveTime => minAIGameMoveTime;

    /// @copydoc IMatchDataProvider.LastMoveAnimLength
    /// <seealso cref="IMatchDataProvider.LastMoveAnimLength"/>
    public float LastMoveAnimLength => lastMoveAnimLength;

    /// @copydoc IMatchDataProvider.GetPlayer
    /// <seealso cref="IMatchDataProvider.GetPlayer(PColor)"/>
    public IPlayer GetPlayer(PColor player) => currentMatch[player];

    // ////////////////////////////////////// //
    // Implementation of ISessionDataProvider //
    // ////////////////////////////////////// //

    /// @copydoc ISessionDataProvider.State
    /// <seealso cref="ISessionDataProvider.State"/>
    public SessionState State => state;

    /// @copydoc ISessionDataProvider.PlayerWhite
    /// <seealso cref="ISessionDataProvider.PlayerWhite"/>
    public string PlayerWhite => currentMatch[PColor.White].PlayerName;

    /// @copydoc ISessionDataProvider.PlayerRed
    /// <seealso cref="ISessionDataProvider.PlayerRed"/>
    public string PlayerRed => currentMatch[PColor.Red].PlayerName;

    /// @copydoc ISessionDataProvider.Matches
    /// <seealso cref="ISessionDataProvider.Matches"/>
    public IEnumerable<Match> Matches => allMatches.Keys;

    /// @copydoc ISessionDataProvider.Results
    /// <seealso cref="ISessionDataProvider.Results"/>
    public IEnumerable<KeyValuePair<Match, Winner>> Results =>
        allMatches.Where(kvp => kvp.Value != Winner.None);

    /// @copydoc ISessionDataProvider.Standings
    /// <seealso cref="ISessionDataProvider.Standings"/>
    public IEnumerable<KeyValuePair<IPlayer, int>> Standings =>
        standings.OrderByDescending(kvp => kvp.Value);

    /// @copydoc ISessionDataProvider.LastMatchResult
    /// <seealso cref="ISessionDataProvider.LastMatchResult"/>
    public Winner LastMatchResult => matchController?.Result ?? Winner.None;

    /// @copydoc ISessionDataProvider.WinnerString
    /// <seealso cref="ISessionDataProvider.WinnerString"/>
    public string WinnerString => matchController?.WinnerString;

    /// @copydoc ISessionDataProvider.WhoPlaysFirst
    /// <seealso cref="ISessionDataProvider.WhoPlaysFirst"/>
    public bool WhoPlaysFirst => uiWhoPlaysFirst;

    /// @copydoc ISessionDataProvider.BlockStartNextMatch
    /// <seealso cref="ISessionDataProvider.BlockStartNextMatch"/>
    public bool BlockStartNextMatch => uiBlockStartNextMatch;

    /// @copydoc ISessionDataProvider.BlockShowResult
    /// <seealso cref="ISessionDataProvider.BlockShowResult"/>
    public bool BlockShowResult => uiBlockShowResult;

    /// @copydoc ISessionDataProvider.UnblockedScreenDuration
    /// <seealso cref="ISessionDataProvider.UnblockedScreenDuration"/>
    public float UnblockedScreenDuration =>  unblockedScreenDuration;
}
