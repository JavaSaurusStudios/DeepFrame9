using System.Collections;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public AudioSource timerAudio;

    public TMPro.TextMeshProUGUI timerText; // Assign in Inspector
    [SerializeField]
    private float startMinutes = 1;

    public delegate void OnTimerOut();
    public OnTimerOut onTimerOut;

    public void StartTimer()
    {
        StartCoroutine(CountdownCoroutine(startMinutes));
    }

    IEnumerator CountdownCoroutine(float minutes)
    {
        float totalTime = minutes * 60f; // Convert minutes to seconds

        int amount = 5;
        while (totalTime > 0)
        {
            int displayMinutes = Mathf.FloorToInt(totalTime / 60f);
            int displaySeconds = Mathf.FloorToInt(totalTime % 60f);
            int displayFrames = Mathf.FloorToInt((totalTime % 1f) * 60); // Approximate frames at 60 FPS

            string timeString = string.Format("{0:00} : {1:00} : {2:00}", displayMinutes, displaySeconds, displayFrames);

            if (timerText != null)
                timerText.text = timeString;

            totalTime -= Time.deltaTime;

            if (displaySeconds <= amount && amount >= 0)
            {
                amount--;
                timerAudio.PlayOneShot(timerAudio.clip);
            }

            yield return null;
        }
        totalTime = 0;
        // Final time (00 : 00 : 00)
        if (timerText != null) timerText.text = "00 : 00 : 00";
        onTimerOut?.Invoke();
    }
}