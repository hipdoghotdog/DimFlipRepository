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
            blockAnim.SetBool("isActive", state);
        }
        isActive = state;
    }

    public virtual void Pull(bool state)
    {
        if (blockAnim != null)
        {
            blockAnim.SetBool("switchOn", state);
        }
        switchOn = state;
    }

    public string GetBlockType()
    {
        return blockType;
    }
}
