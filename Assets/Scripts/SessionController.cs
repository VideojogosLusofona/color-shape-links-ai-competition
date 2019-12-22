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

    private GameObject gameInstance = null;
    private GameController gameController = null;

    private Status status;
    private SessionType sessionType;

    private IList<AIPlayer> listOfAIs = null;

    private IPlayer nextPlayerA, nextPlayerB;

    private IPlayer humanPlayer;

    private void Awake()
    {
        status = Status.Init;
        listOfAIs = new List<AIPlayer>();
        // TODO Find AIs

        humanPlayer = new HumanPlayer();
    }

    private void Start()
    {
        if (listOfAIs.Count == 0)
        {
            // A game between human players, ask user to press OK to start
            nextPlayerA = humanPlayer;
            nextPlayerB = humanPlayer;
            sessionType = SessionType.HumanVsHuman;
        }
        else if (listOfAIs.Count == 1)
        {
            // A game between a human and an AI, ask who plays first
            nextPlayerA = humanPlayer;
            nextPlayerB = listOfAIs[0];
            sessionType = SessionType.PlayerVsPlayer;
        }
        else if (listOfAIs.Count == 2)
        {
            // A game between two AIs, ask who plays first
            nextPlayerA = listOfAIs[0];
            nextPlayerB = listOfAIs[1];
            sessionType = SessionType.PlayerVsPlayer;
        }
        else
        {
            // Multiple AIs, run a competition, show the list of AIs and
            // ask user to press OK to start
            sessionType = SessionType.AllVsAll;
        }
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
                throw new NotImplementedException(
                    "Player vs Player not implemented yet");
            }
            else if (sessionType == SessionType.AllVsAll)
            {
                // Show list and press OK to continue
                throw new NotImplementedException(
                    "All vs All not implemented yet");
            }
        }
        else if (status == Status.Finish)
        {
            GUI.Window(0,
                new Rect(
                    Screen.width / 2 - 100,
                    Screen.height / 2 - 50, 200, 100),
                DrawGameOverWindow,
                sessionType == SessionType.AllVsAll
                    ? "Session Over!" : "Game Over!");
        }
    }

    // Draw window contents
    private void DrawGameOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {
            // Draw OK button
            if (GUI.Button(new Rect(50, 40, 100, 30), "OK"))
            {
                // If button is clicked, exit
                UnityEditor.EditorApplication.isPlaying = false;
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
            if (GUI.Button(new Rect(50, 40, 100, 30), "Start Game"))
            {
                // If button is clicked, create game
                gameInstance = Instantiate(gamePrefab);
                gameInstance.name = "Game";
                gameController = gameInstance.GetComponent<GameController>();
                // TODO this should go to OnEnable
                gameController.GameOver += EndCurrentGame;


                status = Status.InGame;
            }
        }
    }

    private void EndCurrentGame()
    {
        // TODO Consider all vs all situation
        status = Status.Finish;
    }

}
