using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class LevelFlipper : MonoBehaviour
{
    public GameObject[,,] level;

    public void SetLevel(GameObject[,,] l)
    {
        level = l;
    }

    public void flipLevel(Vector3 pp, GameManager.View view)
    {
        if (level == null)
        {
            Debug.LogError("LevelFlipper: Level array is null. Ensure that SetLevel() is called before flipLevel().");
            return;
        }

        int ppx = Mathf.RoundToInt(pp.x);
        int ppy = Mathf.RoundToInt(pp.y);
        int ppz = Mathf.RoundToInt(pp.z);

        for (int i = 0; i < level.GetLength(0); i++)
        {
            for (int k = 0; k < level.GetLength(1); k++)
            {
                for (int j = 0; j < level.GetLength(2); j++)
                {
                    GameObject blockObj = level[i, k, j];

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

                    if (view == GameManager.View.SideView)
                    {
                        bool onHeightPlane = Mathf.RoundToInt(blockObj.transform.position.y) == ppy;
                        blockComponent.Activate(onHeightPlane);
                    }
                    else // TopdownView
                    {
                        bool onSidePlane = Mathf.RoundToInt(blockObj.transform.position.z) == ppz;
                        blockComponent.Activate(onSidePlane);
                    }
                }
            }
        }
    }
}
