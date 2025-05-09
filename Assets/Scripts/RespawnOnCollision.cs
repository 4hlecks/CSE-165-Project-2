using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    public ArrowPointer arrowPointer;  // Reference to ArrowPointer
    public string environmentTag = "Environment"; // Tag your environment mesh as "Environment"

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(environmentTag) && arrowPointer != null)
        {
            Debug.Log("Triggered environment → respawning");

            int respawnIndex = Mathf.Max(arrowPointer.currentCheckpoint - 1, 0);
            Debug.Log($"Respawning at checkpoint index: {respawnIndex}");

            if (respawnIndex < arrowPointer.checkpoints.Count)
            {
                Vector3 respawnPos = arrowPointer.checkpoints[respawnIndex].position;
                Quaternion respawnRot;
                if (respawnIndex < arrowPointer.checkpoints.Count - 1)
                {
                    Vector3 dir = arrowPointer.checkpoints[respawnIndex + 1].position - arrowPointer.checkpoints[respawnIndex].position;
                    dir.y = 0; // flatten
                    respawnRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                }
                else
                {
                    // if last checkpoint → keep checkpoint's rotation or fallback
                    respawnRot = arrowPointer.checkpoints[respawnIndex].rotation;
                }


                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.RespawnWithFade(respawnPos, respawnRot);
                }
                else
                {
                    Debug.LogWarning("GameManager not found → teleporting without fade.");
                    transform.position = respawnPos;
                    transform.rotation = respawnRot;
                }
            }
            else
            {
                Debug.LogWarning($"Respawn index {respawnIndex} is out of bounds!");
            }
        }
    }
}
