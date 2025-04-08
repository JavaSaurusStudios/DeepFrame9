using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;            // The target for the camera to follow
    public Vector3 offset = new Vector3(0, 5, -10); // Default offset from target
    public float smoothSpeed = 0.125f;  // Smoothness factor (lower = smoother)

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position based on target and offset
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = 0;

        // Smoothly interpolate between current and desired position
        Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the position
        transform.position = smoothedPosition;


    }
}
