using UnityEngine;

public class Block : MonoBehaviour
{
    private static readonly int IsActive = Animator.StringToHash("isActive");
    private static readonly int SwitchOn = Animator.StringToHash("switchOn");
    public bool switchOn = false;
    public bool isActive = true;
    private Animator _blockAnim;
    private MeshRenderer _meshRenderer;
    public string blockType;

    // New properties
    public bool isPushable = false;
    public bool isPullable = false;

    // Reference to the Indicator (only for levers)
    public GameObject indicator; // Assign in the Inspector for levers only
    private MeshRenderer indicatorRenderer;

    //Story Blocks
    public string storyText = "";
    private Vector3 initialPosition;
    public float rotationSpeed = 20f;   // Degrees per second
    public float bobSpeed = 2f;         // Speed of the up/down bob
    public float bobHeight = 0.1f;      // Height amplitude of the bob

    private void Start()
    {
        _blockAnim = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
        ApplyTheme();

        // Initialize pushable and pullable based on blockType
        if (blockType.ToLower() == "pushable") isPushable = true;
        
        // Initialize Indicator only for levers
        if (blockType.ToLower() == "lever")
        {
            if (indicator != null)
            {
                indicatorRenderer = indicator.GetComponent<MeshRenderer>();
                UpdateIndicatorColor();
            }
            else
            {
                Debug.LogWarning("Indicator GameObject is not assigned in the Block script for a lever block.");
            }
        }

        // Store initial position for bobbing
        initialPosition = transform.position;
    }

    void Update()
    {
        // Only apply animation if this is a story block
        if (blockType.ToLower() == "story")
        {
            // Rotate around the Y-axis
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Bob up and down
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = initialPosition + new Vector3(0, bobOffset, 0);
        }
    }

    public void ApplyTheme()
    {
        ThemeData theme = ThemeManager.Instance.currentTheme;
        
        if (theme == null || _meshRenderer == null) return;
        
        switch (blockType.ToLower())
        {
            case "block":
            case "blockwlight":
            case "lever":
                _meshRenderer.material = theme.blockMaterial;
                break;
            case "empty":
                _meshRenderer.material = null;
                _meshRenderer.enabled = false;
                break;
            case "start":
                _meshRenderer.material = theme.startMaterial;
                break;
            case "end":
                _meshRenderer.material = theme.endMaterial;
                break;
            case "right ladder":
            case "left ladder":
                _meshRenderer.material = theme.ladderMaterial;
                break;
            case "pushable":
                _meshRenderer.material = theme.pushableMaterial;
                break;
            case "story":
                _meshRenderer.material = theme.storyMaterial;
                break;
            default:
                Debug.LogWarning($"Block: Unknown blockType '{blockType}'. No material applied.");
                break;
        }
    }

    public virtual void Activate(bool state)
    {
        if (_blockAnim != null)
        {
            _blockAnim.SetBool(IsActive, state);
            // For ALL blocks, including levers, set isActive so they appear/disappear properly
            _blockAnim.SetBool("isActive", state);
        }
        isActive = state;

        // Update indicator visibility only for levers
        if (blockType.ToLower() == "lever" && indicator != null)
        {
            indicator.SetActive(state);
        }
    }

    public virtual void Pull(bool state)
    {
        if (_blockAnim != null)
        {
            _blockAnim.SetBool(SwitchOn, state);
        }
        switchOn = state;

        // Update indicator color only for levers
        if (blockType.ToLower() == "lever" && indicatorRenderer != null)
        {
            UpdateIndicatorColor();
        }
    }

    private void UpdateIndicatorColor()
    {
        if (indicatorRenderer != null)
        {
            Color color = switchOn ? Color.green : Color.red;
            indicatorRenderer.material.SetColor("_EmissionColor", color * 2f); // Ensure emission is visible
        }
    }

    public string GetBlockType()
    {
        return blockType;
    }
}
