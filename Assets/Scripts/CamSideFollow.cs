using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSideFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    private readonly Vector3 _offset = new(0, -1, -10);
    private void FixedUpdate()
    {
        transform.position = target.position + _offset;
    }
}
