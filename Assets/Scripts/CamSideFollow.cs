using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSideFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    Vector3 offset = new Vector3(0, -1, -10);
    private void FixedUpdate()
    {
        transform.position = target.position + offset;
    }
}
