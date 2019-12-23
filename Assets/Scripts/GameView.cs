/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private GameObject whiteRoundPiece = null;
    [SerializeField] private GameObject whiteSquarePiece = null;
    [SerializeField] private GameObject redRoundPiece = null;
    [SerializeField] private GameObject redSquarePiece = null;
    [SerializeField] private GameObject pole = null;
    [SerializeField] private GameObject ground = null;
    [SerializeField] private GameObject arrowButton = null;
    [SerializeField] private GameObject playerPanel = null;

    private Board board;
    private ISessionDataProvider sessionData;
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

    private void Awake()
    {
        sessionData = GetComponentInParent<ISessionDataProvider>();
        board = sessionData.Board;
        selectedShapes = new PShape[] { PShape.Round, PShape.Round };
        enabledPlayers = new bool[] {
                sessionData.CurrentPlayer.IsHuman, false };

        ShapeSelected = new ColorShapeEvent();
        BoardUpdated = new UnityEvent();

        // Create matrix for placing game objects representing pieces
        pieces = new GameObject[board.rows, board.cols];

        // Create array for UI arrow script objects
        uiArrows = new UIArrow[board.cols];

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

        //
        // Setup player panels
        //
        GameObject[] playerPanels = {
            Instantiate(playerPanel, transform),
            Instantiate(playerPanel, transform) };
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        Sprite[,] pieceSprites = {
            {
                whiteRoundPiece.GetComponent<SpriteRenderer>().sprite,
                whiteSquarePiece.GetComponent<SpriteRenderer>().sprite,
            },
            {
                redRoundPiece.GetComponent<SpriteRenderer>().sprite,
                redSquarePiece.GetComponent<SpriteRenderer>().sprite,
            }
        };

        for (int i = 0; i < 2; i++)
        {
            // Current player
            PColor player = (PColor)i;
            // Get current panel
            GameObject panel = playerPanels[i];
            // Get current panel's rect transform
            RectTransform rtPanel = panel.GetComponent<RectTransform>();
            // Get current panel toggles for selecting shape
            Toggle[] toggles = panel.GetComponentsInChildren<Toggle>();
            // Setup event camera in panel
            panel.GetComponent<Canvas>().worldCamera = camera;
            // Position panel
            rtPanel.position = new Vector3(
                i == 0
                    // First panel to the right
                    ? gBounds.center.x - gBounds.extents.x / 2
                    // Second panel to the left
                    : gBounds.center.x + gBounds.extents.x / 2,
                gBounds.min.y - rtPanel.rect.height / 2 * rtPanel.localScale.y,
                rtPanel.position.z
            );
            // Set player name in panel
            panel.GetComponentInChildren<Text>().text =
                sessionData.GetPlayer(player).PlayerName;

            // Configure toggles for selecting shape
            for (int j = 0; j < 2; j++)
            {
                // Current shape
                PShape shape = (PShape)j;

                // Get current toggle's label for counting pieces and set it
                // to current piece count
                Text tLabel =
                    toggles[(int)shape].GetComponentInChildren<Text>();
                tLabel.text = board.PieceCount(player, shape).ToString();

                // Wire up method for listening to piece swap events
                toggles[j].onValueChanged.AddListener(
                    b => { if (b) SelectShape(player, shape); });

                // If player not human, disable toggle interaction
                if (!sessionData.GetPlayer(player).IsHuman)
                    toggles[j].interactable = false;

                // Setup correct sprite for the toggle
                toggles[j].transform.GetChild(1).GetComponent<Image>().sprite =
                    pieceSprites[i, j];

                // Wire up listener for programatically changing piece count in
                // current player's toggle UI widget
                BoardUpdated.AddListener(() => {
                    int count = board.PieceCount(player, shape);
                    tLabel.text = count.ToString();
                    if (count == 0)
                    {
                        SelectShape(
                            player,
                            shape == PShape.Round
                                ? PShape.Square
                                : PShape.Round );
                        toggles[(int)shape].interactable = false;
                    }
                });
            }

            // Wire up listener for programatically changing selected shape in
            // current player's toggle
            ShapeSelected.AddListener((PColor p, PShape s) =>
                { if (p == player) toggles[(int)s].isOn = true; });
        }
    }

    private void SelectShape(PColor player, PShape shape)
    {
        selectedShapes[(int)player] = shape;
        ShapeSelected.Invoke(player, shape);
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
        SelectShape(move.piece.color, move.piece.shape);

        // Enable next player if human
        enabledPlayers[(int)board.Turn] =
            sessionData.CurrentPlayer.IsHuman;

        // Notify listeners that board was updated
        BoardUpdated.Invoke();
    }

    private void OnMoveSelected(int col)
    {
        MoveSelected?.Invoke(
            new FutureMove(col, selectedShapes[(int)board.Turn]));
    }

    [Serializable]
    private class ColorShapeEvent : UnityEvent<PColor, PShape> {}

    private UnityEvent BoardUpdated;
    private ColorShapeEvent ShapeSelected;
    public event Action<FutureMove> MoveSelected;
}
