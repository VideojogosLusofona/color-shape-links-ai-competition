/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using System.Collections.Generic;

public class SessionController : MonoBehaviour
{
    private enum Status { Init, InGame, BtwGames, Finish }
    private enum SessionType { HumanVsHuman, PlayerVsPlayer, AllVsAll }

    [SerializeField] private GameObject gamePrefab = null;
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 7;
    [SerializeField] private int winSequence = 4;
    [SerializeField] private int squarePiecesPerPlayer = 11;
    [SerializeField] private int roundPiecesPerPlayer = 10;

    private GameObject gameInstance = null;
    private GameController gameController = null;

    private Status status;
    private SessionType sessionType;

    private IList<AIPlayer> activeAIs = null;

    private IPlayer nextPlayerA, nextPlayerB;

    private IPlayer humanPlayer;

    private void Awake()
    {
        List<AIPlayer> allAIs = new List<AIPlayer>();
        GameObject.Find("AIs")?.GetComponents(allAIs);
        activeAIs = allAIs.FindAll(ai => ai.IsActive);
        status = Status.Init;
        humanPlayer = new HumanPlayer();
    }

    private void Start()
    {
        if (activeAIs.Count == 0)
        {
            // A game between human players, ask user to press OK to start
            nextPlayerA = humanPlayer;
            nextPlayerB = humanPlayer;
            sessionType = SessionType.HumanVsHuman;
        }
        else if (activeAIs.Count == 1)
        {
            // A game between a human and an AI, ask who plays first
            nextPlayerA = humanPlayer;
            nextPlayerB = activeAIs[0];
            sessionType = SessionType.PlayerVsPlayer;
        }
        else if (activeAIs.Count == 2)
        {
            // A game between two AIs, ask who plays first
            nextPlayerA = activeAIs[0];
            nextPlayerB = activeAIs[1];
            sessionType = SessionType.PlayerVsPlayer;
        }
        else
        {
            // Multiple AIs, run a competition, show the list of AIs and
            // ask user to press OK to start
            sessionType = SessionType.AllVsAll;
        }
    }

    private void StartGame()
    {
        gameInstance = Instantiate(gamePrefab);
        gameInstance.name = "Game";
        gameController = gameInstance.GetComponent<GameController>();
        // TODO this should go to OnEnable
        gameController.SetupGame(nextPlayerA, nextPlayerB,
            rows, cols, winSequence,
            squarePiecesPerPlayer, roundPiecesPerPlayer);
        gameController.GameOver += EndCurrentGame;
        status = Status.InGame;
    }

    private void OnGUI()
    {
        if (status == Status.Init)
        {
            if (sessionType == SessionType.HumanVsHuman)
            {
                // Press OK to continue
                GUI.Window(0,
                    new Rect(
                        Screen.width / 2 - 100,
                        Screen.height / 2 - 50, 200, 100),
                    DrawOkLetsStartWindow,
                    "Human vs Human");
            }
            else if (sessionType == SessionType.PlayerVsPlayer)
            {
                // Ask who plays first
                GUI.Window(1,
                    new Rect(
                        Screen.width / 2 - 200,
                        Screen.height / 2 - 50, 400, 100),
                    DrawWhoPlaysFirstWindow,
                    "Who plays first (white)?");
            }
            else if (sessionType == SessionType.AllVsAll)
            {
                // Show list and press OK to continue
                GUI.Window(2,
                    new Rect(
                        Screen.width / 2 - 100,
                        Screen.height / 2 - 50, 200, 100),
                    DrawCompetitionWindow,
                    "Competition");
            }
        }
        else if (status == Status.Finish)
        {
            GUI.Window(3,
                new Rect(
                    Screen.width / 2 - 100,
                    Screen.height / 2 - 50, 200, 100),
                DrawGameOverWindow,
                sessionType == SessionType.AllVsAll
                    ? "Session Over!" : "Game Over!");
        }
    }

    // Draw window contents
    private void DrawOkLetsStartWindow(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {
            // Draw OK button
            if (GUI.Button(new Rect(50, 40, 100, 30), "Start Game"))
            {
                // If button is clicked, start game
                StartGame();
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
            if (GUI.Button(new Rect(50, 40, 140, 30), nextPlayerA.PlayerName))
            {
                // No need to swap players, just start the game
                StartGame();
            }
            if (GUI.Button(new Rect(200, 40, 140, 30), nextPlayerB.PlayerName))
            {
                // Swap players...
                IPlayer aux = nextPlayerA;
                nextPlayerA = nextPlayerB;
                nextPlayerB = aux;
                // ...and then start game
                StartGame();
            }
        }
    }

    // Draw window contents
    private void DrawCompetitionWindow(int id)
    {
        // Is this the correct window?
        if (id == 2)
        {
            // Draw OK button
            if (GUI.Button(new Rect(50, 40, 140, 30), "Not implemented yet"))
            {
                throw new NotImplementedException(
                    "All vs All not implemented yet");
            }
        }
    }

    // Draw window contents
    private void DrawGameOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 3)
        {
            // Draw OK button
            if (GUI.Button(new Rect(50, 40, 100, 30), "OK"))
            {
                // If button is clicked, exit
                Destroy(gameInstance);
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    private void EndCurrentGame()
    {
        // TODO Consider all vs all situation
        status = Status.Finish;
    }
}
