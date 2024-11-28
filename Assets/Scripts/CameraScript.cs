using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Assign the player Transform in the Inspector
    public Animator camAnimator; // Assign the camera Animator in the Inspector

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public Vector3 sideViewOffset = new Vector3(0, 5, -10);
    public Vector3 topViewOffset = new Vector3(0, 20, 0);

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("Transition Settings")]
    public float transitionDuration = 1f; // Duration of the flip animation

    private Vector3 targetOffset;
    private Quaternion targetRotation;
    private bool isTransitioning = false;
    private GameManager.View currentView;

    void Start()
    {
        // Initialize current view based on GameManager's currentView
        GameManager gameManager = FindObjectOfType<GameManager>();
        currentView = gameManager.currentView;

        // Set initial offset and rotation
        if (currentView == GameManager.View.SideView)
        {
            targetOffset = sideViewOffset;
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            targetOffset = topViewOffset;
            targetRotation = Quaternion.Euler(90, 0, 0); // Adjust as needed for top view
        }

        // Initialize camera position and rotation
        transform.position = player.position + targetOffset;
        transform.rotation = targetRotation;
    }

    void LateUpdate()
    {
        // Smoothly follow the player
        Vector3 desiredPosition = player.position + targetOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate towards target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Initiates a view flip to the specified view.
    /// </summary>
    /// <param name="newView">The target view (SideView or TopdownView).</param>
    public void FlipView(GameManager.View newView)
    {
        if (isTransitioning || newView == currentView)
            return;

        StartCoroutine(FlipTransition(newView));
    }

    /// <summary>
    /// Handles the transition between views.
    /// </summary>
    /// <param name="newView">The target view.</param>
    /// <returns></returns>
    private IEnumerator FlipTransition(GameManager.View newView)
    {
        isTransitioning = true;

        // Trigger the appropriate flip animation
        if (newView == GameManager.View.TopdownView)
        {
            camAnimator.SetTrigger("FlipToTopView");
        }
        else
        {
            camAnimator.SetTrigger("FlipToSideView");
        }

        // Optionally, wait for half the transition duration before changing offset
        yield return new WaitForSeconds(transitionDuration / 2f);

        // Update the target offset and rotation based on the new view
        if (newView == GameManager.View.SideView)
        {
            targetOffset = sideViewOffset;
            targetRotation = Quaternion.Euler(0, 0, 0); // Adjust as needed for side view
        }
        else
        {
            targetOffset = topViewOffset;
            targetRotation = Quaternion.Euler(90, 0, 0); // Adjust as needed for top view
        }

        // Wait for the remaining transition duration
        yield return new WaitForSeconds(transitionDuration / 2f);

        // Update the current view
        currentView = newView;
        isTransitioning = false;
    }
}
