﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private GameObject whiteRoundPiece = null;
    [SerializeField] private GameObject whiteSquarePiece = null;
    [SerializeField] private GameObject redRoundPiece = null;
    [SerializeField] private GameObject redSquarePiece = null;
    [SerializeField] private GameObject pole = null;
    [SerializeField] private GameObject ground = null;
    [SerializeField] private GameObject arrowButton = null;

    private Board board;
    private IReadOnlyList<IPlayer> players;
    private bool[] enabledPlayers;
    private PShape[] selectedShapes;
    private GameObject[,] pieces;
    private UIArrow[] uiArrows;

    private readonly float polePadding = 0.1f;
    private Vector2 leftPoleBase;
    private float distBtwPoles;
    private float totalHeightForPieces;
    private float piecesLength;
    private float piecesScale;

    private bool setupDone = false;

    internal void SetupView(
        Board board, IReadOnlyList<IPlayer> players)
    {
        if (setupDone)
            throw new InvalidOperationException(
                "Game view setup can only be performed once");

        selectedShapes = new PShape[] { PShape.Round, PShape.Round };
        enabledPlayers = new bool[] { players[0].IsHuman, false };

        this.players = players;
        this.board = board;

        setupDone = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!setupDone)
            throw new InvalidOperationException(
                "Game view setup needs to be performed before Start()");

        // Instantiate ground
        GameObject groundInst = Instantiate(ground, transform);

        // Determine where ground starts, since everything will be placed with
        // respect to the ground
        Bounds gBounds = groundInst.GetComponent<SpriteRenderer>().bounds;
        Vector3 gTopLeft = new Vector3(gBounds.min.x, gBounds.max.y, 0);

        // Get pole bounds
        Bounds plBounds = pole.GetComponent<SpriteRenderer>().bounds;

        // Get arrow bounds
        Bounds aBounds = arrowButton.GetComponent<SpriteRenderer>().bounds;

        // Get piece bounds (any will do)
        Bounds pcBounds = redRoundPiece.GetComponent<SpriteRenderer>().bounds;

        // Create matrix for placing game objects representing pieces
        pieces = new GameObject[board.rows, board.cols];

        // Create array for UI arrow script objects
        uiArrows = new UIArrow[board.cols];

        // Instantiate poles and arrows
        for (int c = 0; c < board.cols; c++)
        {
            GameObject currPole, currArrow;

            // Instantiate current pole
            currPole = Instantiate(
                pole,
                new Vector3(
                    gTopLeft.x + (c + 1) * (gBounds.size.x / (board.cols + 1)),
                    gTopLeft.y + plBounds.extents.y - polePadding,
                    1),
                Quaternion.identity,
                transform);
            currPole.name = $"Pole{c}";

            // Instantiate current arrow
            currArrow = Instantiate(
                arrowButton,
                new Vector3(
                    gTopLeft.x + (c + 1) * (gBounds.size.x / (board.cols + 1)),
                    gTopLeft.y + plBounds.size.y + polePadding + aBounds.extents.y,
                    4),
                Quaternion.identity,
                transform);
            currArrow.name = $"Arrow{c}";

            // Keep reference to the UI arrow script
            uiArrows[c] = currArrow.GetComponent<UIArrow>();

            // Set the arrow's column
            uiArrows[c].Column = c;

            // Make the controller listen to arrow clicks
            uiArrows[c].Click.AddListener(OnMoveSelected);
        }

        // These will be necessary for calculating the positions of the pieces
        leftPoleBase = new Vector2(
            gTopLeft.x + gBounds.size.x / (board.cols + 1),
            gTopLeft.y);
        distBtwPoles = gBounds.size.x / (board.cols + 1);
        totalHeightForPieces = plBounds.size.y - 2 * polePadding;

        // The scale of the pieces will the minimum between...
        piecesScale = Mathf.Min(
            // ...half of distance between the poles divided by the original
            // width of the prefabs...
            (distBtwPoles / 2) / pcBounds.size.x,
            // ...and the available space for each piece in a pole, divided by
            // the original height of the prefabs
            (totalHeightForPieces / board.rows) / pcBounds.size.y);

        // Keep the length of the pieces (equal in x and y directions)
        piecesLength = piecesScale * pcBounds.size.y;
    }

    // Update a position in the board shown on screen
    internal void UpdateBoard(Move move)
    {
        // Is the screen board position empty and the game board has a piece?
        if (pieces[move.row, move.col] == null
            && board[move.row, move.col].HasValue)
        {
            // Then also place that piece in the screen board

            // Reference to the piece prefab
            GameObject piecePrefab;

            // The piece on the game board to also put in the screen board
            Piece piece = board[move.row, move.col].Value;

            // Determine the piece prefab to use based on the board piece
            if (piece.Is(PColor.White, PShape.Round))
                piecePrefab = whiteRoundPiece;
            else if (piece.Is(PColor.White, PShape.Square))
                piecePrefab = whiteSquarePiece;
            else if (piece.Is(PColor.Red, PShape.Round))
                piecePrefab = redRoundPiece;
            else if (piece.Is(PColor.Red, PShape.Square))
                piecePrefab = redSquarePiece;
            else
                throw new InvalidOperationException(
                    "Trying to instantiate an invalid piece");

            // Instantiate the screen piece
            pieces[move.row, move.col] = Instantiate(
                piecePrefab,
                new Vector3(
                    // Horizontal position
                    leftPoleBase.x + move.col * distBtwPoles,
                    // Vertical position
                    leftPoleBase.y
                        + move.row * (totalHeightForPieces / board.rows)
                        + piecesLength / 2,
                    // Z-axis
                    2),
                Quaternion.identity,
                transform);

            // Correct scale of screen piece
            pieces[move.row, move.col].transform.localScale =
                piecesScale * Vector3.one;

            // Is the column now full?
            if (board.IsColumnFull(move.col))
            {
                // If so, close the arrow
                uiArrows[move.col].Open = false;
            }
        }
        // Or is the screen board position occupied while the game board
        // position is empty?
        else if (pieces[move.row, move.col] != null
            && !board[move.row, move.col].HasValue)
        {
            // In such case, destroy the screen board piece
            Destroy(pieces[move.row, move.col]);
            pieces[move.row, move.col] = null;

            // Open the arrow
            uiArrows[move.col].Open = true;
        }
        // Otherwise it's an impossible situation and we have a bug
        else
        {
            throw new InvalidOperationException(
                "Board view representation not in sync with board model");
        }

        // Disable previous player and update its shape choice
        enabledPlayers[(int)move.piece.color] = false;
        selectedShapes[(int)move.piece.color] = move.piece.shape;

        // Enable next player if human
        enabledPlayers[(int)board.Turn] = players[(int)board.Turn].IsHuman;
    }

    private void OnGUI()
    {
        DrawPlayerPanel(PColor.White);
        DrawPlayerPanel(PColor.Red);
    }

    private void DrawPlayerPanel(PColor player)
    {
        float boxWidth = 150;
        float boxHeight = 100;
        float distFromSide = 20;

        float posFromLeft =
            player == PColor.White
                ? distFromSide
                : Screen.width - boxWidth - distFromSide;

        PShape selectedShape = selectedShapes[(int)player];
        string playerName = players[(int)player].PlayerName;
        bool uiEnabled = enabledPlayers[(int)player];

        GUI.Box(
            new Rect(
                posFromLeft,
                Screen.height / 2 - boxHeight / 2,
                boxWidth, boxHeight),
            playerName);

        selectedShapes[(int)player] = (PShape)GUI.SelectionGrid(
            new Rect (
                posFromLeft + 10,
                Screen.height / 2 - boxHeight / 2 + 20,
                boxWidth - 20,
                boxHeight - 40),
            (int)selectedShape,
            new string[] { "Round", "Square" },
            1);
    }

    private void OnMoveSelected(int col)
    {
        MoveSelected?.Invoke(
            new FutureMove(col, selectedShapes[(int)board.Turn]));
    }

    public event Action<FutureMove> MoveSelected;
}