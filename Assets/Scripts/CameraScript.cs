using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform; // Assign the player Transform in the Inspector
    public Animator camAnimator; // Assign the camera Animator in the Inspector

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public Vector3 sideViewOffset = new(0, 5, -10);
    public Vector3 topViewOffset = new(0, 20, 0);

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("Transition Settings")]
    public float transitionDuration = 1f; // Duration of the flip animation

    private Vector3 _targetOffset;
    private Quaternion _targetRotation;
    private bool _isTransitioning;

    private GameManager.View _currentView;
    
    private void Start()
    {
        playerTransform = GameManager.Instance.player.transform;
        
        // Initialize current view based on GameManager's currentView
        _currentView = GameManager.Instance.currentView;

        // Set initial offset and rotation
        if (_currentView == GameManager.View.SideView)
        {
            _targetOffset = sideViewOffset;
            _targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            _targetOffset = topViewOffset;
            _targetRotation = Quaternion.Euler(90, 0, 0); // Adjust as needed for top view
        }

        // Initialize camera position and rotation
        transform.position = playerTransform.position + _targetOffset;
        transform.rotation = _targetRotation;
    }

    private void LateUpdate()
    {
        // Smoothly follow the player
        Vector3 desiredPosition = playerTransform.position + _targetOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate towards target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Initiates a view flip to the specified view.
    /// </summary>
    /// <param name="newView">The target view (SideView or TopdownView).</param>
    public void FlipView(GameManager.View newView)
    {
        if (_isTransitioning || newView == _currentView)
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
        _isTransitioning = true;

        // Trigger the appropriate flip animation
        camAnimator.SetTrigger(newView == GameManager.View.TopdownView 
            ? "FlipToTopView" 
            : "FlipToSideView");

        // Optionally, wait for half the transition duration before changing offset
        yield return new WaitForSeconds(transitionDuration / 2f);

        // Update the target offset and rotation based on the new view
        if (newView == GameManager.View.SideView)
        {
            _targetOffset = sideViewOffset;
            _targetRotation = Quaternion.Euler(0, 0, 0); // Adjust as needed for side view
        }
        else
        {
            _targetOffset = topViewOffset;
            _targetRotation = Quaternion.Euler(90, 0, 0); // Adjust as needed for top view
        }

        // Wait for the remaining transition duration
        yield return new WaitForSeconds(transitionDuration / 2f);

        // Update the current view
        _currentView = newView;
        _isTransitioning = false;
    }
}
