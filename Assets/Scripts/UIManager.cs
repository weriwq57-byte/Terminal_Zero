using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider healthSlider;
    public Text ammoText;
    public Text waveText;
    public Text scoreText;
    public Text messageText;
    public Text finalScoreText;

    private Button restartButton;
    private Button quitButton;
    private bool isPaused;
    private bool gameEnded;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        // Find buttons by name
        GameObject rbGO = GameObject.Find("RestartButton");
        if (rbGO != null) restartButton = rbGO.GetComponent<Button>();
        GameObject qbGO = GameObject.Find("QuitButton");
        if (qbGO != null) quitButton = qbGO.GetComponent<Button>();
    }

    public void UpdateHealth(float ratio)
    {
        if (healthSlider) healthSlider.value = ratio;
    }

    public void UpdateAmmo(int current, int reserve)
    {
        if (ammoText) ammoText.text = $"{current} / {reserve}";
    }

    public void UpdateWave(int current, int total)
    {
        if (waveText) waveText.text = $"WAVE {current}/{total}";
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"SCORE: {score}";
    }

    public void ShowMessage(string msg)
    {
        if (messageText == null) return;
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 2f);
    }

    void HideMessage()
    {
        if (messageText) messageText.gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        if (gameEnded) return;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ShowGameOver(float score)
    {
        gameEnded = true;
        ShowEndScreen("GAME OVER", score);
    }

    public void ShowWin(float score)
    {
        gameEnded = true;
        ShowEndScreen("YOU ESCAPED", score);
    }

    void ShowEndScreen(string title, float score)
    {
        if (messageText)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = title;
            messageText.fontSize = 52;
        }

        if (finalScoreText)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = $"FINAL SCORE: {Mathf.RoundToInt(score)}";
        }

        if (restartButton) restartButton.gameObject.SetActive(true);
        if (quitButton) quitButton.gameObject.SetActive(true);
    }

    public void ResumeButton() { isPaused = false; Time.timeScale = 1f; }
    public void RestartButton() { Time.timeScale = 1f; if (GameManager.Instance) GameManager.Instance.RestartGame(); }
    public void QuitButton() { Application.Quit(); }
}
