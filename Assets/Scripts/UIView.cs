/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField] private GameObject whiteRoundPiece = null;
    [SerializeField] private GameObject whiteSquarePiece = null;
    [SerializeField] private GameObject redRoundPiece = null;
    [SerializeField] private GameObject redSquarePiece = null;
    [SerializeField] private GameObject pole = null;
    [SerializeField] private GameObject ground = null;
    [SerializeField] private GameObject arrowButton = null;

    private GameController gameController = null;
    private Board board = null;
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
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get reference to the game controller
        gameController =
            GameObject.Find("Game")?.GetComponent<GameController>();
        gameController.BoardUpdate += UpdateBoard;

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

        // Get a reference to the game board
        board = gameController.Board;

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
            // TODO this should go to OnEnable
            uiArrows[c].Click += gameController.MakeAMove;
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
    private void UpdateBoard(int row, int col)
    {
        // Is the screen board position empty and the game board has a piece?
        if (pieces[row, col] == null && board[row, col].HasValue)
        {
            // Then also place that piece in the screen board

            // Reference to the piece prefab
            GameObject piecePrefab;

            // The piece on the game board to also put in the screen board
            Piece piece = board[row, col].Value;

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
            pieces[row, col] = Instantiate(
                piecePrefab,
                new Vector3(
                    // Horizontal position
                    leftPoleBase.x + col * distBtwPoles,
                    // Vertical position
                    leftPoleBase.y + row * (totalHeightForPieces / board.rows)
                         + piecesLength / 2,
                    // Z-axis
                    2),
                Quaternion.identity,
                transform);

            // Correct scale of screen piece
            pieces[row, col].transform.localScale = piecesScale * Vector3.one;

            // Is the column now full?
            if (board.IsColumnFull(col))
            {
                // If so, close the arrow
                uiArrows[col].Open = false;
            }
        }
        // Or is the screen board position occupied while the game board
        // position is empty?
        else if (pieces[row, col] != null && !board[row, col].HasValue)
        {
            // In such case, destroy the screen board piece
            Destroy(pieces[row, col]);
            pieces[row, col] = null;

            // Open the arrow
            uiArrows[col].Open = true;
        }
        // Otherwise it's an impossible situation and we have a bug
        else
        {
            throw new InvalidOperationException(
                "Board view representation not in sync with board model");
        }
    }
}
