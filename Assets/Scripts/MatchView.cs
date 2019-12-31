/// @file
/// @brief This file contains the ::MatchView class.
///
/// @author Nuno Fachada
/// @date 2019
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Script which renders the matches UI.
/// </summary>
/// <remarks>
/// Based on the MVC design pattern, composed in this case by the following
/// classes:
/// * *Model* - <see cref="Board"/>.
/// * *View* - This class.
/// * *Controller* - <see cref="MatchController"/>.
/// </remarks>
public class MatchView : MonoBehaviour
{

    // Prefabs required for building the UI
    private GameObject whiteRoundPiece = null;
    private GameObject whiteSquarePiece = null;
    private GameObject redRoundPiece = null;
    private GameObject redSquarePiece = null;
    private GameObject pole = null;
    private GameObject ground = null;
    private GameObject arrowButton = null;
    private GameObject playerPanel = null;

    // Reference to the game board
    private Board board;

    // Reference to the match data provider
    private IMatchDataProvider matchData;

    // The shapes selected by each player
    private PShape[] selectedShapes;

    // UI representation of the game board
    private GameObject[,] pieces;

    // Buttons/arrows for humans to select their next move
    private HumanMoveButton[] uiArrows;

    // Last move animation length in seconds
    private float lastMoveAnimLength;

    // Reference to the UI messages box
    private Text messageBoxText;

    // Queue of messages not yet shown
    private Queue<string> messageQueue;

    // Contains all the messages shown so far
    private StringBuilder messages;

    // Auxiliary variables for positioning the UI elements
    private Vector2 leftPoleBase;
    private float distBtwPoles;
    private float totalHeightForPieces;
    private float piecesLength;
    private float piecesScale;
    private bool finished;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Top and bottom pole padding
        const float polePadding = 0.1f;

        // Ground instance
        GameObject groundInst, messageBox;

        // Top-left of ground sprite renderer
        Vector3 gTopLeft;

        // Bounds of different sprite renderers
        Bounds gBounds, plBounds, aBounds, pcBounds;

        // Get reference to the camera
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        // /////////////////////////////////// //
        // Get references to essential prefabs //
        // /////////////////////////////////// //

        whiteRoundPiece = Resources.Load<GameObject>("Prefabs/PieceWhiteRound");
        whiteSquarePiece = Resources.Load<GameObject>("Prefabs/PieceWhiteSquare");
        redRoundPiece = Resources.Load<GameObject>("Prefabs/PieceRedRound");
        redSquarePiece = Resources.Load<GameObject>("Prefabs/PieceRedSquare");
        pole = Resources.Load<GameObject>("Prefabs/Pole");
        ground = Resources.Load<GameObject>("Prefabs/Ground");
        arrowButton = Resources.Load<GameObject>("Prefabs/ArrowButton");
        playerPanel = Resources.Load<GameObject>("Prefabs/PlayerPanel");

        // //////////////////////////////////////////// //
        // Initialize required variables and coroutines //
        // //////////////////////////////////////////// //

        // Instantiate a string builder, used for keeping messages
        messages = new StringBuilder();

        // We just started, so game is not finished yet
        finished = false;

        // Get reference to the session data and the game board
        matchData = GetComponentInParent<IMatchDataProvider>();
        board = matchData.Board;

        // Get value of last move animation length in seconds
        lastMoveAnimLength = matchData.LastMoveAnimLength;

        // Both players have the round shapes initially selected by default
        selectedShapes = new PShape[] { PShape.Round, PShape.Round };

        // Instantiate Unity events for shape selection and board updating
        ShapeSelected = new ColorShapeEvent();
        BoardUpdated = new UnityEvent();

        // Create matrix for placing game objects representing pieces
        pieces = new GameObject[board.rows, board.cols];

        // Create array for UI arrow script objects
        uiArrows = new HumanMoveButton[board.cols];

        // Instantiate the message queue
        messageQueue = new Queue<string>();

        // Get reference to the "Message Box" canvas game object, set the
        // reference to the main camera, and get a reference to the UI text to
        // display the messages
        messageBox = GameObject.Find("MessageBox").gameObject;
        messageBox.GetComponent<Canvas>().worldCamera = camera;
        messageBoxText = messageBox.GetComponentInChildren<Text>();

        // Initialize message box coroutine
        StartCoroutine(UpdateMessageBox());

        // /////////////////////////////////////// //
        // Initialize and place game board objects //
        // /////////////////////////////////////// //

        // Instantiate ground
        groundInst = Instantiate(ground, transform);

