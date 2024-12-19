using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTopFollow : MonoBehaviour
{

    // Start is called before the first frame update
    public Transform target;
    Vector3 offset = new  Vector3(0, 10, 0);
    private void FixedUpdate()
    {
        transform.position = target.position + offset;
    }
}
