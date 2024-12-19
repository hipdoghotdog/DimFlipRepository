using UnityEngine;

public class Block : MonoBehaviour
{
    public bool switchOn = false;
    public bool isActive = true;
    private Animator blockAnim;
    private MeshRenderer meshRenderer;
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

    void Start()
    {
        blockAnim = GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
        ApplyTheme();

        // Initialize pushable and pullable based on blockType
        switch (blockType.ToLower())
        {
            case "pushable":
                isPushable = true;
                break;
            case "lever":
                isPullable = true;
                break;
                // Add other block types if necessary
        }

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
        if (theme != null && meshRenderer != null)
        {
            switch (blockType.ToLower())
            {
                case "block":
                case "blockwlight":
                case "lever":
                    meshRenderer.material = theme.blockMaterial;
                    break;
                case "empty":
                    meshRenderer.material = null;
                    meshRenderer.enabled = false;
                    break;
                case "start":
                    meshRenderer.material = theme.startMaterial;
                    break;
                case "end":
                    meshRenderer.material = theme.endMaterial;
                    break;
                case "right ladder":
                case "left ladder":
                    meshRenderer.material = theme.ladderMaterial;
                    break;
                case "pushable":
                    meshRenderer.material = theme.pushableMaterial;
                    break;
                case "story":
                    meshRenderer.material = theme.storyMaterial;
                    break;
                default:
                    Debug.LogWarning($"Block: Unknown blockType '{blockType}'. No material applied.");
                    break;
            }
        }
    }

    public virtual void Activate(bool state)
    {
        if (blockAnim != null)
        {
            // For ALL blocks, including levers, set isActive so they appear/disappear properly
            blockAnim.SetBool("isActive", state);
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
        if (blockAnim != null)
        {
            blockAnim.SetBool("switchOn", state);
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
