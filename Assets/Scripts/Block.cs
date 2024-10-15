using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isActive = true;
    private Animator blockAnim;

    void Start(){
        blockAnim = gameObject.GetComponent<Animator>();
    }

    public void activate(bool b) {
        blockAnim.SetBool("isActive", b);
        isActive = b;
    }
}
