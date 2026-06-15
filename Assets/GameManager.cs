using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioSource sfxSource;

    public AudioClip matchSound;
    public AudioClip levelCompleteSound;
    public AudioClip levelFailedSound;

    public int score = 0;
    public int goal = 100;
    public int level = 1;

    public float timeLeft = 50f;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI highScoreText;

    public GameObject levelCompleteUI;
    public GameObject levelFailedUI;
    public GameObject pauseUI;

    public TextMeshProUGUI completeGoalText;
    public TextMeshProUGUI completeScoreText;

    public TextMeshProUGUI failedGoalText;
    public TextMeshProUGUI failedScoreText;

    private bool isGameEnded = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);

        if (levelFailedUI != null)
            levelFailedUI.SetActive(false);

        if (pauseUI != null)
            pauseUI.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (isGameEnded) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            EndGame();
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    // =======================
    // PAUSE SYSTEM
    // =======================

    public void PauseGame()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // =======================

    void EndGame()
    {
        isGameEnded = true;

        SaveHighScore();

        if (score >= goal)
        {
            levelCompleteUI.SetActive(true);

            if (completeGoalText != null)
                completeGoalText.text = "Goal : " + goal;

            if (completeScoreText != null)
                completeScoreText.text = "Your Score : " + score;

            if (levelCompleteSound != null)
                sfxSource.PlayOneShot(levelCompleteSound);

            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            if (level >= unlockedLevel)
            {
                PlayerPrefs.SetInt("UnlockedLevel", level + 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            levelFailedUI.SetActive(true);

            if (failedGoalText != null)
                failedGoalText.text = "Goal : " + goal;

            if (failedScoreText != null)
                failedScoreText.text = "Your Score : " + score;

            if (levelFailedSound != null)
                sfxSource.PlayOneShot(levelFailedSound);
        }

        Time.timeScale = 0f;
    }

    // =======================
    // BUTTON FUNCTION
    // =======================

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    // =======================

    void SaveHighScore()
    {
        int currentHigh = PlayerPrefs.GetInt("HighScore", 0);

        if (score > currentHigh)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (goalText != null)
            goalText.text = "Goal: " + goal;

        if (levelText != null)
            levelText.text = "Level: " + level;

        if (timeText != null)
            timeText.text = "Time: " + Mathf.Ceil(timeLeft);

        if (highScoreText != null)
            highScoreText.text = "High: " + GetHighScore();
    }
}