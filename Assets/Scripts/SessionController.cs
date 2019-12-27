/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using System.Collections.Generic;

public class SessionController : MonoBehaviour, ISessionDataProvider
{
    private enum Status { Init, InMatch, InBtwMatches, Finish }
    private enum SessionType { HumanVsHuman, PlayerVsPlayer, AllVsAll }
    private struct Match
    {
        public readonly IPlayer player1;
        public readonly IPlayer player2;
        public IPlayer this[PColor color] => color == PColor.White ? player1
                : color == PColor.Red ? player2
                    : throw new InvalidOperationException(
                        $"Invalid player color");
        public Match(IPlayer player1, IPlayer player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
        public Match Swap() => new Match(player2, player1);
    }

    [SerializeField] private GameObject matchPrefab = null;
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 7;
    [SerializeField] private int winSequence = 4;
    [SerializeField] private int roundPiecesPerPlayer = 10;
    [SerializeField] private int squarePiecesPerPlayer = 11;

    [Tooltip("Maximum real time that AI can take to play")]
    [SerializeField] private float aITimeLimit = 0.5f;

    [Tooltip("Even if the AI plays immediately, this time gives the "
        + "illusion that the AI took some minimum time to play")]
    [SerializeField] private float minAIGameMoveTime = 0.25f;

    private Board board;
    private Match nextMatch;
    private IEnumerable<Match> allMatches;

    private GameObject matchInstance = null;
    private MatchController matchController = null;

    private Status status;
    private SessionType sessionType;

    private IList<AIPlayer> activeAIs = null;

    private IPlayer humanPlayer;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get all active AIs and put them in a list
        List<AIPlayer> allAIs = new List<AIPlayer>();
        GetComponents(allAIs);
        activeAIs = allAIs.FindAll(ai => ai.IsActive);

        // Set the init status, before any matches begin
        status = Status.Init;

        // Instantiate a human player if there are not enough AIs to do a match
        if (activeAIs.Count < 2)
            humanPlayer = new HumanPlayer();
    }

    // Start is called on the frame when a script is enabled just before any
    // of the Update methods are called the first time.
    private void Start()
    {
        if (activeAIs.Count == 0)
        {
            // A game between human players, ask user to press OK to start
            sessionType = SessionType.HumanVsHuman;
            nextMatch = new Match(humanPlayer, humanPlayer);
        }
        else if (activeAIs.Count == 1)
        {
            // A game between a human and an AI, ask who plays first
            sessionType = SessionType.PlayerVsPlayer;
            nextMatch = new Match(humanPlayer, activeAIs[0]);
        }
        else if (activeAIs.Count == 2)
        {
            // A game between two AIs, ask who plays first
            sessionType = SessionType.PlayerVsPlayer;
            nextMatch = new Match(activeAIs[0], activeAIs[1]);
        }
        else
        {
            // Multiple AIs, run a competition, show the list of AIs and
            // ask user to press OK to start
            sessionType = SessionType.AllVsAll;

            // Prepare matches
        }
    }

    private void StartNextMatch()
    {
        // Instantiate a board for the next match
        board = new Board(rows, cols, winSequence,
            roundPiecesPerPlayer, squarePiecesPerPlayer);

        // Instantiate the next match
        matchInstance = Instantiate(matchPrefab, transform);
        matchInstance.name = "Match";

        // Get a reference to the match controller of the next match
        matchController = matchInstance.GetComponent<MatchController>();

        // Add a listener for the match over event
        matchController.MatchOver.AddListener(EndCurrentMatch);

        // Set status as in match
        status = Status.InMatch;
    }

