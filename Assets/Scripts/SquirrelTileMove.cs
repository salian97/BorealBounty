using UnityEngine;
using UnityEngine.Tilemaps;

public class SquirrelSmoothGridMove : MonoBehaviour
{
    public Tilemap stumpTilemap;
    public float moveSpeed = 6f; // Tune for faster/slower movement

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // Snap squirrel to the center of current cell at start
        Vector3Int startCell = stumpTilemap.WorldToCell(transform.position);
        targetPosition = stumpTilemap.GetCellCenterWorld(startCell);
        transform.position = targetPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            // Move squirrel smoothly towards target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
                transform.position = targetPosition;
                isMoving = false;
            }
            return;
        }

        Vector3Int moveDir = Vector3Int.zero;
        if (Input.GetKey(KeyCode.W)) moveDir = Vector3Int.up;
        else if (Input.GetKey(KeyCode.S)) moveDir = Vector3Int.down;
        else if (Input.GetKey(KeyCode.A)) moveDir = Vector3Int.left;
        else if (Input.GetKey(KeyCode.D)) moveDir = Vector3Int.right;

        if (moveDir != Vector3Int.zero && !isMoving)
        {
            Vector3Int currCell = stumpTilemap.WorldToCell(transform.position);
            Vector3Int targetCell = currCell + moveDir;
            var tile = stumpTilemap.GetTile(targetCell);
            if (tile != null)
            {
                targetPosition = stumpTilemap.GetCellCenterWorld(targetCell);
                isMoving = true;
            }
        }
    }
}
