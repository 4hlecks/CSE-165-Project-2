using UnityEngine;
using System.Collections.Generic;

public class ArrowPointer : MonoBehaviour
{
    public Camera xrCamera;
    public RectTransform arrowImage;
    public List<Transform> checkpoints;

    public int currentCheckpoint = 0;

    void Update()
    {
        if (!xrCamera) return;

        // Step 1: Position arrow UI in front of camera
        Vector3 offset = xrCamera.transform.forward * 0.5f + xrCamera.transform.up * -0.35f;
        transform.position = xrCamera.transform.position + offset;

        // Step 2: Make the canvas face the camera (upright)
        Vector3 forward = xrCamera.transform.forward;
        forward.y = 0;
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        // Step 3: Point the arrow to next checkpoint (if valid)
        if (checkpoints != null && currentCheckpoint < checkpoints.Count)
        {
            Vector3 toCheckpoint = checkpoints[currentCheckpoint].position - xrCamera.transform.position;
            toCheckpoint.y = 0;

            Vector3 cameraForwardFlat = xrCamera.transform.forward;
            cameraForwardFlat.y = 0;

            float angle = Vector3.SignedAngle(cameraForwardFlat.normalized, toCheckpoint.normalized, Vector3.up);

            arrowImage.localEulerAngles = new Vector3(90, 0, 90 - angle);
            arrowImage.gameObject.SetActive(true);
        }
        else
        {
            // ✅ No more checkpoints → hide arrow
            arrowImage.gameObject.SetActive(false);
        }
    }

    public void AdvanceToNextCheckpoint()
    {
        if (currentCheckpoint < checkpoints.Count)
        {
            currentCheckpoint++;
            Debug.Log($"Advanced to checkpoint index: {currentCheckpoint}");
        }

        if (currentCheckpoint >= checkpoints.Count)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.StopStopwatch();
        }
    }

}
