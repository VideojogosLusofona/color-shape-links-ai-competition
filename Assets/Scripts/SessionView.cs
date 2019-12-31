/// @file
/// @brief This file contains the ::SessionView class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which renders the session UI.
/// </summary>
/// <remarks>
/// Based on the MVC design pattern, composed in this case by the following
/// classes:
/// * *Model* - A list of <see cref="Match"/> instances can be considered the
/// model, although there isn't a well defined model in this case.
/// * *View* - This class.
/// * *Controller* - <see cref="SessionController"/>.
/// </remarks>
public class SessionView : MonoBehaviour
{
    // Reference to the session data provider
    private ISessionDataProvider sessionData;

    // Reference to the coroutine that provides a timer for non-blocking
    // session UI screens (next match screen and match results screen)
    private Coroutine nonBlockingScreenTimer;

    // List of all matches to be played
    private IReadOnlyList<Match> matches;

    // List of results of matches played so far (match-result pairs)
    private IReadOnlyList<KeyValuePair<Match, Winner>> results;

    // List of current standings (player-points pairs)
    private IReadOnlyList<KeyValuePair<IPlayer, int>> standings;

    // Vectors for holding the scrollviews
    private Vector2 scrollViewVector1 = Vector2.zero;
    private Vector2 scrollViewVector2 = Vector2.zero;

