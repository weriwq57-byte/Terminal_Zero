using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private float healthRatio = 1f;
    private string ammoText = "12 / 60";
    private string waveText = "WAVE 1/3";
    private string scoreText = "SCORE: 0";
    private string messageText = "";
    private float messageTimer;
    private string finalScoreText = "";

    private enum ScreenState { HUD, Pause, GameOver, Win }
    private ScreenState screen = ScreenState.HUD;

    private GUIStyle labelStyle;
    private GUIStyle bigStyle;
    private GUIStyle buttonStyle;
    private bool stylesInitialized;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    void InitStyles()
    {
        if (stylesInitialized) return;
        stylesInitialized = true;

        labelStyle = new GUIStyle();
        labelStyle.fontSize = 22;
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleLeft;

        bigStyle = new GUIStyle(labelStyle);
        bigStyle.fontSize = 48;
        bigStyle.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle();
        buttonStyle.fontSize = 26;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f));
        buttonStyle.hover.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f));
        buttonStyle.alignment = TextAnchor.MiddleCenter;
    }

    Texture2D MakeTex(int w, int h, Color c)
    {
        Texture2D t = new Texture2D(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                t.SetPixel(x, y, c);
        t.Apply();
        return t;
    }

    void OnGUI()
    {
        InitStyles();

        switch (screen)
        {
            case ScreenState.HUD: DrawHUD(); break;
            case ScreenState.Pause: DrawPause(); break;
            case ScreenState.GameOver: DrawEndScreen("GAME OVER"); break;
            case ScreenState.Win: DrawEndScreen("YOU ESCAPED"); break;
        }
    }

    void DrawHUD()
    {
        float sw = Screen.width, sh = Screen.height;

        // Health bar
        float hbX = sw * 0.02f, hbY = sh * 0.88f, hbW = sw * 0.2f, hbH = sh * 0.04f;
        DrawBar(hbX, hbY, hbW, hbH, healthRatio, Color.red, new Color(0.1f, 0.1f, 0.1f));

        // Ammo
        GUI.Label(new Rect(sw * 0.02f, sh * 0.83f, sw * 0.15f, sh * 0.04f), ammoText, labelStyle);

        // Wave
        GUI.Label(new Rect(sw * 0.45f, sh * 0.93f, sw * 0.1f, sh * 0.05f), waveText, bigStyle);

        // Score
        labelStyle.alignment = TextAnchor.MiddleRight;
        GUI.Label(new Rect(sw * 0.8f, sh * 0.93f, sw * 0.18f, sh * 0.05f), scoreText, labelStyle);
        labelStyle.alignment = TextAnchor.MiddleLeft;

        // Center message
        if (messageTimer > 0)
        {
            GUI.color = new Color(1, 0.3f, 0.1f, Mathf.Min(1, messageTimer));
            bigStyle.fontSize = 48;
            GUI.Label(new Rect(0, sh * 0.4f, sw, sh * 0.1f), messageText, bigStyle);
            GUI.color = Color.white;
        }
    }

    void DrawPause()
    {
        DrawOverlay(new Color(0, 0, 0, 0.5f));

        bigStyle.fontSize = 48;
        GUI.Label(new Rect(0, Screen.height * 0.35f, Screen.width, 60), "PAUSED", bigStyle);

        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f, Screen.width * 0.2f, 50), "RESUME", buttonStyle))
            TogglePause();

        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.6f, Screen.width * 0.2f, 50), "QUIT", buttonStyle))
            Application.Quit();
    }

    void DrawEndScreen(string title)
    {
        DrawOverlay(new Color(0, 0, 0, 0.7f));

        bigStyle.fontSize = 52;
        GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, 60), title, bigStyle);

        labelStyle.fontSize = 36;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, Screen.height * 0.4f, Screen.width, 50), finalScoreText, labelStyle);
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.fontSize = 22;

        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.55f, Screen.width * 0.2f, 50), "RESTART", buttonStyle))
        {
            if (GameManager.Instance) GameManager.Instance.RestartGame();
        }

        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.65f, Screen.width * 0.2f, 50), "QUIT", buttonStyle))
            Application.Quit();
    }

    void DrawBar(float x, float y, float w, float h, float ratio, Color fill, Color bg)
    {
        GUI.color = bg;
        GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
        GUI.color = fill;
        GUI.DrawTexture(new Rect(x, y, w * ratio, h), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void DrawOverlay(Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void Update()
    {
        if (messageTimer > 0) messageTimer -= Time.deltaTime;
    }

    // Public API
    public void UpdateHealth(float ratio) { healthRatio = ratio; }
    public void UpdateAmmo(int current, int reserve) { ammoText = $"{current} / {reserve}"; }
    public void UpdateWave(int current, int total) { waveText = $"WAVE {current}/{total}"; }
    public void UpdateScore(int score) { scoreText = $"SCORE: {score}"; }

    public void ShowMessage(string msg)
    {
        messageText = msg;
        messageTimer = 2f;
    }

    public void TogglePause()
    {
        if (screen == ScreenState.Pause)
        {
            screen = ScreenState.HUD;
            Time.timeScale = 1f;
        }
        else if (screen == ScreenState.HUD)
        {
            screen = ScreenState.Pause;
            Time.timeScale = 0f;
        }
    }

    public void ShowGameOver(float score)
    {
        finalScoreText = $"FINAL SCORE: {Mathf.RoundToInt(score)}";
        screen = ScreenState.GameOver;
    }

    public void ShowWin(float score)
    {
        finalScoreText = $"FINAL SCORE: {Mathf.RoundToInt(score)}";
        screen = ScreenState.Win;
    }
}
