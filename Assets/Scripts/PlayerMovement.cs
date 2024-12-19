using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameManager;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private GameManager _gameManager;

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public Animator playerAnimator;
    private Quaternion _targetRotation;
    private Vector3 _targetPosition;
    public bool isMoving = false;

    // Movement queue for handling step-by-step movement
    private readonly Queue<Vector3> _movementQueue = new();

    private void Awake()
    {
        _gameManager = Instance;
    }

    private void Update()
    {
        // Handle rotation towards the target rotation
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            _targetRotation,
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

    private Block GetBlock(Vector3 position)
    {
        if (_gameManager.CurrentLevel == null)
        {
            Debug.LogError("PlayerMovement: CurrentLevel is null in GameManager.");
            return null;
        }

        if (position.x < 0 || position.x >= _gameManager.CurrentLevel.GetLength(0) ||
            position.y < 0 || position.y >= _gameManager.CurrentLevel.GetLength(1) ||
            position.z < 0 || position.z >= _gameManager.CurrentLevel.GetLength(2))
        {
            Debug.LogWarning($"PlayerMovement: Position {position} is out of bounds.");
            return null;
        }

        GameObject blockGO = _gameManager.CurrentLevel[(int)position.x, (int)position.y, (int)position.z];
        if (blockGO == null)
            return null;

        return blockGO.GetComponent<Block>();
    }

    private bool CanIStepOnBlock(Vector3 position)
    {
        Block block = GetBlock(position);
        if (block == null)
        {
            Debug.LogWarning($"PlayerMovement: No block found at position {position}.");
            return false;
        }

        string blockType = block.GetBlockType();
        return !_gameManager.levelBuilder.GetUnsteppableBlocks().Contains(blockType);
    }

    private void HandleInput()
    {
        // Check horizontal movement
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TryMoveHorizontal(1, Quaternion.Euler(0, 0, 0));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            TryMoveHorizontal(-1, Quaternion.Euler(0, 180, 0));
        }
        else if (_gameManager.currentView == View.TopdownView)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                TryMoveVertical(1, Quaternion.Euler(0, -90, 0));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                TryMoveVertical(-1, Quaternion.Euler(0, 90, 0));
            }
        }
    }

    private void TryMoveHorizontal(int xIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;

        int maxX = _gameManager.CurrentLevel.GetLength(0) - 1;
        if ((xIncrement > 0 && currentPosition.x >= maxX) || (xIncrement < 0 && currentPosition.x <= 0))
            return;

        if (_gameManager.currentView == View.TopdownView)
        {
            Vector3 targetPos = currentPosition + new Vector3(xIncrement, 0, 0);
            if (CanIStepOnBlock(targetPos))
            {
                StartMovement(targetPos, desiredRotation);
            }
        }
        else
        {
            TrySideViewMovement(xIncrement, desiredRotation);
        }
    }

    private void TrySideViewMovement(int xIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;

        Vector3 toPosUp = currentPosition + new Vector3(xIncrement, 1, 0);
        Vector3 toPosDown = currentPosition + new Vector3(xIncrement, -1, 0);
        Vector3 toPosSame = currentPosition + new Vector3(xIncrement, 0, 0);

        int maxY = _gameManager.CurrentLevel.GetLength(1) - 1;

        // Check for climbing up
        if (currentPosition.y < maxY && CanIStepOnBlock(toPosUp) && CanIUseLadder(currentPosition, toPosUp))
        {
            newPosition += new Vector3(xIncrement, 1, 0);
            StartMovement(newPosition, desiredRotation, isDownwardLadderMovement: false); // Moving up
        }
        // Check for climbing down
        else if (currentPosition.y > 0 && CanIStepOnBlock(toPosDown) && CanIUseLadder(currentPosition, toPosDown))
        {
            newPosition += new Vector3(xIncrement, -1, 0);
            StartMovement(newPosition, desiredRotation, isDownwardLadderMovement: true); // Moving down
        }
        // Regular horizontal movement or pushing a block
        else
        {
            // Check if the target position is steppable
            if (CanIStepOnBlock(toPosSame) && (currentPosition.y == maxY || !CanIStepOnBlock(toPosSame + Vector3.up)))
            {
                Vector3 blockPosition = toPosSame + Vector3.up;
                Block blockAtToPosSame = gameManager.GetBlock(blockPosition);
                if (blockAtToPosSame != null && blockAtToPosSame.GetBlockType() == "story")
                {
                    const float yThreshold = 0.2f;

                    // Calculate the expected Y-position
                    float expectedY = currentPosition.y + 1f;
                    float actualY = blockAtToPosSame.transform.position.y;

                    // Check if the actual Y-position is within the acceptable range
                    if (Mathf.Abs(actualY - expectedY) < yThreshold)
                    {
                        gameManager.am.DisplayText(blockAtToPosSame.storyText);

                        blockAtToPosSame.blockType = "empty";
                        blockAtToPosSame.ApplyTheme();

                        int x = Mathf.RoundToInt(blockPosition.x);
                        int y = Mathf.RoundToInt(blockPosition.y);
                        int z = Mathf.RoundToInt(blockPosition.z);
                        GameObject oldBlock = gameManager.current_level[x, y, z];
                        Destroy(oldBlock);

                        GameObject newBlock = Instantiate(gameManager.lb.blockTemplates[0], new Vector3(x, y, z), Quaternion.identity);
                        gameManager.current_level[x, y, z] = newBlock;
                    }
                }

                newPosition += new Vector3(xIncrement, 0, 0);
                StartMovement(newPosition, desiredRotation);
            }
            else
            {
                // Retrieve the block at the same horizontal position but one layer up
                Vector3 blockPosition = toPosSame + Vector3.up;
                Block blockAtToPosSame = GetBlock(blockPosition);

                Debug.LogWarning(blockAtToPosSame.GetBlockType());

                // Check if the block is pushable and on the correct level
                if (blockAtToPosSame != null  && Mathf.Approximately(blockAtToPosSame.transform.position.y, currentPosition.y + 1))
                {
                    // Calculate the position ahead of the pushable block
                    Vector3 pushBlockTargetPos = blockPosition + new Vector3(xIncrement, 0, 0); // Maintain the y-offset

                    // Check if the position ahead is within bounds
                    int maxX = _gameManager.CurrentLevel.GetLength(0) - 1;
                    int maxZ = _gameManager.CurrentLevel.GetLength(2) - 1;
                    maxY = _gameManager.CurrentLevel.GetLength(1) - 1;

                    if (pushBlockTargetPos.x >= 0 && pushBlockTargetPos.x <= maxX &&
                        pushBlockTargetPos.z >= 0 && pushBlockTargetPos.z <= maxZ &&
                        pushBlockTargetPos.y >= 0 && pushBlockTargetPos.y <= maxY)
                    {
                        // Check if the block at the target position is null or empty
                        Block targetBlock = GetBlock(pushBlockTargetPos);

                        if (targetBlock == null || targetBlock.blockType.ToLower() == "empty")
                        {
                            // Play push animation and sound
                            playerAnimator.SetTrigger("Push");
                            SoundManager.Instance.PlaySound(Sound.Push);

                                // Move the blocks
                                MoveBlock(blockAtToPosSame, pushBlockTargetPos);
                                MoveBlock(targetBlock, blockPosition);

                                // Update the level array
                                Vector3Int targetPos = Vector3Int.RoundToInt(pushBlockTargetPos);
                                Vector3Int currentPos = Vector3Int.RoundToInt(blockPosition);

                                // Swap the blocks in the level array
                                (_gameManager.CurrentLevel[targetPos.x, targetPos.y, targetPos.z],
                                _gameManager.CurrentLevel[currentPos.x, currentPos.y, currentPos.z]) =
                                    (_gameManager.CurrentLevel[currentPos.x, currentPos.y, currentPos.z],
                                    _gameManager.CurrentLevel[targetPos.x, targetPos.y, targetPos.z]);

                            // Move the player to the position of the block
                            newPosition += new Vector3(xIncrement, 0, 0);
                            StartMovement(newPosition, desiredRotation, activateWalkingAni: false);
                        }
                    }
                }
            }
        }

    #region MoveBlock Method
        /// <summary>
        /// Initiates the movement of a block to a new position using GameManager's coroutine.
        /// </summary>
        /// <param name="block">The block to move.</param>
        /// <param name="newPosition">The target position.</param>
        public void MoveBlock(Block block, Vector3 newPosition)
        {
            if (_gameManager == null)
            {
                Debug.LogError("PlayerMovement: GameManager instance is null.");
                return;
            }

            if (block == null)
            {
                Debug.LogWarning("PlayerMovement: Attempted to move a null block.");
                return;
            }

            _gameManager.StartCoroutine(_gameManager.MoveBlockCoroutine(block, newPosition));
        }
    #endregion

    private void TryMoveVertical(int zIncrement, Quaternion desiredRotation)
    {
        Vector3 currentPosition = transform.position;

        int maxZ = _gameManager.CurrentLevel.GetLength(2) - 1;
        if ((zIncrement > 0 && currentPosition.z >= maxZ) || (zIncrement < 0 && currentPosition.z <= 0)) return;

        Vector3 targetPos = currentPosition + new Vector3(0, 0, zIncrement);
        if (CanIStepOnBlock(targetPos))
        {
            StartMovement(targetPos, desiredRotation);
        }
    }

    private void StartMovement(Vector3 newPosition, Quaternion desiredRotation, bool isDownwardLadderMovement = false, bool activateWalkingAni = true)
    {
        _movementQueue.Clear();
        if (isDownwardLadderMovement)
        {
            Vector3 currentPosition = transform.position;
            Vector3 delta = newPosition - currentPosition;

            if (delta.x != 0 && delta.y != 0)
            {
                Vector3 intermediatePos = new Vector3(currentPosition.x + delta.x, currentPosition.y, currentPosition.z);
                _movementQueue.Enqueue(intermediatePos);
            }
        }

        _movementQueue.Enqueue(newPosition);

        _targetPosition = _movementQueue.Dequeue();
        isMoving = true;
        playerAnimator.SetBool(IsWalking, activateWalkingAni);

        // Handle rotation
        float angleDifference = Quaternion.Angle(transform.rotation, desiredRotation);
        if (angleDifference > 1f)
        {
            _targetRotation = desiredRotation;
        }

        // Play step sound
        SoundManager.Instance.PlaySound(Sound.Step);
    }

    private void MovePlayer()
    {
        if (_gameManager == null)
        {
            Debug.LogError("PlayerMovement: GameManager reference is missing.");
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
        {
            transform.position = _targetPosition;

            if (_movementQueue.Count > 0)
            {
                // Continue to the next position in the queue
                _targetPosition = _movementQueue.Dequeue();
            }
            else
            {
                // Movement complete
                isMoving = false;
                playerAnimator.SetBool(IsWalking, false);

                // Check for level completion
                Block currentBlock = GetBlock(transform.position);
                if (currentBlock != null && currentBlock.GetBlockType() == "end")
                {
                    _gameManager.NextLevel();
                }

                // After the player stops moving, apply gravity to blocks
                _gameManager.CheckGravity();
            }
        }
    }

    /// <summary>
    /// Resets the player's position to the start position defined in LevelBuilder.
    /// </summary>
    public void ResetPlayerPosition()
    {
        if (_gameManager == null || _gameManager.levelBuilder == null)
        {
            Debug.LogError("PlayerMovement: GameManager or LevelBuilder reference is missing.");
            return;
        }

        Vector3 startPosition = _gameManager.levelBuilder.GetStartBlockPosition();
        transform.position = startPosition;
        _targetPosition = startPosition;
        isMoving = true;
        playerAnimator.SetBool(IsWalking, true);
        _targetRotation = Quaternion.identity;
    }

    private bool CanIUseLadder(Vector3 playerPos, Vector3 targetPos)
    {
        Vector3 delta = targetPos - playerPos;

        if (Mathf.Approximately(delta.x, 1)) // Moving right
        {
            if (Mathf.Approximately(delta.y, 1)) // Climbing up
                return GetBlock(playerPos + Vector3.up).GetBlockType() == "right ladder";
            if (Mathf.Approximately(delta.y, -1)) // Climbing down
                return GetBlock(playerPos + Vector3.right).GetBlockType() == "left ladder";
        }
        else if (Mathf.Approximately(delta.x, -1)) // Moving left
        {
            if (Mathf.Approximately(delta.y, 1)) // Climbing up
                return GetBlock(playerPos + Vector3.up).GetBlockType() == "left ladder";
            if (Mathf.Approximately(delta.y, -1)) // Climbing down
                return GetBlock(playerPos + Vector3.left).GetBlockType() == "right ladder";
        }
        return false;
    }
}
