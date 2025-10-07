using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BoardManager boardManager; // reference to the board
    public float tileSize = 1f;

    private int x, y; // current grid coordinates
    private Transform boardTransform;

    void Start()
    {
        x = GetComponent<Piece>().x;
        y = GetComponent<Piece>().y;
        boardTransform = boardManager.transform;
    }

    void Update()
    {
        int moveX = 0;
        int moveY = 0;

        // Input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveY = 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveY = -1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveX = 1;

        // Only move if there’s input
        if (moveX != 0 || moveY != 0)
        {
            TryMove(moveX, moveY);
        }
    }

    void TryMove(int moveX, int moveY)
    {
        int targetX = x + moveX;
        int targetY = y + moveY;

        // Clamp to board boundaries
        if (targetX < 0 || targetX >= boardManager.boardSize) return;
        if (targetY < 0 || targetY >= boardManager.boardSize) return;

        // Move player
        x = targetX;
        y = targetY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);

        // Update Piece component
        Piece piece = GetComponent<Piece>();
        piece.x = x;
        piece.y = y;
    }
}
