using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{


    [SerializeField] private GameObject whiteRoundPiece = null;
    [SerializeField] private GameObject whiteSquarePiece = null;
    [SerializeField] private GameObject redRoundPiece = null;
    [SerializeField] private GameObject redSquarePiece = null;
    [SerializeField] private GameObject pole = null;
    [SerializeField] private GameObject ground = null;

    private GameController gameController = null;
    private Board board = null;

    private void Awake()
    {
        gameController =
            GameObject.Find("Game")?.GetComponent<GameController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get a reference to the game board
        board = gameController.Board;

        // Instantiate ground
        Instantiate(ground);


        // Instantiate poles
        for (int c = 0; c < board.Cols; c++)
        {
            Instantiate(pole, new Vector2(-8 + c * (16f / (board.Cols - 1)), 0.1f), Quaternion.identity);
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
