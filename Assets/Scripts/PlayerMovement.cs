using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public Animator playerAnimator;
    public Quaternion targetRotation;
    public Vector3 targetPosition;

    void Start()
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
        if (!gameManager.isMoving)
        {
            HandleInput();
        } else
        {
            //MovePlayer();
        }
    }

    public bool CanIStepOnBlock(Vector3 p)
    {
        string b = gameManager.GetBlock(p).GetBlockType();
        return !(gameManager.lb.GetUnsteppableBlocks().Contains(b));
    }

    void HandleInput()
    {
        Vector3 pp = transform.position;

        // Movement input handling
        Vector3 newPP = pp;
        bool movementKeyPressed = false;

        // Define a temporary variable for desired rotation
        Quaternion desiredRotation = transform.rotation;

        // Move Player Right
        if (Input.GetKeyDown(KeyCode.RightArrow) && pp.x != gameManager.current_level.GetLength(0) - 1)
        {
            if (gameManager.currentView == View.TopdownView)
            {
                if (CanIStepOnBlock(new Vector3(pp.x + 1, pp.y, pp.z)))
                {
                    newPP.x += 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    desiredRotation = Quaternion.Euler(0, 0, 0);
                    movementKeyPressed = true;
                }
            }
            else
            {
                // Side view movement logic
                Vector3 toPos = new Vector3(pp.x + 1, pp.y + 1, pp.z);
                Vector3 toPos2 = new Vector3(pp.x + 1, pp.y - 1, pp.z);
                if (pp.y != gameManager.current_level.GetLength(1) - 1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
                {
                    // Step up if ladder present
                    newPP.y += 1;
                    newPP.x += 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                else if (pp.y != 0 && CanIStepOnBlock(toPos2) && CanIUseLadder(pp, toPos2))
                {
                    // Step down if ladder present
                    newPP.x += 1;
                    newPP.y -= 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                else if (CanIStepOnBlock(new Vector3(pp.x + 1, pp.y, pp.z)) && pp.y == gameManager.current_level.GetLength(1) - 1
                        || CanIStepOnBlock(new Vector3(pp.x + 1, pp.y, pp.z)) && !CanIStepOnBlock(new Vector3(pp.x + 1, pp.y + 1, pp.z)))
                {
                    // Move right
                    newPP.x += 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                desiredRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        // Move Player Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) && pp.x != 0)
        {
            if (gameManager.currentView == View.TopdownView)
            {
                if (CanIStepOnBlock(new Vector3(pp.x - 1, pp.y, pp.z)))
                {
                    newPP.x -= 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    desiredRotation = Quaternion.Euler(0, 180, 0);
                    movementKeyPressed = true;
                }
            }
            else
            {
                // Side view movement logic
                Vector3 toPos = new Vector3(pp.x - 1, pp.y + 1, pp.z);
                Vector3 toPos2 = new Vector3(pp.x - 1, pp.y - 1, pp.z);
                if (pp.y != gameManager.current_level.GetLength(1) - 1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
                {
                    // Move up
                    newPP.y += 1;
                    newPP.x -= 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                else if (pp.y != 0 && CanIStepOnBlock(toPos2) && CanIUseLadder(pp, toPos2))
                {
                    // Move down
                    newPP.x -= 1;
                    newPP.y -= 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                else if (CanIStepOnBlock(new Vector3(pp.x - 1, pp.y, pp.z)) && pp.y == gameManager.current_level.GetLength(1) - 1 ||
                        CanIStepOnBlock(new Vector3(pp.x - 1, pp.y, pp.z)) && !CanIStepOnBlock(new Vector3(pp.x - 1, pp.y + 1, pp.z)))
                {
                    // Move left
                    newPP.x -= 1;
                    SoundManager.instance.PlaySound(Sound.STEP);
                    movementKeyPressed = true;
                }
                desiredRotation = Quaternion.Euler(0, 180, 0);
            }
        }

        // Move Player Up (Top-down view)
        if (gameManager.currentView == View.TopdownView)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && pp.z != gameManager.current_level.GetLength(2) - 1 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z + 1)))
            {
                newPP.z += 1;
                SoundManager.instance.PlaySound(Sound.STEP);
                desiredRotation = Quaternion.Euler(0, -90, 0);
                movementKeyPressed = true;
            }

            // Move Player Down (Top-down view)
            if (Input.GetKeyDown(KeyCode.DownArrow) && pp.z != 0 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z - 1)))
            {
                newPP.z -= 1;
                SoundManager.instance.PlaySound(Sound.STEP);
                desiredRotation = Quaternion.Euler(0, 90, 0);
                movementKeyPressed = true;
            }
        }

        if (movementKeyPressed && newPP != pp)
        {
            // Calculate the angle difference
            float angleDifference = Quaternion.Angle(transform.rotation, desiredRotation);

            // Only set targetRotation if the difference is significant
            if (angleDifference > 1f)
            {
                targetRotation = desiredRotation;
            }

            // Start moving towards the target position
            targetPosition = newPP;
            gameManager.isMoving = true;
            playerAnimator.SetBool("isWalking", true);
        }
    }

    public void ResetPlayerPosition()
    {
        Vector3 pp = gameManager.lb.startBlockPosition;
        targetPosition = pp;
        gameManager.isMoving = true;
        playerAnimator.SetBool("isWalking", true);
        transform.position = pp;
        targetRotation = Quaternion.identity;
        gameManager.cam.transform.position = new Vector3(gameManager.current_level.GetLength(0) / 2, pp.y + 5, pp.z - 10);
        gameManager.cam.transform.LookAt(new Vector3(gameManager.current_level.GetLength(0) / 2, pp.y, pp.z));

        if (gameManager.currentView == View.TopdownView)
        {
            gameManager.Flip(pp);
        }
        else
        {
            gameManager.Flip(pp, false);
            gameManager.Flip(pp, false);
        }
    }

    public bool CanIUseLadder(Vector3 playerPos, Vector3 targetPos)
    {
        // Going right
        if (playerPos.x - targetPos.x == -1)
        {
            if (playerPos.y - targetPos.y == -1)
            {
                // Climbing up
                return gameManager.GetBlock(new Vector3(0, 1, 0) + playerPos).GetBlockType() == "right ladder";
            }
            else if (playerPos.y - targetPos.y == 1)
            {
                // Climbing down
                return gameManager.GetBlock(new Vector3(1, 0, 0) + playerPos).GetBlockType() == "left ladder";
            }
        } // Going left
        else if (playerPos.x - targetPos.x == 1)
        {
            if (playerPos.y - targetPos.y == -1)
            {
                // Climbing up
                return gameManager.GetBlock(new Vector3(0, 1, 0) + playerPos).GetBlockType() == "left ladder";
            }
            else if (playerPos.y - targetPos.y == 1)
            {
                // Climbing down
                return gameManager.GetBlock(new Vector3(-1, 0, 0) + playerPos).GetBlockType() == "right ladder";
            }
        }
        return false;
    }

    public void MovePlayer()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager reference is missing.");
            return;
        }

        Vector3 target = targetPosition;

        // Smoothly move the player towards the target position
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // Check if the player has reached the target position
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            gameManager.isMoving = false;
            playerAnimator.SetBool("isWalking", false);

            // When reaching end goal
            if (gameManager.GetBlock(transform.position).GetBlockType() == "end")
            {
                gameManager.on_next_level(gameManager.lb.currentLevel);
            }
        }
    }

}
