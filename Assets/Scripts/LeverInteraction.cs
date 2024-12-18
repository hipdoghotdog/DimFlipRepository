using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class LeverInteractionScript : MonoBehaviour
{
    public Animator playerAnimator;      // Assign this in the Inspector
    public float leverAnimationDuration = 0.6f;

    private bool _isInteracting;
    private GameManager _gameManager;

    private void Start()
    {
        // Find the GameManager in the scene
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }

        // Ensure playerAnimator is assigned
        if (playerAnimator != null) return;
        
        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator component not found on the player.");
        }
    }

    private void Update()
    {
        HandleLeverInput();
    }

    private Block GetBlock(Vector3 position)
    {
        return _gameManager.CurrentLevel[(int)position.x, (int)position.y, (int)position.z].GetComponent<Block>();
    }

    private void HandleLeverInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 playerPosition = transform.position;

            // Assuming GameManager has a method to get the block at a position
            if (GetBlock(playerPosition).GetBlockType() == "lever")
            {
                HandleLeverInteraction(playerPosition);
            }
        }
    }

    private void HandleLeverInteraction(Vector3 playerPosition)
    {
        if (_isInteracting) return;
        StartCoroutine(HandleLeverInteractionCoroutine(playerPosition));
    }

    private IEnumerator HandleLeverInteractionCoroutine(Vector3 playerPosition)
    {
        _isInteracting = true;

        // Trigger the player animation
        playerAnimator.SetTrigger("Pull");

        yield return new WaitForSeconds(leverAnimationDuration);

        ApplyLeverEffects(playerPosition);

        _isInteracting = false;
    }

    private void ApplyLeverEffects(Vector3 playerPosition)
    {
        // Toggle the lever state
        bool state = !GetBlock(playerPosition).switchOn;
        GetBlock(playerPosition).Pull(state);

        // Existing lever interaction logic
        int num = (int)(playerPosition.x * 100 + playerPosition.y * 10 + playerPosition.z);
        int BlockCC = _gameManager.levelBuilder.Levels[_gameManager.currentLevelIndex].InterActPairs[num];
        int[] c = new int[4];
        
        for (int n = 3; n >= 0; n--)
        {
            c[n] = BlockCC % 10;
            BlockCC /= 10;
        }
        
        GameObject block = _gameManager.CurrentLevel[c[0], c[1], c[2]];
        Vector3 blockPos = block.transform.position;
        int i = state ? -1 : 1;
        
        switch (c[3])
        {
            case 1:
                i *= -1;
                blockPos.y += i;
                block.transform.position = blockPos;
                _gameManager.CurrentLevel[c[0], c[1], c[2]] = Instantiate(_gameManager.levelBuilder.BlockTemplates[0]);
                _gameManager.CurrentLevel[c[0], c[1] + i, c[2]] = block;
                break;
            case 2:
                blockPos.y += i;
                block.transform.position = blockPos;
                _gameManager.CurrentLevel[c[0], c[1], c[2]] = Instantiate(_gameManager.levelBuilder.BlockTemplates[0]);
                _gameManager.CurrentLevel[c[0], c[1] + i, c[2]] = block;
                break;
            case 3:
                i *= -1;
                blockPos.x += i;
                block.transform.position = blockPos;
                _gameManager.CurrentLevel[c[0], c[1], c[2]] = Instantiate(_gameManager.levelBuilder.BlockTemplates[0]);
                _gameManager.CurrentLevel[c[0] + i, c[1], c[2]] = block;
                break;
            case 4:
                blockPos.x += i;
                block.transform.position = blockPos;
                _gameManager.CurrentLevel[c[0], c[1], c[2]] = Instantiate(_gameManager.levelBuilder.BlockTemplates[0]);
                _gameManager.CurrentLevel[c[0] + i, c[1], c[2]] = block;
                break;
        }

        // Refresh the level state
        _gameManager.levelFlipper.RefreshLevel();

        _gameManager.levelBuilder.Levels[_gameManager.currentLevelIndex].InterActPairs[num] = c[0] * 1000 + (c[1] + i) * 100 + c[2] * 10 + c[3];

        // Play lever sound
        SoundManager.Instance.PlaySound(Sound.Lever);
    }
}
