using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("XR Origin (camera rig) – usually the child of this object.")]
    public Transform xrOrigin;          // Drag XR Origin here if NOT a child.
    [Tooltip("Main camera inside XR Origin. Used as the pivot for RotateAround.")]
    public Transform headTransform;     // Drag Main Camera here.

    [Header("Motion")]
    public float forwardSpeed = 2f;   // metres/second
    public float yawSpeedDeg = 45f;  // degrees/second (left/right)
    public float pitchSpeedDeg = 30f;  // degrees/second (up/down)

    bool _turnLeft, _turnRight, _pitchUp, _pitchDown;
    public bool gameStarted = false;

    Transform Target => xrOrigin != null ? xrOrigin : transform;

    void Update()
    {
        if (!gameStarted) return;

        // Continuous forward motion in local Z
        Target.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.Self);

        // --- Yaw (turn left/right)
        float yawDir = (_turnRight ? 1f : 0f) + (_turnLeft ? -1f : 0f);
        if (Mathf.Abs(yawDir) > 0.01f)
            RotateAroundHead(Vector3.up, yawDir * yawSpeedDeg * Time.deltaTime);

        // --- Pitch (look up/down)
        float pitchDir = (_pitchUp ? -1f : 0f) + (_pitchDown ? 1f : 0f);
        if (Mathf.Abs(pitchDir) > 0.01f)
            RotateAroundHead(transform.right, pitchDir * pitchSpeedDeg * Time.deltaTime);
    }

    /* -------- called from StaticHandGesture UnityEvents -------- */

    public void StartTurnLeft() => _turnLeft = true;
    public void StopTurnLeft() => _turnLeft = false;

    public void StartTurnRight() => _turnRight = true;
    public void StopTurnRight() => _turnRight = false;

    public void StartPitchUp() => _pitchUp = true;
    public void StopPitchUp() => _pitchUp = false;

    public void StartPitchDown() => _pitchDown = true;
    public void StopPitchDown() => _pitchDown = false;

    /* ----------------------------------------------------------- */

    void RotateAroundHead(Vector3 axis, float angleDeg)
    {
        if (headTransform == null)
        {
            // Fallback: rotate around self
            Target.Rotate(axis, angleDeg, Space.World);
            return;
        }

        var pivot = new Vector3(headTransform.position.x, headTransform.position.y, headTransform.position.z);
        Target.RotateAround(pivot, axis, angleDeg);
    }
}