    // Ask who plays first in the next frame?
    private bool nextWhoPlaysFirst;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get reference to the session data
        sessionData = GetComponent<ISessionDataProvider>();
    }

    // Start is called on the frame when a script is enabled just before any of
    // the Update methods are called the first time
    private void Start()
    {
        // Get a list of all matches to be played
        matches = new List<Match>(sessionData.Matches);

        // Show "who plays first" menu?
        nextWhoPlaysFirst = sessionData.WhoPlaysFirst;
    }

    // OnGUI is called for rendering and handling GUI events. This means that
    // OnGUI might be called several times per frame (one call per event).
    private void OnGUI()
    {
        // The UI to present depends on the session state
        switch (sessionData.State)
        {
            // We're in the session Begin state
            case SessionState.Begin:

                // Are we in tournament mode?
                if (matches.Count > 1)
                {
                    // If so, present a list of matches
                    GUI.Window(0,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowListOfMatches,
                        "List of matches");
                }
                else
                {
                    // Otherwise, notify listeners that we should move to the
                    // PreMatch state
                    OnPreMatch();
                }
                break;

            // We're in the session PreMatch state
            case SessionState.PreMatch:

                // Should we ask who plays first or do we already know?
                if (nextWhoPlaysFirst)
                {
                    // Ask who plays first
                    GUI.Window(1,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowWhoPlaysFirst,
                        "Who plays first/white?");
                }
                else
                {
                    // Show "Start next match" window
                    GUI.Window(2,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowStartNextMatch,
                        "Next match");
                }
                break;

            // We're in the session InMatch state
            case SessionState.InMatch:
                // Here we simply set results to null, so that when we move to
                // the next state (PostMatch), results and standings get
                // updated
                if (results != null)
                {
                    results = null;
                }
                break;

            // We're in the session PostMatch state
            case SessionState.PostMatch:

                // If results is null, let's retrieve them together with the
                // updated standings
                if (results == null)
                {
                    results = new List<KeyValuePair<Match, Winner>>(
                        sessionData.Results);
                    standings = new List<KeyValuePair<IPlayer, int>>(
                        sessionData.Standings);
                }
                GUI.Window(3,
                    new Rect(0, 0, Screen.width, Screen.height),
                    WindowMatchResult,
                    "Match result");
                break;

            // We're in the session End state
            case SessionState.End:

                // Are we in tournament mode?
                if (matches.Count > 1)
                {
                    // If so, show final standings and results
                    GUI.Window(4,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowFinalStandings,
                        "Final standings");
                }
                else
                {
                    // Otherwise, just notify listeners that session may be
                    // effectively terminated
                    OnEndSession();
                }
                break;

            // Invalid state, thrown exception
            default:
                throw new InvalidOperationException(
                    $"Unknown session state: {sessionData.State}");
        }
    }

    // Draw contents of list of matches window
    private void WindowListOfMatches(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {

            // Determine an appropriate number of pixels per match
            int vPixelsPerMatch = Mathf.Max(18, Screen.height / 30);

            // Set text size depending on number of matches
            int fontSize = Mathf.Max(10, Screen.height / 50);

            // Show "Matches to play" label, above the list of matches
            GUI.Label(
                new Rect(
                    Screen.width / 6,
                    Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                    Screen.width * 2 / 6,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                    fontSize, "orange", "Matches to play"));

            // Begin the ScrollView
            scrollViewVector1 = GUI.BeginScrollView(
                new Rect(
                    Screen.width / 6,
                    Screen.height / 6,
                    Screen.width * 3 / 12,
                    Screen.height * 4 / 6),
                scrollViewVector1,
                new Rect(
                    0,
                    0,
                    Screen.width * 1 / 6,
                    vPixelsPerMatch * matches.Count));

            // Draw a box for the scrollview contents
            GUI.Box(
                new Rect(
                    0,
                    0,
                    Screen.width * 3 / 12
                        - GUI.skin.verticalScrollbar.fixedWidth - 1,
                    vPixelsPerMatch * matches.Count),
                "");

            // Show match list
            for (int i = 0; i < matches.Count; i++)
            {
                // Set alternating color for each match
                string color = i % 2 == 0 ? "white" : "grey";

                // Show match
                GUI.Label(
                    new Rect(
                        0,
                        i * vPixelsPerMatch,
                        Screen.width * 3 / 12,
                        vPixelsPerMatch),
                    string.Format(
                        "<size={0}><color={1}><i>{2,4}.</i>  {3}</color></size>",
                        fontSize, color, i + 1, matches[i]));
            }

            // End the ScrollView
            GUI.EndScrollView();

            // Draw go to first match button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2,
                    Screen.height / 6,
                    Screen.width / 4,
                    Screen.height / 8),
                "Start tournament"))
            {
                // Notify we should pass to pre-match state
                OnPreMatch();
            }
        }
    }

    // Draw contents of list of who plays first window
    private void WindowWhoPlaysFirst(int id)
    {
        // Is this the correct window?
        if (id == 1)
        {
            // Draw buttons to ask who plays first
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - Screen.width * 6 / 20,
                    Screen.height / 2 - Screen.height / 16,
                    Screen.width / 4,
                    Screen.height / 8),
                sessionData.PlayerWhite))
            {
                // No need to swap players, just disable this menu next frame
                nextWhoPlaysFirst = false;
            }
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 + Screen.width / 20,
                    Screen.height / 2 - Screen.height / 16,
                    Screen.width / 4,
                    Screen.height / 8),
                sessionData.PlayerRed))
            {
                // Notify player swap
                OnSwapPlayers();
                // Disable this menu next frame
                nextWhoPlaysFirst = false;
            }
        }
    }

    // Draw contents of the start next match window
    private void WindowStartNextMatch(int id)
    {
        // Is this the correct window?
        if (id == 2)
        {
            // Get the default style for labels
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);

            // Define a text-centered gui style
            guiLabelStyle.alignment = TextAnchor.MiddleCenter;
            guiLabelStyle.fontSize = Screen.width / 35;

            // Draw a box under the label
            GUI.Box(
                new Rect(
                    Screen.width / 2 - Screen.width / 5,
                    Screen.height * 1 / 10,
                    Screen.width * 2 / 5,
                    Screen.height / 4),
                "");

            // Show the label for next match
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 5,
                    Screen.height * 1 / 10,
                    Screen.width * 2 / 5,
                    Screen.height / 4),
                string.Format("{0}{1}{2}",
                    $"<color=white>{sessionData.PlayerWhite}</color>\n",
                    "<color=grey>vs</color>\n",
                    $"<color=red>{sessionData.PlayerRed}</color>"),
                guiLabelStyle);

            // Is this a tournament and is this not the first game?
            if (results?.Count >= 1)
            {
                // Determine an appropriate number of pixels per match
                int vPixelsPerMatch = Mathf.Max(16, Screen.height / 40);

                // Set text size depending on number of matches
                int fontSize = Mathf.Max(8, Screen.height / 65);

                // The rect for the standings
                Rect rectStd = new Rect(
                    Screen.width / 12,
                    Screen.height * 1 / 10,
                    Screen.width / 6,
                    Screen.height * 4 / 6);

                // The rect for the results
                Rect rectRes = new Rect(
                    Screen.width * 9 / 12,
                    Screen.height * 1 / 10,
                    Screen.width / 6,
                    Screen.height * 4 / 6);

                // If so, show standings and results
                ShowStandingsAndResults(
                    vPixelsPerMatch, fontSize, rectStd, rectRes);
            }

            // Is this a blocking screen?
            if (sessionData.BlockStartNextMatch)
            {
                // If so, screen will be unblocked when user presses button

                // Draw next match button
                if (GUI.Button(
                    new Rect(
                        Screen.width / 2 - Screen.width / 7,
                        Screen.height / 2,
                        Screen.width * 2 / 7,
                        Screen.height / 8),
                    "Start"))
                {
                    // Notify start of next match
                    OnStartNextMatch();
                }
            }
            else
            {
                // Otherwise, screen will be unlocked after some time

                // The unlock period is assured by a coroutine
                if (nonBlockingScreenTimer == null)
                {
                    nonBlockingScreenTimer = StartCoroutine(
                        NonBlockingScreenTimer(OnStartNextMatch));
                }
            }
        }
    }

    // Draw contents of the match results window
    private void WindowMatchResult(int id)
    {
        // Is this the correct window?
        if (id == 3)
        {
            // Determine new content color depending on the result
            string color = sessionData.LastMatchResult == Winner.Draw
                ? "grey"
                : sessionData.LastMatchResult == Winner.White
                    ? "white"
                    : "red";
            string result = sessionData.LastMatchResult == Winner.Draw
                    ? "It's a draw"
                    : $"Winner is {sessionData.WinnerString}";

            // Define a text-centered gui style
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);
            guiLabelStyle.alignment = TextAnchor.MiddleCenter;
            guiLabelStyle.fontSize = Screen.width / 35;

            // Draw a box under the label
            GUI.Box(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height * 7 / 8 - Screen.height / 16,
                    Screen.width * 2 / 3,
                    Screen.height / 8),
                "");

            // Show the label indicating the final result of the game
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height * 7 / 8 - Screen.height / 16,
                    Screen.width * 2 / 3,
                    Screen.height / 8),
                $"<color={color}>{result}</color>",
                guiLabelStyle);

            // Is this a blocking screen?
            if (sessionData.BlockShowResult)
            {
                // If so, screen will be unblocked when user presses button

                // Draw unlock button
                if (GUI.Button(
                    new Rect(
                        Screen.width / 2 - Screen.width / 10,
                        Screen.height / 2 - Screen.height / 14,
                        Screen.width * 2 / 10,
                        Screen.height / 8),
                    "OK"))
                {
                    // Notify result shown
                    OnMatchClear();
                }
            }
            else
            {
                // Otherwise, screen will be unlocked after some time

                // The unlock period is assured by a coroutine
                if (nonBlockingScreenTimer == null)
                {
                    nonBlockingScreenTimer = StartCoroutine(
                        NonBlockingScreenTimer(OnMatchClear));
                }
            }
        }
    }

    // Draw contents of final standings window
    private void WindowFinalStandings(int id)
    {
        // Is this the correct window?
        if (id == 4)
        {
            // Determine an appropriate number of pixels per match
            int vPixelsPerMatch = Mathf.Max(18, Screen.height / 30);

            // Set text size depending on number of matches
            int fontSize = Mathf.Max(10, Screen.height / 50);

            // The rect for the standings
            Rect rectStd = new Rect(
                Screen.width / 6,
                Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                Screen.width * 3 / 12,
                Screen.height * 4 / 6);

            // The rect for the results
            Rect rectRes = new Rect(
                Screen.width / 2,
                Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                Screen.width * 3 / 12,
                Screen.height * 4 / 6);

            // Show standings and results
            ShowStandingsAndResults(
                vPixelsPerMatch, fontSize, rectStd, rectRes);

            // Draw "Finish" button
            if (GUI.Button(
                new Rect(
                    Screen.width * 5 / 6,
                    Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                    Screen.width / 12,
                    Screen.height / 10),
                "Quit"))
            {
                // If button is clicked, notify session end
                OnEndSession();
            }
        }
    }

    // Method for showing standings and results
    private void ShowStandingsAndResults(
        int vPixelsPerMatch, int fontSize, Rect rectStd, Rect rectRes)
    {
        // Variable for determining player standing when it has the
        // same points as previous player
        int pos = 0;

        // ////////////// //
        // SHOW STANDINGS //
        // ////////////// //

        // Show "Standings" label, above the list of standings
        GUI.Label(
            new Rect(
                rectStd.x,
                rectStd.y - vPixelsPerMatch * 3 / 2,
                rectStd.width,
                vPixelsPerMatch),
            string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                fontSize, "orange", "Standings"));

        // Begin the ScrollView
        scrollViewVector1 = GUI.BeginScrollView(
            rectStd,
            scrollViewVector1,
            new Rect(
                0,
                0,
                rectStd.width - Screen.width * 1 / 12,
                vPixelsPerMatch * (standings.Count + 1) + 2));

        // Draw a box for the scrollview contents
        GUI.Box(
            new Rect(
                0,
                0,
                rectStd.width - GUI.skin.verticalScrollbar.fixedWidth - 1,
                vPixelsPerMatch * (standings.Count + 1) + 2),
            "");

        // Show "Player" and "Points" labels, above the list of standings
        GUI.Label(
            new Rect(
                0,
                0,
                rectStd.width + Screen.width * 1 / 12,
                vPixelsPerMatch),
            string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                fontSize, "olive", "  Player"));

        GUI.Label(
            new Rect(
                rectStd.width * 3 / 5,
                0,
                rectStd.width - Screen.width * 1 / 12,
                vPixelsPerMatch),
            string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                fontSize, "olive", "Points"));

        // Show standings
        for (int i = 0; i < standings.Count; i++)
        {
            // Set alternating color for each player
            string color = i % 2 == 0 ? "white" : "grey";

            // If player has the same points as the previous player, then
            // it should have the same standing
            if (i == 0)
            {
                pos = 1;
            }
            else if (standings[i].Value != standings[i - 1].Value)
            {
                pos = i + 1;
            }

            // Show player name
            GUI.Label(
                new Rect(
                    0,
                    (i + 1) * vPixelsPerMatch,
                    rectStd.width - Screen.width * 1 / 12,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}>{2}</color></size>",
                    fontSize, color, $"{pos,3}. {standings[i].Key}"));

            // Show player points
            GUI.Label(
                new Rect(
                    rectStd.width * 3 / 5,
                    (i + 1) * vPixelsPerMatch,
                    rectStd.width - Screen.width * 1 / 12,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}>    {2,2}</color></size>",
                    fontSize, color, standings[i].Value));
        }

        // End the ScrollView
        GUI.EndScrollView();

        // //////////// //
        // SHOW RESULTS //
        // //////////// //

        // Show "Results" label, above the list of results
        GUI.Label(
            new Rect(
                rectRes.x,
                rectRes.y - vPixelsPerMatch * 3 / 2,
                rectRes.width,
                vPixelsPerMatch),
            string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                fontSize, "orange", "Results"));

        // Begin the ScrollView
        scrollViewVector2 = GUI.BeginScrollView(
            rectRes,
            scrollViewVector2,
            new Rect(
                0,
                0,
                rectRes.width - Screen.width * 1 / 12,
                vPixelsPerMatch * results.Count + 2));

        // Draw a box for the scrollview contents
        GUI.Box(
            new Rect(
                0,
                0,
                rectRes.width - GUI.skin.verticalScrollbar.fixedWidth - 1,
                vPixelsPerMatch * results.Count + 2),
            "");

        // Show match results
        for (int i = 0; i < results.Count; i++)
        {
            // Color for current result
            string color = results[i].Value == Winner.White
                ? "white"
                : results[i].Value == Winner.Red ? "red" : "grey";

            // Is first player bold (winner)?
            string player1 = results[i].Value == Winner.White
                ? $"<b>{results[i].Key.player1}</b>"
                : results[i].Key.player1.ToString();

            // Is second player bold (winner)?
            string player2 = results[i].Value == Winner.Red
                ? $"<b>{results[i].Key.player2}</b>"
                : results[i].Key.player2.ToString();

            // Show match, result is based on color
            GUI.Label(
                new Rect(
                    0,
                    i * vPixelsPerMatch,
                    rectRes.width,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}> {2}</color></size>",
                    fontSize, color,
                    $"{player1} vs {player2}"));
        }

        // End the ScrollView
        GUI.EndScrollView();
    }

    // Coroutine that invokes a delegate after a certain amount of time
    // In practice, it's used as a timer for non-blocking session UI screens,
    // namely the "next match" screen and the "match results" screen
    private IEnumerator NonBlockingScreenTimer(Action eventToInvoke)
    {
        // We'll get back in a moment
        yield return new WaitForSeconds(sessionData.UnblockedScreenDuration);

        // Invoke delegate
        eventToInvoke?.Invoke();

        // Set coroutine reference to null, so others know that no timer is
        // currently set
        nonBlockingScreenTimer = null;
    }

    // Invoke the PreMatch notification event
    private void OnPreMatch()
    {
        nextWhoPlaysFirst = sessionData.WhoPlaysFirst;
        PreMatch?.Invoke();
    }

    // Invoke the SwapPlayers notification event
    private void OnSwapPlayers()
    {
        SwapPlayers?.Invoke();
    }

    // Invoke the StartNextMatch notification event
    private void OnStartNextMatch()
    {
        StartNextMatch?.Invoke();
    }

    // Invoke the MatchClear notification event
    private void OnMatchClear()
    {
        MatchClear?.Invoke();
    }

    // Invoke the EndSession notification event
    private void OnEndSession()
    {
        EndSession?.Invoke();
    }

    /// <summary>
    /// Event which notifies listeners that the session should move to the
    /// PreMatch state.
    /// </summary>
    public event Action PreMatch;

    /// <summary>
    /// Event which notifies listeners that current match players should be
    /// swapped.
    /// </summary>
    public event Action SwapPlayers;

    /// <summary>
    /// Event which notifies listeners that the next match should be started.
    /// </summary>
    public event Action StartNextMatch;

    /// <summary>
    /// Event which notifies listeners that the current match should be
    /// destroyed / cleared.
    /// </summary>
    public event Action MatchClear;

    /// <summary>
    /// Event which notifies listeners that the session terminate.
    /// </summary>
    public event Action EndSession;
}