        // Determine where ground starts, since everything will be placed with
        // respect to the ground
        gBounds = groundInst.GetComponent<SpriteRenderer>().bounds;
        gTopLeft = new Vector3(gBounds.min.x, gBounds.max.y, 0);

        // Get pole bounds
        plBounds = pole.GetComponent<SpriteRenderer>().bounds;

        // Get arrow bounds
        aBounds = arrowButton.GetComponent<SpriteRenderer>().bounds;

        // Get piece bounds (any will do)
        pcBounds = redRoundPiece.GetComponent<SpriteRenderer>().bounds;

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
            uiArrows[c] = currArrow.GetComponent<HumanMoveButton>();

            // Set the arrow's column
            uiArrows[c].Column = c;

            // Listen to arrow clicks in order to perform move selection
            uiArrows[c].Click.AddListener(OnMoveSelected);

            // Enable or disable arrow depending on who's playing
            currArrow.SetActive(matchData.CurrentPlayer.IsHuman);
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

        // /////////////////// //
        // Setup player panels //
        // /////////////////// //

        // Instantiate player panels
        GameObject[] playerPanels = {
            Instantiate(playerPanel, transform),
            Instantiate(playerPanel, transform) };

        // Initialize an array of piece sprites, which will simplify
        // passing the correct sprite for each shape in each player panel
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

        // Initialize panels for each player
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
                matchData.GetPlayer(player).PlayerName;

            // Configure toggles for selecting shape
            for (int j = 0; j < 2; j++)
            {
                // Current shape
                PShape shape = (PShape)j;

                // Setup correct sprite for the toggle
                toggles[j].transform.GetChild(1).GetComponent<Image>().sprite =
                    pieceSprites[i, j];

                // Delegate to be called at start and after each move, which:
                // 1. Updates piece count in current player's toggle UI widget
                // 2. Enables/disables toggle interaction depending on who's
                // playing
                UnityAction setToggles = () =>
                {
                    // 1.
                    // Count for current player and shape
                    int count = board.PieceCount(player, shape);
                    // Update label to update with shape count
                    toggles[(int)shape].GetComponentInChildren<Text>().text =
                        count.ToString();
                    // If count reached zero, swap shape selection
                    if (count == 0)
                    {
                        SelectShape(
                            player,
                            shape == PShape.Round
                                ? PShape.Square
                                : PShape.Round);
                        toggles[(int)shape].interactable = false;
                    }
                    // 2.
                    else
                    {
                        // Player is human, is its turn and game not over,
                        // enable toggle
                        if (matchData.GetPlayer(player).IsHuman
                            && player == board.Turn && !finished)
                        {
                            toggles[(int)shape].interactable = true;
                        }
                        // Otherwise disable toggle
                        else
                        {
                            toggles[(int)shape].interactable = false;
                        }
                    }
                };

                // Invoke delegate to initialize toggles
                setToggles.Invoke();

                // Make this delegate be called after each move
                BoardUpdated.AddListener(setToggles);

                // Wire up method for listening to piece swap events
                toggles[j].onValueChanged.AddListener(
                    b => { if (b) SelectShape(player, shape); });
            }

