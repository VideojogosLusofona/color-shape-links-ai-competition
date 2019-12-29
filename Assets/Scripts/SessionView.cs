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
    [SerializeField] private float nonBlockingScreenDuration = 1.5f;
    private ISessionDataProvider sessionData;
    private Coroutine nonBlockingScreenTimer;
    private IReadOnlyList<string> matches;
    private IReadOnlyList<KeyValuePair<string, Winner>> results;
    private IReadOnlyList<KeyValuePair<IPlayer, int>> standings;

    private bool nextWhoPlaysFirst;

    private void Awake()
    {
        sessionData = GetComponent<ISessionDataProvider>();
    }

    private void Start()
    {
        // Get a list of all matches to be performed
        matches = new List<string>(sessionData.Matches.Select(kvp => kvp.Key));

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
                // TODO In game tournament info
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
                        results = new List<KeyValuePair<string, Winner>>(
                            sessionData.Matches);
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
            // Get the default style for labels
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);

            // Determine an appropriate number of pixels per match
            int vPixelsPerMatch =
                Screen.height / Math.Max(matches.Count, 20);

            // Determine an appropriate vertical position for the first match
            int firstMatchVertPos = Screen.height / 2 - Mathf.Min(
                Screen.height * 9 / 20,
                matches.Count * vPixelsPerMatch / 2);

            // Set text size depending on number of matches
            guiLabelStyle.fontSize =
                Screen.height / Mathf.Max(matches.Count, 35);

            // Show match list
            for (int i = 0; i < matches.Count; i++)
            {
                GUI.Label(
                    new Rect(
                        Screen.width / 6,
                        firstMatchVertPos + i * vPixelsPerMatch,
                        Screen.width * 2 / 6,
                        vPixelsPerMatch),
                    matches[i],
                    guiLabelStyle);
            }

            // Draw go to first match button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 + Screen.width / 8,
                    Screen.height / 2 - Screen.height / 16,
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
            // Keep original content color
            Color originalColor = GUI.contentColor;

            // Get the default style for labels
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);

            // Define a text-centered gui style
            guiLabelStyle.alignment = TextAnchor.MiddleCenter;
            guiLabelStyle.fontSize = Screen.width / 30;

            // Set content color for player 1 (white)
            GUI.contentColor = Color.white;

            // Show the label for player 1 (white)
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height * 1 / 10,
                    Screen.width * 2 / 3,
                    Screen.height / 10),
                sessionData.PlayerWhite,
                guiLabelStyle);

            // Set content color for VS word
            GUI.contentColor = Color.gray;

            // Show the label for VS word
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height * 2 / 10,
                    Screen.width * 2 / 3,
                    Screen.height / 10),
                "vs",
                guiLabelStyle);

            // Set content color for player 2 (red)
            GUI.contentColor = Color.red;

            // Show the label for player 2 (red)
            GUI.Label(
                new Rect(
                    Screen.width / 2 - Screen.width / 3,
                    Screen.height * 3 / 10,
                    Screen.width * 2 / 3,
                    Screen.height / 10),
                sessionData.PlayerRed,
                guiLabelStyle);

            // Set content color back to the original color
            GUI.contentColor = originalColor;

            // Is this a blocking screen?
            if (sessionData.BlockStartNextMatch)
            {
                // If so, screen will be unblocked when user presses button

                // Draw next match button
                if (GUI.Button(
                    new Rect(
                        Screen.width / 2 - Screen.width / 7,
                        Screen.height * 9 / 20,
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
            // Keep original content color
            Color originalColor = GUI.contentColor;

            // Determine new content color depending on the result
            Color color = sessionData.LastMatchResult == Winner.Draw
                ? Color.yellow
                : sessionData.LastMatchResult == Winner.White
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
                sessionData.LastMatchResult == Winner.Draw
                    ? "It's a draw"
                    : $"Winner is {sessionData.WinnerString}",
                guiLabelStyle);

            // Set content color back to the original color
            GUI.contentColor = originalColor;

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

            // Get the default style for labels
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);

            // Determine an appropriate number of pixels per match
            int vPixelsPerMatch =
                Screen.height / Math.Max(results.Count, 20);

            // Determine an appropriate vertical position for the first match
            int firstMatchVertPos = Screen.height / 2 - Mathf.Min(
                Screen.height * 9 / 20,
                results.Count * vPixelsPerMatch / 2);

            // Set text size depending on number of matches
            guiLabelStyle.fontSize =
                Screen.height / Mathf.Max(results.Count, 35);

            // Show match results
            for (int i = 0; i < results.Count; i++)
            {
                // Match
                GUI.Label(
                    new Rect(
                        Screen.width / 6,
                        firstMatchVertPos + i * vPixelsPerMatch,
                        Screen.width * 2 / 6,
                        vPixelsPerMatch),
                    results[i].Key,
                    guiLabelStyle);
                // Result
                GUI.Label(
                    new Rect(
                        Screen.width * 3 / 6,
                        firstMatchVertPos + i * vPixelsPerMatch,
                        Screen.width * 1 / 6,
                        vPixelsPerMatch),
                    results[i].Value == Winner.White
                        ? "1" : results[i].Value == Winner.Red ? "2" : "X",
                    guiLabelStyle);
            }

            // Draw "Finish" button
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 + Screen.width / 8,
                    Screen.height / 2 - Screen.height / 16,
                    Screen.width / 4,
                    Screen.height / 8),
                "Finish"))
            {
                // If button is clicked, notify session end
                OnEndSession();
            }
        }
    }

    private IEnumerator NonBlockingScreenTimer(Action eventToInvoke)
    {
        yield return new WaitForSeconds(nonBlockingScreenDuration);
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
