using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    public enum GameState
    {
        WAITING, STARTING, PLAYING, GAME_OVER
    }

    public GameState state = GameState.WAITING;

    public CountDownTimer countDownTimer;

    public GameObject GameOverScreen;
    public TMPro.TextMeshProUGUI endScoreField;
    public TMPro.TextMeshProUGUI highScoreField;
    public Button RestartButton;

    public GameObject SettingsScreen;

    void Awake()
    {
        Time.timeScale = 1;
        INSTANCE = this;
        state = GameState.WAITING;
        countDownTimer.timerText.text = "Touch anywhere to start...";
    }

    void Start()
    {
        countDownTimer.onTimerOut += () =>
        {
            state = GameState.GAME_OVER;

            Time.timeScale = 0;
            GameOverScreen.SetActive(true);
            int HighScore = PlayerPrefs.GetInt("HighScore", 0);
            int CurrentScore = DepthTracker.INSTANCE.depth;
            if (CurrentScore > HighScore)
            {
                PlayerPrefs.SetInt("HighScore", CurrentScore);
                PlayerPrefs.Save();
            }
            endScoreField.text = CurrentScore.ToString("D4") + " meters";
            highScoreField.enabled = CurrentScore > HighScore;
        };

        RestartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

    }

    public void StartGame()
    {
        if (state == GameState.WAITING)
        {
            StartCoroutine(StartGameCountDown());
        }
    }

    IEnumerator StartGameCountDown()
    {
        SettingsScreen.SetActive(false);
        state = GameState.STARTING;
        countDownTimer.timerText.text = "3";
        countDownTimer.timerAudio.PlayOneShot(countDownTimer.timerAudio.clip);
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        countDownTimer.timerText.text = "2";
        countDownTimer.timerAudio.PlayOneShot(countDownTimer.timerAudio.clip);
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        countDownTimer.timerText.text = "1";
        countDownTimer.timerAudio.PlayOneShot(countDownTimer.timerAudio.clip);
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        countDownTimer.timerAudio.PlayOneShot(countDownTimer.timerAudio.clip);
        countDownTimer.timerText.text = "Start !";
        state = GameState.PLAYING;
        yield return new WaitForSeconds(.5f);
        yield return new WaitForEndOfFrame();
        countDownTimer.StartTimer();
    }


    public void OpenTwitchPage()
    {
        Application.OpenURL("https://twitch.tv/drjavasaurus");
    }

}
