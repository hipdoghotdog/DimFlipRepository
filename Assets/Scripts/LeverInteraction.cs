using System.Collections;
using UnityEngine;

public class LeverInteractionScript : MonoBehaviour
{
    public Animator playerAnimator;      // Assign this in the Inspector
    public float leverAnimationDuration = 0.2f;

    private bool isInteracting = false;
    private GameManager gameManager;

    void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }

        // Ensure playerAnimator is assigned
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Animator component not found on the player.");
            }
        }
    }

    void Update()
    {
        HandleLeverInput();
    }

    void HandleLeverInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 pp = transform.position;

            // Assuming GameManager has a method to get the block at a position
            if (gameManager.GetBlock(pp).GetBlockType() == "lever")
            {
                HandleLeverInteraction(pp);
            }
        }
    }

    void HandleLeverInteraction(Vector3 pp)
    {
        if (isInteracting) return;
        StartCoroutine(HandleLeverInteractionCoroutine(pp));
    }

    IEnumerator HandleLeverInteractionCoroutine(Vector3 pp)
    {
        isInteracting = true;

        // Trigger the player animation
        playerAnimator.SetTrigger("Pull");

        yield return new WaitForSeconds(leverAnimationDuration);

        ApplyLeverEffects(pp);

        isInteracting = false;
    }

    void ApplyLeverEffects(Vector3 pp)
    {
        // Toggle the lever state
        bool state = !gameManager.GetBlock(pp).switchOn;
        gameManager.GetBlock(pp).Pull(state);

        // Existing lever interaction logic
        int num = (int)(pp.x * 100 + pp.y * 10 + pp.z);
        int BlockCC = gameManager.lb.interActPairs[num];
        int[] c = new int[4];
        for (int n = 3; n >= 0; n--)
        {
            c[n] = BlockCC % 10;
            BlockCC /= 10;
        }
        GameObject block = gameManager.current_level[c[0], c[1], c[2]];
        Vector3 blockPos = block.transform.position;
        int i = state ? -1 : 1;
        switch (c[3])
        {
            case 1:
                i *= -1;
                blockPos.y += i;
                block.transform.position = blockPos;
                gameManager.current_level[c[0], c[1], c[2]] = Instantiate(gameManager.lb.blockTemplates[0]);
                gameManager.current_level[c[0], c[1] + i, c[2]] = block;
                break;
            case 2:
                blockPos.y += i;
                block.transform.position = blockPos;
                gameManager.current_level[c[0], c[1], c[2]] = Instantiate(gameManager.lb.blockTemplates[0]);
                gameManager.current_level[c[0], c[1] + i, c[2]] = block;
                break;
            case 3:
                i *= -1;
                blockPos.x += i;
                block.transform.position = blockPos;
                gameManager.current_level[c[0], c[1], c[2]] = Instantiate(gameManager.lb.blockTemplates[0]);
                gameManager.current_level[c[0] + i, c[1], c[2]] = block;
                break;
            case 4:
                blockPos.x += i;
                block.transform.position = blockPos;
                gameManager.current_level[c[0], c[1], c[2]] = Instantiate(gameManager.lb.blockTemplates[0]);
                gameManager.current_level[c[0] + i, c[1], c[2]] = block;
                break;
        }

        // Refresh the level state
        gameManager.Flip(pp, false);
        gameManager.Flip(pp, false);

        gameManager.lb.interActPairs[num] = c[0] * 1000 + (c[1] + i) * 100 + c[2] * 10 + c[3];

        // Play lever sound
        SoundManager.instance.PlaySound(Sound.LEVER);
    }
}
