using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public DroneController droneController;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI stopwatchText;
    public TextMeshProUGUI checkpointText;
    public Image fadeImage;
    public AudioSource countdownBeepSource; // ✅ Add this

    public float countdownDuration = 3f;
    public float fadeDuration = 1.5f;

    float countdownTimer;
    float stopwatchTimer;
    bool stopwatchRunning = false;
    int lastDisplayedSecond = -1; // ✅ Track last displayed second

    void Start()
    {
        countdownTimer = countdownDuration;
        droneController.gameStarted = false;

        countdownText.gameObject.SetActive(true);
        stopwatchText.gameObject.SetActive(false);
        checkpointText.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!droneController.gameStarted && countdownTimer > 0)
        {
            countdownTimer -= Time.deltaTime;

            int currentSecond = Mathf.CeilToInt(countdownTimer);

            // ✅ Play beep if second changed
            if (currentSecond != lastDisplayedSecond && currentSecond > 0)
            {
                lastDisplayedSecond = currentSecond;
                if (countdownBeepSource != null)
                    countdownBeepSource.Play();
            }

            countdownText.text = currentSecond.ToString();

            if (countdownTimer <= 0)
            {
                countdownText.gameObject.SetActive(false);
                stopwatchText.gameObject.SetActive(true);
                droneController.gameStarted = true;
                stopwatchRunning = true;
            }
        }
        if (stopwatchRunning)
        {
            stopwatchTimer += Time.deltaTime;
            stopwatchText.text = FormatTime(stopwatchTimer);
        }
    }

    public void StopStopwatch()
    {
        stopwatchRunning = false;
        string stopwatchFinalTime = FormatTime(stopwatchTimer);
        stopwatchText.text = "Finished at: " + stopwatchFinalTime;
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void ShowCheckpointMessage(string message)
    {
        StartCoroutine(ShowCheckpointMessageRoutine(message));
    }

    IEnumerator ShowCheckpointMessageRoutine(string message)
    {
        checkpointText.text = message;
        checkpointText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        checkpointText.gameObject.SetActive(false);
    }

    public void RespawnWithFade(Vector3 respawnPos, Quaternion respawnRot)
    {
        StartCoroutine(RespawnSequence(respawnPos, respawnRot));
    }

    IEnumerator RespawnSequence(Vector3 respawnPos, Quaternion respawnRot)
    {
        stopwatchText.gameObject.SetActive(true);
        droneController.gameStarted = false;
        countdownText.gameObject.SetActive(false);
        checkpointText.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(true);

        yield return StartCoroutine(FadeBlack(0f, 1f, fadeDuration));

        droneController.transform.position = respawnPos;
        droneController.transform.rotation = respawnRot;

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FadeBlack(1f, 0f, fadeDuration));
        fadeImage.gameObject.SetActive(false);

        countdownTimer = 3;
        lastDisplayedSecond = -1; // ✅ Reset second tracker
        countdownText.gameObject.SetActive(true);
    }

    IEnumerator FadeBlack(float from, float to, float duration)
    {
        Color c = fadeImage.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}
