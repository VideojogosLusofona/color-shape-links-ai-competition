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
    private enum Status { Init, InGame, BtwGames, Finish }
    private enum SessionType { HumanVsHuman, PlayerVsPlayer, AllVsAll }

    [SerializeField] private GameObject gamePrefab = null;
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 7;
    [SerializeField] private int winSequence = 4;
    [SerializeField] private int squarePiecesPerPlayer = 11;
    [SerializeField] private int roundPiecesPerPlayer = 10;

    [Tooltip("Maximum real time that AI can take to play")]
    [SerializeField] private float aITimeLimit = 0.5f;

    [Tooltip("Even if the AI plays immediately, this time gives the "
        + "illusion that the AI took some minimum time to play")]
    [SerializeField] private float minAIGameMoveTime = 0.25f;

    private IPlayer[] players;
    private Board board;

    private GameObject gameInstance = null;
    private GameController gameController = null;

    private Status status;
    private SessionType sessionType;

    private IList<AIPlayer> activeAIs = null;

    private IPlayer humanPlayer;

    private void Awake()
    {
        List<AIPlayer> allAIs = new List<AIPlayer>();
        GetComponents(allAIs);
        activeAIs = allAIs.FindAll(ai => ai.IsActive);
        status = Status.Init;
        humanPlayer = new HumanPlayer();

        players = new IPlayer[2];

        board = new Board(rows, cols, winSequence,
            roundPiecesPerPlayer, squarePiecesPerPlayer);
    }

    private void Start()
    {
        if (activeAIs.Count == 0)
        {
            // A game between human players, ask user to press OK to start
            players[(int)PColor.White] = humanPlayer;
            players[(int)PColor.Red] = humanPlayer;
            sessionType = SessionType.HumanVsHuman;
        }
        else if (activeAIs.Count == 1)
        {
            // A game between a human and an AI, ask who plays first
            players[(int)PColor.White] = humanPlayer;
            players[(int)PColor.Red] = activeAIs[0];
            sessionType = SessionType.PlayerVsPlayer;
        }
        else if (activeAIs.Count == 2)
        {
            // A game between two AIs, ask who plays first
            players[(int)PColor.White] = activeAIs[0];
            players[(int)PColor.Red] = activeAIs[1];
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
        gameInstance = Instantiate(gamePrefab, transform);
        gameInstance.name = "Game";
        gameController = gameInstance.GetComponent<GameController>();
        gameController.GameOver.AddListener(EndCurrentGame);
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
                    DrawGameOverWindow,
                    "Game Over!");
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
                "Start Game"))
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
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 150,
                    Screen.height / 2 - 25,
                    140,
                    50),
                players[0].PlayerName))
            {
                // No need to swap players, just start the game
                StartGame();
            }
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 + 10,
                    Screen.height / 2 - 25,
                    140,
                    50),
                players[1].PlayerName))
            {
                // Swap players...
                IPlayer aux = players[0];
                players[0] = players[1];
                players[1] = aux;
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
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                "Not implemented yet"))
            {
                throw new NotImplementedException(
                    "All vs All not implemented yet");
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
                Destroy(gameInstance);
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }


    // Draw window contents
    private void DrawGameOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 4)
        {
            string winStr =
                gameController.Result != Winner.Draw
                    ? gameController.PlrNameColor(
                        gameController.Result.ToPColor())
                    : "";
            int winW = Screen.width * 2 / 3;
            int winH = Screen.height * 2 / 3;
            Color originalColor = GUI.contentColor;
            Color color = gameController.Result == Winner.Draw
                ? Color.yellow
                : gameController.Result == Winner.White
                    ? Color.white
                    : Color.red;
            GUIStyle guiLabelStyle = new GUIStyle(GUI.skin.label);
            guiLabelStyle.alignment = TextAnchor.MiddleCenter;
            guiLabelStyle.fontSize = Screen.width / 30;
            GUI.contentColor = color;
            GUI.Label(
                new Rect(
                    winW / 2 - winW / 3,
                    winH / 2 - winH / 3,
                    winW * 2 / 3,
                    winH * 2 / 3),
                gameController.Result == Winner.Draw
                    ? "It's a draw"
                    : $"Winner is {winStr}",
                guiLabelStyle);
            // Draw OK button
            GUI.contentColor = originalColor;
            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50),
                "OK"))
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


    // Implementation of ISessionDataProvider
    public Board Board => board;
    public IPlayer CurrentPlayer => players[(int)board.Turn];
    public float AITimeLimit => aITimeLimit;
    public float TimeBetweenAIMoves => minAIGameMoveTime;
    public IPlayer GetPlayer(PColor player) => players[(int)player];
}
