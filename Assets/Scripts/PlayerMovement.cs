using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private GameManager gameManager; // Reference to GameManager
    private GameManager.View currentView;
    private float xInput;
    private float yInput;
    private float zInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    void FixedUpdate()
    {
        // Get the current view from the GameManager
        currentView = gameManager.currentView;

        // Reset inputs
        xInput = 0f;
        yInput = 0f;
        zInput = 0f;

        if (currentView == GameManager.View.SideView)
        {
            // Side view controls
            xInput = Input.GetAxis("Horizontal");
            yInput = Input.GetAxis("Jump");
        }
        else // TopdownView
        {
            // Top-down view controls
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");
        }

        // Apply movement
        Vector3 movement = new Vector3(xInput * speed, yInput * speed, zInput * speed);
        rb.AddForce(movement);
    }
}
