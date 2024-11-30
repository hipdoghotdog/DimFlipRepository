using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public Animator playerAnimator;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    public bool isMoving = false;

    // Movement queue for handling step-by-step movement
    private Queue<Vector3> movementQueue = new Queue<Vector3>();

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (!isMoving)
        {
            HandleInput();
        }
        else
        {
            MovePlayer();
        }
    }

    public bool CanIStepOnBlock(Vector3 position)
    {
        string blockType = gameManager.GetBlock(position).GetBlockType();
        return !gameManager.lb.GetUnsteppableBlocks().Contains(blockType);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (TryMoveHorizontal(1, Quaternion.Euler(0, 0, 0)))
                return;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (TryMoveHorizontal(-1, Quaternion.Euler(0, 180, 0)))
                return;
        }
        else if (gameManager.currentView == View.TopdownView)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (TryMoveVertical(1, Quaternion.Euler(0, -90, 0)))
                    return;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (TryMoveVertical(-1, Quaternion.Euler(0, 90, 0)))
                    return;
            }
        }
    }

    bool TryMoveHorizontal(int xIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;

        int maxX = gameManager.current_level.GetLength(0) - 1;
        if ((xIncrement > 0 && currentPosition.x >= maxX) || (xIncrement < 0 && currentPosition.x <= 0))
            return false;

        if (gameManager.currentView == View.TopdownView)
        {
            Vector3 targetPos = currentPosition + new Vector3(xIncrement, 0, 0);
            if (CanIStepOnBlock(targetPos))
            {
                StartMovement(targetPos, desiredRotation);
                return true;
            }
        }
        else
        {
            if (TrySideViewMovement(xIncrement, desiredRotation))
                return true;
        }
        return false;
    }

    bool TrySideViewMovement(int xIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;

        Vector3 toPosUp = currentPosition + new Vector3(xIncrement, 1, 0);
        Vector3 toPosDown = currentPosition + new Vector3(xIncrement, -1, 0);
        Vector3 toPosSame = currentPosition + new Vector3(xIncrement, 0, 0);

        int maxY = gameManager.current_level.GetLength(1) - 1;

        // Check for climbing up
        if (currentPosition.y < maxY && CanIStepOnBlock(toPosUp) && CanIUseLadder(currentPosition, toPosUp))
        {
            newPosition += new Vector3(xIncrement, 1, 0);
            StartMovement(newPosition, desiredRotation, isDownwardLadderMovement: false); // Moving up
            return true;
        }
        // Check for climbing down
        else if (currentPosition.y > 0 && CanIStepOnBlock(toPosDown) && CanIUseLadder(currentPosition, toPosDown))
        {
            newPosition += new Vector3(xIncrement, -1, 0);
            StartMovement(newPosition, desiredRotation, isDownwardLadderMovement: true); // Moving down
            return true;
        }
        // Regular horizontal movement
        else if (CanIStepOnBlock(toPosSame) && (currentPosition.y == maxY || !CanIStepOnBlock(toPosSame + Vector3.up)))
        {
            newPosition += new Vector3(xIncrement, 0, 0);
            StartMovement(newPosition, desiredRotation);
            return true;
        }
        return false;
    }

    bool TryMoveVertical(int zIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;

        int maxZ = gameManager.current_level.GetLength(2) - 1;
        if ((zIncrement > 0 && currentPosition.z >= maxZ) || (zIncrement < 0 && currentPosition.z <= 0))
            return false;

        Vector3 targetPos = currentPosition + new Vector3(0, 0, zIncrement);
        if (CanIStepOnBlock(targetPos))
        {
            StartMovement(targetPos, desiredRotation);
            return true;
        }
        return false;
    }
    void StartMovement(Vector3 newPosition, Quaternion desiredRotation, bool isDownwardLadderMovement = false)
    {
        if (isDownwardLadderMovement)
        {
            movementQueue.Clear();

            Vector3 currentPosition = transform.position;
            Vector3 delta = newPosition - currentPosition;

            if (delta.x != 0 && delta.y != 0)
            {
                Vector3 intermediatePos = new Vector3(currentPosition.x + delta.x, currentPosition.y, currentPosition.z); 
                movementQueue.Enqueue(intermediatePos);
                movementQueue.Enqueue(newPosition); 
            }
            else
            {
                movementQueue.Enqueue(newPosition);
            }

            targetPosition = movementQueue.Dequeue();
            isMoving = true;
            playerAnimator.SetBool("isWalking", true);
        }
        else
        {
            movementQueue.Clear();
            movementQueue.Enqueue(newPosition);

            targetPosition = movementQueue.Dequeue();
            isMoving = true;
            playerAnimator.SetBool("isWalking", true);
        }

        // Handle rotation
        float angleDifference = Quaternion.Angle(transform.rotation, desiredRotation);
        if (angleDifference > 1f)
        {
            targetRotation = desiredRotation;
        }

        // Play step sound
        SoundManager.instance.PlaySound(Sound.STEP);
    }

    public void MovePlayer()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager reference is missing.");
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;

            if (movementQueue.Count > 0)
            {
                // Continue to the next position in the queue
                targetPosition = movementQueue.Dequeue();
            }
            else
            {
                // Movement complete
                isMoving = false;
                playerAnimator.SetBool("isWalking", false);

                // Check for level completion
                if (gameManager.GetBlock(transform.position).GetBlockType() == "end")
                {
                    gameManager.on_next_level(gameManager.lb.currentLevel);
                }
            }
        }
    }

    public void ResetPlayerPosition()
    {
        Vector3 startPosition = gameManager.lb.startBlockPosition;
        transform.position = startPosition;
        targetPosition = startPosition;
        isMoving = true;
        playerAnimator.SetBool("isWalking", true);
        targetRotation = Quaternion.identity;

        gameManager.cam.transform.position = new Vector3(
            gameManager.current_level.GetLength(0) / 2,
            startPosition.y + 5,
            startPosition.z - 10
        );
        gameManager.cam.transform.LookAt(new Vector3(
            gameManager.current_level.GetLength(0) / 2,
            startPosition.y,
            startPosition.z
        ));

        if (gameManager.currentView == View.TopdownView)
        {
            gameManager.Flip(startPosition);
        }
        else
        {
            gameManager.Flip(startPosition, false);
            gameManager.Flip(startPosition, false);
        }
    }

    public bool CanIUseLadder(Vector3 playerPos, Vector3 targetPos)
    {
        Vector3 delta = targetPos - playerPos;

        if (delta.x == 1) // Moving right
        {
            if (delta.y == 1) // Climbing up
                return gameManager.GetBlock(playerPos + Vector3.up).GetBlockType() == "right ladder";
            else if (delta.y == -1) // Climbing down
                return gameManager.GetBlock(playerPos + Vector3.right).GetBlockType() == "left ladder";
        }
        else if (delta.x == -1) // Moving left
        {
            if (delta.y == 1) // Climbing up
                return gameManager.GetBlock(playerPos + Vector3.up).GetBlockType() == "left ladder";
            else if (delta.y == -1) // Climbing down
                return gameManager.GetBlock(playerPos + Vector3.left).GetBlockType() == "right ladder";
        }
        return false;
    }
}
