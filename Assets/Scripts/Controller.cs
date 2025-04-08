using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Vector2 startMousePosition;  // The position where the mouse button was first pressed
    [SerializeField]
    private Vector2 currentMousePosition;  // The position where the mouse button is now pressed
    [SerializeField]
    private float holdDuration = 0f;     // How long the mouse button has been held down
    [SerializeField]
    private float dragDistance;         // The distance the player is dragging
    [SerializeField]
    private bool isDragging = false;     // Whether the player is currently dragging the mouse
    [SerializeField]
    private Vector3 dragDirection;       // The direction of the mouse drag

    public delegate void OnDragUpdate(bool isDragging, float holdDuration, Vector3 startMousePosition, Vector3 currentMousePosition, float dragDistance, Vector3 dragDirection);
    public OnDragUpdate onDragUpdate;

    public delegate void OnLaunch(float dragDistance, Vector3 dragDirection);
    public OnLaunch onLaunch;

    void Update()
    {
        if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING) return;
        // Check if the mouse button is being pressed
        if (Input.GetMouseButtonDown(0))  // 0 is the left mouse button
        {
            startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            holdDuration = 0f;  // Reset the hold duration when the button is first pressed
            isDragging = true;   // Start tracking the drag
        }

        // Check if the mouse button is being held down
        if (Input.GetMouseButton(0) && isDragging)
        {
            holdDuration += Time.deltaTime;  // Increase the hold duration as long as the button is held

            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragDirection = -(currentMousePosition - startMousePosition);  // Calculate the direction of the drag
            dragDirection.z = 0;
            dragDistance = dragDirection.magnitude;
        }

        // When the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;  // Stop tracking the drag
            onLaunch?.Invoke(dragDistance, dragDirection.normalized);
        }

        onDragUpdate?.Invoke(isDragging, holdDuration, startMousePosition, currentMousePosition, dragDistance, dragDirection.normalized);
    }
}