            // Wire up listener for programatically changing selected shape in
            // current player's toggle
            ShapeSelected.AddListener((PColor p, PShape s) =>
                { if (p == player) toggles[(int)s].isOn = true; });
        }
    }

    // Make shape selection visible in the UI
    private void SelectShape(PColor player, PShape shape)
    {
        // Keep the currently selected shape
        selectedShapes[(int)player] = shape;
        // Update UI widgets which depend on the shape selection
        ShapeSelected.Invoke(player, shape);
    }

    // Co-routine which animates last move
    private IEnumerator AnimateLastMove(Move lastMove)
    {
        // Keep track of time which animation started and time that it should
        // end
        float timeStarted = Time.time;
        float timeToEnd = timeStarted + lastMoveAnimLength;
        // Get the game object to animate
        GameObject piece = pieces[lastMove.row, lastMove.col];
        // Get the sprite renderer to fade in and out
        SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
        // Original color of the sprite renderer
        Color color = spriteRenderer.color;

        // Animate while we have time, avoiding sudden breaks in the animation
        while (Time.time < timeToEnd || color.a < 0.95f)
        {
            color.a = Mathf.Cos(Time.time * Mathf.PI * 2 / lastMoveAnimLength)
                * 1f / 2f + 1f / 2f;
            spriteRenderer.color = color;

            // See you next frame
            yield return null;
        }
        // Make sure piece shows when coroutine finishes
        color.a = 1f;
        spriteRenderer.color = color;
    }

    // Update a position in the board shown on screen
    internal void UpdateBoard(Move move, Winner result, Pos[] solution)
    {
        // Update finished flag
        this.finished = result != Winner.None;

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

            // Animation last move
            StartCoroutine(AnimateLastMove(move));
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

        // Update previous player shape choice
        SelectShape(move.piece.color, move.piece.shape);

        // Disable or enable GUI stuff depending on who's playing next
        foreach (HumanMoveButton arrow in uiArrows)
        {
            arrow.gameObject.SetActive(
                !finished && matchData.CurrentPlayer.IsHuman);
        }

        // Notify listeners that board was updated
        BoardUpdated.Invoke();

        // If match is finished and result is not a draw, draw a line marking
        // the solution
        if (result == Winner.Red || result == Winner.White)
        {
            // The variable where we'll place the line renderer
            LineRenderer linRend;

            // Game object which will hold the line renderer
            GameObject winLine = new GameObject("WinLine");

            // Determine initial and final line positions
            Vector3 start =
                pieces[solution[0].row, solution[0].col].transform.position;
            Vector3 end = pieces[
                solution[solution.Length - 1].row,
                solution[solution.Length - 1].col]
                    .transform.position;
            start.z = -2;
            end.z = -2;

            // Set the UI as the parent of the line game object
            winLine.transform.SetParent(transform);

            // Position the game object which will hold the line renderer
            winLine.transform.position = start;

            // Add the line renderer to the game object and retrieve it
            winLine.AddComponent<LineRenderer>();
            linRend = winLine.GetComponent<LineRenderer>();

            // Set line material (use same shader as used for sprites)
            linRend.material = new Material(Shader.Find("Sprites/Default"));

            // Set line width
            linRend.startWidth = 0.3f;
            linRend.endWidth = 0.3f;

            // Set line color
            linRend.startColor = new Color(1f, 0f, 0f, 0.75f);
            linRend.endColor = new Color(1f, 0f, 0f, 0.75f);

            // Set line position
            linRend.SetPosition(0, start);
            linRend.SetPosition(1, end);

            // Specify that positions are in world space
            linRend.useWorldSpace = true;

            // Make sure line appears in the correct sorting order
            linRend.sortingOrder = 5;
        }
    }

    // Method that invokes event indicating that a move was selected via the UI
    private void OnMoveSelected(int col)
    {
        MoveSelected?.Invoke(
            new FutureMove(col, selectedShapes[(int)board.Turn]));
    }

    /// <summary>
    /// Submit a message which will be shown in the message box when possible.
    /// However, message will be shown in the Unity console immediately.
    /// </summary>
    /// <param name="str">Message to show in the message box.</param>
    internal void SubmitMessage(string str)
    {
        messageQueue.Enqueue(str);
        Debug.Log(str);
    }

    // Present messages with some delay between them
    private IEnumerator UpdateMessageBox()
    {
        // This coroutine will be called at least once per AI move
        YieldInstruction timeAImoves = new WaitForSeconds(
            matchData.MinAIGameMoveTime);

        // When there are more messages, the coroutine is called in a tenth
        // of that time, giving the illusion of scrolling
        YieldInstruction minTimMsgs = new WaitForSeconds(
            matchData.MinAIGameMoveTime / 10);

        // Enter the infinite loop
        while (true)
        {
            // Are there messages in the queue?
            if (messageQueue.Count > 0)
            {
                // If so, let's post them
                while (messageQueue.Count > 0)
                {
                    // Get message out of the queue and into the string builder
                    messages.Append($"\n- {messageQueue.Dequeue()}");
                    // Update the text UI widget
                    messageBoxText.text = messages.ToString();
                    // We'll return in a moment
                    yield return minTimMsgs;
                }
            }
            else
            {
                // If there are no messages in the queue, come back later
                yield return timeAImoves;
            }
        }
    }

    // Definition of a Unity event class which accepts a color and a shape
    [Serializable]
    private class ColorShapeEvent : UnityEvent<PColor, PShape> { }

    // Unity event which will be invoked when the board is updated
    private UnityEvent BoardUpdated;

    // Unity event which will be invoked when a shape is selected
    private ColorShapeEvent ShapeSelected;

    /// <summary>
    /// Native C# event which will be invoked when a move is selected in the UI.
    /// </summary>
    public event Action<FutureMove> MoveSelected;
}
