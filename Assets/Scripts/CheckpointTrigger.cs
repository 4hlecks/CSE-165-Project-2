using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointIndex;
    private ArrowPointer arrowPointer;

    [Header("Visual feedback")]
    public Material clearedMaterial;
    public AudioSource checkpointAudioSource;
    private Renderer checkpointRenderer;

    void Start()
    {
        arrowPointer = FindObjectOfType<ArrowPointer>();

        checkpointRenderer = GetComponentInChildren<Renderer>();
        if (checkpointRenderer == null)
            Debug.LogWarning($"Checkpoint {checkpointIndex}: No Renderer found!");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered Checkpoint {checkpointIndex}");
        if (other.CompareTag("Player") && arrowPointer != null)
        {
            Debug.Log($"Current Checkpoint: {arrowPointer.currentCheckpoint}");

            if (arrowPointer.currentCheckpoint == checkpointIndex)
            {
                arrowPointer.AdvanceToNextCheckpoint();
                if (checkpointRenderer != null && clearedMaterial != null)
                {
                    checkpointRenderer.material = clearedMaterial;
                }

                if (checkpointAudioSource != null && checkpointAudioSource.clip != null)
                {
                    checkpointAudioSource.Play();
                }

                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null)
                {
                    gm.ShowCheckpointMessage($"You passed Checkpoint {checkpointIndex + 1}!");
                }
            }
        }
    }
}
