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

    private void Start()
    {
        _blockAnim = GetComponent<Animator>();
        _meshRenderer = GetComponent<MeshRenderer>();
        ApplyTheme();

        // Initialize pushable and pullable based on blockType
        if (blockType.ToLower() == "pushable") isPushable = true;
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
        }
        isActive = state;
    }

    public virtual void Pull(bool state)
    {
        if (_blockAnim != null)
        {
            _blockAnim.SetBool(SwitchOn, state);
        }
        switchOn = state;
    }

    public string GetBlockType()
    {
        return blockType;
    }
}
