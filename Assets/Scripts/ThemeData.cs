using UnityEngine;

[CreateAssetMenu(menuName = "Game/Theme")]
public class ThemeData : ScriptableObject
{
    public Material blockMaterial;
    public Material startMaterial;
    public Material endMaterial;
    public Material ladderMaterial;
    public Material leverMaterial;

    public GameObject blockModel;
    public GameObject emptyModel;
    public GameObject startModel;
    public GameObject endModel;
    public GameObject ladderModel;
    public GameObject leverModel;
    public GameObject blockwlightModel;
}
