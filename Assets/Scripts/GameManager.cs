using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;
    public GameObject cam;
    private GameObject[,,] level1;
    private bool init = false;
    public GameObject endScene;

    public Animator camAnimator;
    public Animator playerAnimator; // Assign this in the Inspector

    private CameraScript cameraScript;

    public ArtifactManager am;

    public enum View
    {
        SideView,
        TopdownView
    }

    public View currentView;

    // Movement variables
    private Vector3 logicalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    public float moveSpeed = 5f; // Adjust for movement speed

    private Quaternion targetRotation;
    public float rotationSpeed = 720f;

    void Flip(Vector3 pp, bool camFlip = true)
    {
        lf.flipLevel(pp, currentView);
        currentView = currentView == View.SideView ? View.TopdownView : View.SideView;

        if (currentView == View.TopdownView && camFlip)
        {
            camAnimator.SetTrigger("FlipToTopView");
        }
        else if (currentView == View.SideView && camFlip)
        {
            camAnimator.SetTrigger("FlipToSideView");
        }

        // Notify CameraScript to switch view
        if (camFlip && cameraScript != null)
        {
            cameraScript.FlipView(currentView);
        }
    }

    bool CanIStepOnBlock(Vector3 p)
    {
        string b = GetBlock(p).GetBlockType();
        return !(lb.GetUnsteppableBlocks().Contains(b));
    }

    // Take player position and block you want to go to
    // Returns TRUE if there is a valid ladder at the right place
    bool CanIUseLadder(Vector3 playerPos, Vector3 targetPos)
    {
        // Going right
        if (playerPos.x - targetPos.x == -1)
        {
            if (playerPos.y - targetPos.y == -1)
            {
                // Climbing up
                return GetBlock(new Vector3(0, 1, 0) + playerPos).GetBlockType() == "right ladder"; ;
            }
            else if (playerPos.y - targetPos.y == 1)
            {
                // Climbing down
                return GetBlock(new Vector3(1, 0, 0) + playerPos).GetBlockType() == "left ladder";
            }
        } // Going left
        else if (playerPos.x - targetPos.x == 1)
        {
            if (playerPos.y - targetPos.y == -1)
            {
                // Climbing up
                return GetBlock(new Vector3(0, 1, 0) + playerPos).GetBlockType() == "left ladder";
            }
            else if (playerPos.y - targetPos.y == 1)
            {
                // Climbing down
                return GetBlock(new Vector3(-1, 0, 0) + playerPos).GetBlockType() == "right ladder";
            }
        }
        return false;
    }

    Block GetBlock(Vector3 p)
    {
        return level1[(int)p.x, (int)p.y, (int)p.z].GetComponent<Block>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lf.SetLevel(lb.RemoteBuild());
        level1 = lf.level;
        player.transform.position = lb.levelParent.transform.position;
        logicalPosition = player.transform.position; // Initialize logical position

        Vector3 spp = player.transform.position;
        cam.transform.position = new Vector3(level1.GetLength(0) / 2, spp.y + 5, spp.z - 10);
        cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, spp.y, spp.z));
        endScene.SetActive(false);

        am.DisplayText("Hello There!");

        // Initialize CameraScript reference
        cameraScript = cam.GetComponent<CameraScript>();
        if (cameraScript == null)
        {
            Debug.LogError("CameraScript component not found on the camera.");
        }

        targetRotation = player.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            player.transform.position = new Vector3(0, 0, 0);
            logicalPosition = player.transform.position; // Ensure logical position is set
            Flip(logicalPosition, false);
            Flip(logicalPosition, false);
            if (currentView == View.TopdownView)
            {
                Flip(logicalPosition);
            }
            init = true;
        }

        // Handle smooth rotation
        player.transform.rotation = Quaternion.RotateTowards(
            player.transform.rotation,
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

    void HandleInput()
    {
        Vector3 pp = logicalPosition;

        // Handle lever interaction
        if (Input.GetKeyDown(KeyCode.E) && GetBlock(pp).GetBlockType() == "lever")
        {
            HandleLeverInteraction(pp);
        }

        // Handle reset position
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetPlayerPosition();
        }

        // Flip level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip(pp);
        }

        // Movement input handling
        Vector3 newPP = pp;
        bool movementKeyPressed = false;

        // Define a temporary variable for desired rotation
        Quaternion desiredRotation = player.transform.rotation;

        // Move Player Right
        if (Input.GetKeyDown(KeyCode.RightArrow) && pp.x != level1.GetLength(0) - 1)
        {
            if (currentView == View.TopdownView)
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
                if (pp.y != level1.GetLength(1) - 1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
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
                else if (CanIStepOnBlock(new Vector3(pp.x + 1, pp.y, pp.z)) && pp.y == level1.GetLength(1) - 1
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
            if (currentView == View.TopdownView)
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
                if (pp.y != level1.GetLength(1) - 1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
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
                else if (CanIStepOnBlock(new Vector3(pp.x - 1, pp.y, pp.z)) && pp.y == level1.GetLength(1) - 1 ||
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
        if (currentView == View.TopdownView)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && pp.z != level1.GetLength(2) - 1 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z + 1)))
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
            float angleDifference = Quaternion.Angle(player.transform.rotation, desiredRotation);

            // Only set targetRotation if the difference is significant
            if (angleDifference > 1f)
            {
                targetRotation = desiredRotation;
            }

            // Start moving towards the target position
            targetPosition = newPP;
            isMoving = true;
            playerAnimator.SetBool("isWalking", true);
        }
    }


    void MovePlayer()
    {
        // Smoothly move the player towards the target position
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the player has reached the target position
        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            player.transform.position = targetPosition;
            logicalPosition = targetPosition;
            isMoving = false;
            playerAnimator.SetBool("isWalking", false);

            // Handle interactions after movement completes
            HandlePostMovementInteractions();
        }
    }


    void HandlePostMovementInteractions()
    {
        Vector3 pp = logicalPosition;

        // When reaching end goal
        if (GetBlock(pp).GetBlockType() == "end")
        {
            pp = lb.levelParent.transform.position;
            logicalPosition = pp;
            player.transform.position = pp;

            if (lb.currentLevel == 4)
            {
                endScene.SetActive(true);
            }
            else
            {
                lb.currentLevel += 1;

                // Clean up old level
                for (int i = 0; i < level1.GetLength(0); i++)
                {
                    for (int k = 0; k < level1.GetLength(1); k++)
                    {
                        for (int j = 0; j < level1.GetLength(2); j++)
                        {
                            if (level1[i, k, j] == null) { continue; }

                            Destroy(level1[i, k, j]);
                        }
                    }
                }
                level1 = null;

                // Load new level
                lf.SetLevel(lb.RemoteBuild());
                level1 = lf.level;
                pp = lb.levelParent.transform.position;
                logicalPosition = pp;
                player.transform.position = pp;

                Vector3 spp = player.transform.position;
                cam.transform.position = new Vector3(level1.GetLength(0) / 2, spp.y + 5, spp.z - 10);
                cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, spp.y, spp.z));

                init = false;
            }

            // Get some more dialogue from the artifact
            switch (lb.currentLevel)
            {
                case 1:
                    am.DisplayText("Great! try finding somewhere to climb up");
                    break;
                case 2:
                    am.DisplayText("What a confusing mess of blocks! \nI can reset you back to the start if you get lost");
                    break;
                case 3:
                    am.DisplayText("Well done! Look at that lever there");
                    break;
                case 4:
                    am.DisplayText("Hmm, this look challenging. \nThere must be a catch to get through this");
                    break;
            }
        }
    }

    void HandleLeverInteraction(Vector3 pp)
    {
        bool state = !GetBlock(pp).switchOn;
        GetBlock(pp).pull(state);
        int num = (int)(pp.x * 100 + pp.y * 10 + pp.z);
        int BlockCC = lb.interActPairs[num];
        int[] c = new int[4];
        for (int n = 3; n >= 0; n--)
        {
            c[n] = BlockCC % 10;
            BlockCC /= 10;
        }
        GameObject block = level1[c[0], c[1], c[2]];
        Vector3 blockPos = block.transform.position;
        int i = state ? -1 : 1;
        switch (c[3])
        {
            case 1:
                i *= -1;
                blockPos.y += i;
                block.transform.position = (blockPos);
                level1[c[0], c[1], c[2]] = Instantiate(lb.blockTemplates[0]);
                level1[c[0], c[1] + i, c[2]] = block;
                break;
            case 2:
                blockPos.y += i;
                block.transform.position = (blockPos);
                level1[c[0], c[1], c[2]] = Instantiate(lb.blockTemplates[0]);
                level1[c[0], c[1] + i, c[2]] = block;
                break;
            case 3:
                i *= -1;
                blockPos.x += i;
                block.transform.position = (blockPos);
                level1[c[0], c[1], c[2]] = Instantiate(lb.blockTemplates[0]);
                level1[c[0] + i, c[1], c[2]] = block;
                break;

            case 4:
                blockPos.x += i;
                block.transform.position = (blockPos);
                level1[c[0], c[1], c[2]] = Instantiate(lb.blockTemplates[0]);
                level1[c[0] + i, c[1], c[2]] = block;

                break;
        }
        Flip(pp, false);
        Flip(pp, false);

        lb.interActPairs[num] = c[0] * 1000 + (c[1] + i) * 100 + c[2] * 10 + c[3];

        // Play sound of lever
        SoundManager.instance.PlaySound(Sound.LEVER);
    }

    void ResetPlayerPosition()
    {
        Vector3 pp = lb.levelParent.transform.position;
        logicalPosition = pp;
        targetPosition = pp;
        isMoving = true;
        playerAnimator.SetBool("isWalking", true);
        player.transform.position = pp;

        if (currentView == View.TopdownView)
        {
            Flip(pp);
        }
        else
        {
            Flip(pp, false);
            Flip(pp, false);
        }
    }
}
