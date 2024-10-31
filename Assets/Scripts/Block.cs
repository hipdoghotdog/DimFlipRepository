using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isActive = true;
    private Animator blockAnim;

    public string blockType;

    void Start(){
        blockAnim = gameObject.GetComponent<Animator>();
    }

    virtual public void activate(bool b) {
        blockAnim.SetBool("isActive", b);
        isActive = b;
    }

    public string GetBlockType(){
        return blockType;
    }
}