    private void OnGUI()
    {
        if (status == Status.Init)
        {
            if (sessionType == SessionType.HumanVsHuman)
            {
                // Press OK to continue
                GUI.Window(0,
                    new Rect(0, 0, Screen.width, Screen.height),
                    DrawOkLetsStartWindow,
                    "Human vs Human");
            }
            else if (sessionType == SessionType.PlayerVsPlayer)
            {
                // Ask who plays first
                GUI.Window(1,
                    new Rect(0, 0, Screen.width, Screen.height),
                    DrawWhoPlaysFirstWindow,
                    "Who plays first (white)?");
            }
            else if (sessionType == SessionType.AllVsAll)
            {
                // Show list and press OK to continue
                GUI.Window(2,
                    new Rect(0, 0, Screen.width, Screen.height),
                    DrawCompetitionWindow,
                    "Competition");
            }
        }
        else if (status == Status.Finish)
        {
            if (sessionType == SessionType.AllVsAll)
            {
                GUI.Window(3,
                    new Rect(0, 0, Screen.width, Screen.height),
                    DrawSessionOverWindow,
                    "Session over");
            }
            else
            {
                GUI.Window(4,
                    new Rect(0, 0, Screen.width, Screen.height),
                    DrawMatchOverWindow,
                    "Match Over!");
            }
        }
    }

    // Draw window contents
    private void DrawOkLetsStartWindow(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {
            // Draw OK button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                "Start Match"))
            {
                // If button is clicked, start game
                StartNextMatch();
            }
        }
    }

    // Draw window contents
    private void DrawWhoPlaysFirstWindow(int id)
    {
        // Is this the correct window?
        if (id == 1)
        {
            // Draw buttons to ask who plays first
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 150,
                    Screen.height / 2 - 25,
                    140,
                    50),
                nextMatch[PColor.White].PlayerName))
            {
                // No need to swap players, just start the game
                StartNextMatch();
            }
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 + 10,
                    Screen.height / 2 - 25,
                    140,
                    50),
                nextMatch[PColor.Red].PlayerName))
            {
                // Swap players...
                nextMatch = nextMatch.Swap();
                // ...and then start game
                StartNextMatch();
            }
        }
    }

    // Draw window contents
    private void DrawCompetitionWindow(int id)
    {
        // Is this the correct window?
        if (id == 2)
        {
            // If competition hasn't started yet, list all the teams
            if (status == Status.Init)
            {
                // Draw OK button
                if (GUI.Button(
                    new Rect(
                        Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                    "Go to first match"))
                {
                    // Change status, so next screen is information about
                    // first match
                    status = Status.InBtwMatches;
                }
            }
            // Show information about next match
            else if (status == Status.InBtwMatches)
            {

            }
        }
    }

    // Draw window contents
    private void DrawSessionOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 3)
        {
            // Draw OK button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                "OK"))
            {
                // If button is clicked, exit
                Destroy(matchInstance);
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    // Draw window contents
    private void DrawMatchOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 4)
        {
            // Keep original content color
            Color originalColor = GUI.contentColor;
            // Determine new content color depending on the result
            Color color = matchController.Result == Winner.Draw
                ? Color.yellow
                : matchController.Result == Winner.White
                    ? Color.white
                    : Color.red;
            // Define a text-centered gui style
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);
            guiLabelStyle.alignment = TextAnchor.MiddleCenter;
            guiLabelStyle.fontSize = Screen.width / 30;
            // Set content color
            GUI.contentColor = color;
            // Show the label indicating the final result of the game
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height / 4,
                    Screen.width * 2 / 3,
                    Screen.height / 8),
                matchController.Result == Winner.Draw
                    ? "It's a draw"
                    : $"Winner is {matchController.WinnerString}",
                guiLabelStyle);
            // Set content color back to the original color
            GUI.contentColor = originalColor;
            // Draw OK button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                "OK"))
            {
                // If button is clicked, exit
                Destroy(matchInstance);
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    private void EndCurrentMatch()
    {
        // TODO Consider all vs all situation
        status = Status.Finish;
    }


    // Implementation of ISessionDataProvider
    public Board Board => board;
    public IPlayer CurrentPlayer => nextMatch[board.Turn];
    public float AITimeLimit => aITimeLimit;
    public float TimeBetweenAIMoves => minAIGameMoveTime;
    public IPlayer GetPlayer(PColor player) => nextMatch[player];
}
