/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionView : MonoBehaviour
{
    private ISessionDataProvider sessionData;
    private Coroutine nonBlockingScreenTimer;
    private IReadOnlyList<Match> matches;
    private IReadOnlyList<KeyValuePair<Match, Winner>> results;
    private IReadOnlyList<KeyValuePair<IPlayer, int>> standings;

    // Vectors for holding the scrollviews
    private Vector2 scrollViewVector1 = Vector2.zero;
    private Vector2 scrollViewVector2 = Vector2.zero;

    private bool nextWhoPlaysFirst;

    private void Awake()
    {
        sessionData = GetComponent<ISessionDataProvider>();
    }

    private void Start()
    {
        // Get a list of all matches to be performed
        matches = new List<Match>(sessionData.Matches.Select(kvp => kvp.Key));

        // Show "who plays first" menu?
        nextWhoPlaysFirst = sessionData.WhoPlaysFirst;
    }

    private void OnGUI()
    {
        switch (sessionData.State)
        {
            case SessionState.Begin:
                if (sessionData.ShowListOfMatches)
                {
                    GUI.Window(0,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowListOfMatches,
                        "List of matches");
                }
                else
                {
                    OnPreMatch();
                }
                break;
            case SessionState.PreMatch:
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
                    // Start next match window
                    GUI.Window(2,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowStartNextMatch,
                        "Next match");
                }
                break;
            case SessionState.InMatch:
                // We don't do anything in this case, everything is done by the
                // match controller
                break;
            case SessionState.PostMatch:
                GUI.Window(3,
                    new Rect(0, 0, Screen.width, Screen.height),
                    WindowMatchResult,
                    "Match result");
                break;
            case SessionState.End:
                if (sessionData.ShowTournamentStandings)
                {
                    if (results == null)
                    {
                        results = new List<KeyValuePair<Match, Winner>>(
                            sessionData.Matches);
                        standings = new List<KeyValuePair<IPlayer, int>>(
                            sessionData.Standings);
                    }
                    GUI.Window(4,
                        new Rect(0, 0, Screen.width, Screen.height),
                        WindowFinalStandings,
                        "Final standings");
                }
                else
                {
                    OnEndSession();
                }
                break;
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
                    Screen.width * 3 /12
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
            // Variable for determining player standing when it has the
            // same points as previous player
            int pos = 0;

            // Determine an appropriate number of pixels per match
            int vPixelsPerMatch = Mathf.Max(18, Screen.height / 30);

            // Set text size depending on number of matches
            int fontSize = Mathf.Max(10, Screen.height / 50);

            // ////////////// //
            // SHOW STANDINGS //
            // ////////////// //

            // Show "Standings" label, above the list of standings
            GUI.Label(
                new Rect(
                    Screen.width / 6,
                    Screen.height / 6 - vPixelsPerMatch * 6 / 2,
                    Screen.width * 2 / 6,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                    fontSize, "orange", "Standings"));

            // Begin the ScrollView
            scrollViewVector1 = GUI.BeginScrollView(
                new Rect(
                    Screen.width / 6,
                    Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                    Screen.width * 3 / 12,
                    Screen.height * 4 / 6),
                scrollViewVector1,
                new Rect(
                    0,
                    0,
                    Screen.width * 1 / 6,
                    vPixelsPerMatch * (standings.Count + 1)));

            // Draw a box for the scrollview contents
            GUI.Box(
                new Rect(
                    0,
                    0,
                    Screen.width * 3 /12
                        - GUI.skin.verticalScrollbar.fixedWidth - 1,
                    vPixelsPerMatch * (standings.Count + 1)),
                "");

            // Show "Player" and "Points" labels, above the list of standings
            GUI.Label(
                new Rect(
                    0,
                    0,
                    Screen.width * 2 / 6,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                    fontSize, "olive", "  Player"));

            GUI.Label(
                new Rect(
                    Screen.width * 1 / 6,
                    0,
                    Screen.width * 1 / 6,
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
                        Screen.width * 1 / 6,
                        vPixelsPerMatch),
                    string.Format("<size={0}><color={1}>{2}</color></size>",
                        fontSize, color, $"{pos,3}. {standings[i].Key}"));

                // Show player points
                GUI.Label(
                    new Rect(
                        Screen.width * 1 / 6,
                        (i + 1) * vPixelsPerMatch,
                        Screen.width * 1 / 6,
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
                    Screen.width / 2,
                    Screen.height / 6 - vPixelsPerMatch * 6 / 2,
                    Screen.width * 3 / 12,
                    vPixelsPerMatch),
                string.Format("<size={0}><color={1}><b>{2}</b></color></size>",
                    fontSize, "orange", "Results"));

            // Begin the ScrollView
            scrollViewVector2 = GUI.BeginScrollView(
                new Rect(
                    Screen.width / 2,
                    Screen.height / 6 - vPixelsPerMatch * 3 / 2,
                    Screen.width * 3 / 12,
                    Screen.height * 4 / 6),
                scrollViewVector2,
                new Rect(
                    0,
                    0,
                    Screen.width * 1 / 6,
                    vPixelsPerMatch * results.Count));

            // Draw a box for the scrollview contents
            GUI.Box(
                new Rect(
                    0,
                    0,
                    Screen.width * 3 /12
                        - GUI.skin.verticalScrollbar.fixedWidth - 1,
                    vPixelsPerMatch * results.Count),
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
                        Screen.width * 3 / 12,
                        vPixelsPerMatch),
                    string.Format("<size={0}><color={1}> {2}</color></size>",
                        fontSize, color,
                        $"{player1} vs {player2}"));
            }

            // End the ScrollView
            GUI.EndScrollView();

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

    private IEnumerator NonBlockingScreenTimer(Action eventToInvoke)
    {
        yield return new WaitForSeconds(sessionData.NoMatchScreenDuration);
        eventToInvoke?.Invoke();
        nonBlockingScreenTimer = null;
    }

    private void OnPreMatch()
    {
        nextWhoPlaysFirst = sessionData.WhoPlaysFirst;
        PreMatch?.Invoke();
    }

    private void OnSwapPlayers()
    {
        SwapPlayers?.Invoke();
    }

    private void OnStartNextMatch()
    {
        StartNextMatch?.Invoke();
    }

    private void OnMatchClear()
    {
        MatchClear?.Invoke();
    }

    private void OnEndSession()
    {
        EndSession?.Invoke();
    }

    public event Action PreMatch;
    public event Action SwapPlayers;
    public event Action StartNextMatch;
    public event Action MatchClear;
    public event Action EndSession;
}
