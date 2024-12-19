using UnityEngine;

public class LevelFlipper : MonoBehaviour
{
    private GameManager _gameManager;

    private GameObject[,,] _level;
    private bool _init;

    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _level = _gameManager.CurrentLevel;
    }

    private void Update()
    {
        if (!_init)
        {
            _init = true;
            RefreshLevel();
        }
        if (!GameManager.Instance.playerMovement.isMoving)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip();
        }
    }

    public void RefreshLevel()
    {
        ActivateBlocks();
        ActivateBlocks();
    }

    private void Flip()
    {
        ActivateBlocks(); 

        _gameManager.cameraScript.camAnimator.SetTrigger(_gameManager.currentView == GameManager.View.TopdownView
            ? "FlipToTopView"
            : "FlipToSideView");

        _gameManager.cameraScript.FlipView(_gameManager.currentView);
    }

    private void ActivateBlocks()
    {
        if (_level == null)
        {
            Debug.LogError("LevelFlipper: Level array is null.");
            return;
        }
        
        Vector3 playerPosition = GameObject.Find("Player").transform.position;

        int playerPositionY = Mathf.RoundToInt(playerPosition.y);
        int playerPositionZ = Mathf.RoundToInt(playerPosition.z);

        for (int i = 0; i < _level.GetLength(0); i++)
        {
            for (int k = 0; k < _level.GetLength(1); k++)
            {
                for (int j = 0; j < _level.GetLength(2); j++)
                {
                    GameObject blockObj = _level[i, k, j];

                    if (blockObj == null)
                    {
                        continue;
                    }

                    Block blockComponent = blockObj.GetComponent<Block>();

                    if (blockComponent == null)
                    {
                        Debug.LogWarning($"LevelFlipper: Block at ({i}, {k}, {j}) is missing the Block component.");
                        continue; // Skip blocks without a Block component
                    }

                    if (_gameManager.currentView == GameManager.View.SideView)
                    {
                        bool onHeightPlane = Mathf.RoundToInt(blockObj.transform.position.y) == playerPositionY;
                        blockComponent.Activate(onHeightPlane);
                    }
                    else // TopdownView
                    {
                        bool onSidePlane = Mathf.RoundToInt(blockObj.transform.position.z) == playerPositionZ;
                        blockComponent.Activate(onSidePlane);
                    }
                }
            }
        }
        
        // Update the global currentView variable
        _gameManager.currentView = _gameManager.currentView == GameManager.View.SideView
            ? GameManager.View.TopdownView 
            : GameManager.View.SideView;
    }
}
