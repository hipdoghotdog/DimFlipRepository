using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int STARTLEVEL = 7;

    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;
    public GameObject cam;
    [HideInInspector]
    public GameObject[,,] current_level;
    private bool init = false;
    public GameObject endScene;

    public Animator camAnimator;
    public Animator playerAnimator; // Assign this in the Inspector

    private CameraScript cameraScript;

    public ArtifactManager am;

    public PlayerMovement pm;

    public enum View
    {
        SideView,
        TopdownView
    }

    public View currentView;    

    void Start()
    {
        am.DisplayText("Hello There!");

        cameraScript = cam.GetComponent<CameraScript>();

        load_new_level(STARTLEVEL);

        pm = player.GetComponent<PlayerMovement>();
        pm.ResetPlayerPosition();
    }

    public void Flip(Vector3 pp, bool camFlip = true)
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

    public Block GetBlock(Vector3 p)
    {
        return current_level[(int)p.x, (int)p.y, (int)p.z].GetComponent<Block>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            Flip(player.transform.position, false);
            Flip(player.transform.position, false);
            if (currentView == View.TopdownView)
            {
                Flip(player.transform.position);
            }
            init = true;
        }
        if (!pm.isMoving) { 
            HandleInput();
        }
    }

    void HandleInput()
    {

        // Handle reset position
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            on_next_level(lb.currentLevel - 1); //reload current level
        }

        // Flip level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip(player.transform.position);
        }
    }

    public void on_next_level(int level)
    {

        if (level == 5)
        {
            endScene.SetActive(true);
        }
        else
        {
            init = false;
        }

        destroy_current_level();

        load_new_level(level + 1);

        pm.ResetPlayerPosition();

        switch (level)
        {
            case 1:
                am.DisplayText("Great! Try finding somewhere to climb up.");
                break;
            case 2:
                am.DisplayText("What a confusing mess of blocks! I can reset you back to the start if you get lost.");
                break;
            case 3:
                am.DisplayText("Well done! Look at that lever there.");
                break;
            case 4:
                am.DisplayText("Hmm, this looks challenging. There must be a catch to get through this.");
                break;
            case 5:
                am.DisplayText("Try to push the wooden crate. Remember you can reset with 'Backspace'");
                break;
        }
    }

    public void destroy_current_level()
    {
        for (int i = 0; i < current_level.GetLength(0); i++)
        {
            for (int k = 0; k < current_level.GetLength(1); k++)
            {
                for (int j = 0; j < current_level.GetLength(2); j++)
                {
                    if (current_level[i, k, j] == null) { continue; }

                    Destroy(current_level[i, k, j]);
                }
            }
        }
        current_level = null;
    }

    public void load_new_level(int new_level)
    {
        lb.currentLevel = new_level;

        lf.SetLevel(lb.RemoteBuild());

        current_level = lf.level;
    }

    public void CheckGravity()
    {
        bool blocksFell = true;
        while (blocksFell)
        {
            blocksFell = false;

            for (int x = 0; x < current_level.GetLength(0); x++)
            {
                for (int y = 0; y < current_level.GetLength(1); y++)
                {
                    for (int z = 0; z < current_level.GetLength(2); z++)
                    {
                        GameObject go = current_level[x, y, z];
                        if (go == null) continue;

                        Block block = go.GetComponent<Block>();
                        if (block != null && block.isPushable && block.isActive)
                        {
                            int belowY = y - 1;
                            if (belowY < 0)
                            {
                                // Block falls out of the level
                                // Instead of destroying immediately, animate it falling out.
                                // Let's say we drop it one extra unit below:
                                Vector3 fallOutPos = new Vector3(x, -1, z);
                                StartCoroutine(FallOutOfLevelCoroutine(go, fallOutPos));

                                current_level[x, y, z] = null;
                                blocksFell = true;
                            }
                            else
                            {
                                // Check if below is empty
                                GameObject belowBlockGO = current_level[x, belowY, z];
                                if (belowBlockGO == null)
                                {
                                    // Just fall down
                                    StartCoroutine(MoveBlockCoroutine(block, new Vector3(x, belowY, z)));
                                    current_level[x, belowY, z] = go;
                                    current_level[x, y, z] = null;
                                    blocksFell = true;
                                }
                                else
                                {
                                    Block belowBlock = belowBlockGO.GetComponent<Block>();
                                    if (belowBlock != null && belowBlock.blockType.ToLower() == "empty")
                                    {
                                        // Swap with empty block
                                        StartCoroutine(MoveBlockCoroutine(block, new Vector3(x, belowY, z)));
                                        current_level[x, belowY, z] = go;
                                        current_level[x, y, z] = belowBlockGO;
                                        blocksFell = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator MoveBlockCoroutine(Block block, Vector3 targetPosition)
    {
        if (block == null) yield break; // Safety check

        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 initialPosition = block.transform.position;

        // Make sure the block's GameObject is still alive
        GameObject blockGO = block.gameObject;

        while (elapsed < duration)
        {
            if (blockGO == null) yield break; // Block got destroyed externally?
            blockGO.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (blockGO != null)
        {
            blockGO.transform.position = targetPosition;
        }
    }

    // New coroutine for falling out of the level
    public IEnumerator FallOutOfLevelCoroutine(GameObject blockGO, Vector3 targetPosition)
    {
        Block block = blockGO.GetComponent<Block>();
        if (block == null) yield break;

        // Animate the block falling out of the level
        yield return MoveBlockCoroutine(block, targetPosition);

        // After finishing the animation, we can now safely destroy the block
        if (blockGO != null)
        {
            Destroy(blockGO);
        }
    }



}