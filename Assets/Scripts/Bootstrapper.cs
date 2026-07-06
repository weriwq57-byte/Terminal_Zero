using UnityEngine;
using UnityEngine.UI;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            return;

        SetupCamera();
        SetupFloor();
        SetupWalls();
        SetupPlayer();
        SetupSpawnPoints();
        SetupDoors();
        SetupUI();
        SetupManagers();
        SetupPrefabs();
    }

    static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject go = new GameObject("MainCamera", typeof(Camera), typeof(AudioListener));
            cam = go.GetComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 8f;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    static void SetupFloor()
    {
        GameObject floor = new GameObject("Floor");
        SpriteRenderer sr = floor.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(2, 2, Color.gray);
        sr.color = new Color(0.25f, 0.25f, 0.27f);
        sr.sortingOrder = -10;
        floor.transform.localScale = new Vector3(20, 12, 1);
    }

    static void SetupWalls()
    {
        Color c = new Color(0.12f, 0.12f, 0.14f);
        CreateWall(new Vector2(0, -6.3f), new Vector2(20f, 0.6f), c);
        CreateWall(new Vector2(0, 6.3f), new Vector2(20f, 0.6f), c);
        CreateWall(new Vector2(-10.3f, 0), new Vector2(0.6f, 12f), c);
        CreateWall(new Vector2(10.3f, 0), new Vector2(0.6f, 12f), c);
    }

    static void CreateWall(Vector2 pos, Vector2 size, Color color)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = pos;
        wall.tag = "Wall";
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(2, 2, color);
        sr.color = color;
        wall.transform.localScale = Vector3.one;
    }

    static void SetupPlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(8, 8, Color.blue);
        sr.color = new Color(0.2f, 0.4f, 0.9f);
        sr.sortingOrder = 10;
        player.transform.localScale = Vector3.one * 0.4f;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearDamping = 8f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = player.AddComponent<CircleCollider2D>();
        col.radius = 1f;

        PlayerController pc = player.AddComponent<PlayerController>();

        GameObject fp = new GameObject("FirePoint");
        fp.transform.SetParent(player.transform);
        fp.transform.localPosition = new Vector3(0, 1.5f, 0);
        pc.firePoint = fp.transform;

        GameObject de = new GameObject("DeathEffect");
        de.AddComponent<SpriteRenderer>().sprite = CreateSolidSprite(12, 12, Color.red);
        de.SetActive(false);
        pc.deathEffect = de;
    }

    static void SetupSpawnPoints()
    {
        Vector2[] positions = {
            new Vector2(-6, 4), new Vector2(6, 4),
            new Vector2(-6, -4), new Vector2(6, -4),
            new Vector2(0, 5), new Vector2(0, -5)
        };

        foreach (Vector2 pos in positions)
        {
            GameObject sp = new GameObject("SpawnPoint");
            sp.transform.position = pos;
            sp.tag = "SpawnPoint";
        }
    }

    static void SetupDoors()
    {
        GameObject door = new GameObject("Door");
        door.tag = "Door";
        door.transform.position = new Vector2(0, -6.8f);
        door.transform.localScale = new Vector3(3f, 0.4f, 1f);
        SpriteRenderer sr = door.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(2, 2, Color.green);
        sr.color = Color.green;
        sr.sortingOrder = 5;
    }

    static void SetupUI()
    {
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.tag = "UI";

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        UIManager ui = canvasGO.AddComponent<UIManager>();
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Health bar
        GameObject hbGO = new GameObject("HealthBar", typeof(Image));
        hbGO.transform.SetParent(canvasGO.transform);
        RectTransform hbRT = hbGO.GetComponent<RectTransform>();
        hbRT.anchorMin = new Vector2(0.02f, 0.88f);
        hbRT.anchorMax = new Vector2(0.22f, 0.93f);
        hbRT.offsetMin = hbRT.offsetMax = Vector2.zero;
        Image hbBG = hbGO.GetComponent<Image>();
        hbBG.color = new Color(0.1f, 0.1f, 0.1f);

        GameObject hbFill = new GameObject("Fill", typeof(Image));
        hbFill.transform.SetParent(hbGO.transform);
        Image fillImg = hbFill.GetComponent<Image>();
        fillImg.color = Color.red;
        RectTransform fillRT = hbFill.GetComponent<RectTransform>();
        fillRT.anchorMin = fillRT.anchorMax = new Vector2(0, 0.5f);
        fillRT.sizeDelta = new Vector2(0, 0);

        Slider slider = hbGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        slider.interactable = false;
        slider.fillRect = fillRT;
        slider.targetGraphic = hbBG;
        ui.healthSlider = slider;

        // Ammo
        Text ammoTxt = MakeText(canvasGO, "AmmoText", font, "12 / 60", 22, Color.white,
            new Vector2(0.02f, 0.83f), new Vector2(0.15f, 0.87f));
        ammoTxt.alignment = TextAnchor.MiddleLeft;
        ui.ammoText = ammoTxt;

        // Wave
        Text waveTxt = MakeText(canvasGO, "WaveText", font, "WAVE 1/3", 28, Color.white,
            new Vector2(0.5f, 0.93f), new Vector2(0.5f, 0.98f));
        waveTxt.alignment = TextAnchor.MiddleCenter;
        ui.waveText = waveTxt;

        // Score
        Text scoreTxt = MakeText(canvasGO, "ScoreText", font, "SCORE: 0", 22, Color.white,
            new Vector2(0.8f, 0.93f), new Vector2(0.98f, 0.98f));
        scoreTxt.alignment = TextAnchor.MiddleRight;
        ui.scoreText = scoreTxt;

        // Message (center screen)
        Text msgTxt = MakeText(canvasGO, "MessageText", font, "", 42, new Color(1f, 0.3f, 0.1f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        msgTxt.alignment = TextAnchor.MiddleCenter;
        msgTxt.gameObject.SetActive(false);
        ui.messageText = msgTxt;

        // Game Over / Win overlay panel
        GameObject overlay = new GameObject("Overlay", typeof(Image));
        overlay.transform.SetParent(canvasGO.transform);
        RectTransform oRT = overlay.GetComponent<RectTransform>();
        oRT.anchorMin = Vector2.zero;
        oRT.anchorMax = Vector2.one;
        oRT.offsetMin = oRT.offsetMax = Vector2.zero;
        Image overlayImg = overlay.GetComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0);
        overlayImg.raycastTarget = false;

        // Restart / Quit buttons (always there but hidden behind overlay)
        Text finalScore = MakeText(canvasGO, "FinalScoreText", font, "", 36, Color.white,
            new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.5f));
        finalScore.alignment = TextAnchor.MiddleCenter;
        finalScore.gameObject.SetActive(false);

        MakeButton(canvasGO, "RestartButton", "RESTART", new Vector2(0.5f, 0.35f), font, () => {
            if (GameManager.Instance) GameManager.Instance.RestartGame();
        }).gameObject.SetActive(false);

        MakeButton(canvasGO, "QuitButton", "QUIT", new Vector2(0.5f, 0.25f), font, () => Application.Quit())
            .gameObject.SetActive(false);

        ui.finalScoreText = finalScore;
    }

    static Text MakeText(GameObject parent, string name, Font font, string text, int size, Color color,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        Text txt = go.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = size;
        txt.color = color;
        txt.text = text;
        return txt;
    }

    static Button MakeButton(GameObject parent, string name, string label, Vector2 anchorPos, Font font,
        UnityEngine.Events.UnityAction action)
    {
        GameObject btnGO = new GameObject(name, typeof(RectTransform), typeof(Image));
        btnGO.transform.SetParent(parent.transform);
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchorPos;
        rt.sizeDelta = new Vector2(220, 50);

        Image btnImg = btnGO.GetComponent<Image>();
        btnImg.color = new Color(0.3f, 0.3f, 0.3f);

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(action);

        GameObject txtGO = new GameObject("Text", typeof(RectTransform));
        txtGO.transform.SetParent(btnGO.transform);
        Text txt = txtGO.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = 26;
        txt.text = label;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform trt = txtGO.GetComponent<RectTransform>();
        trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
        trt.sizeDelta = new Vector2(220, 50);
        return btn;
    }

    static void SetupManagers()
    {
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        GameObject am = new GameObject("AudioManager");
        AudioManager mgr = am.AddComponent<AudioManager>();
        AudioSource sfx = am.AddComponent<AudioSource>();
        AudioSource amb = am.AddComponent<AudioSource>();
        mgr.sfxSource = sfx;
        mgr.ambienceSource = amb;
    }

    static void SetupPrefabs()
    {
        // Bullet
        GameObject bullet = new GameObject("BulletPrefab");
        bullet.SetActive(false);
        bullet.tag = "Bullet";
        SpriteRenderer bsr = bullet.AddComponent<SpriteRenderer>();
        bsr.sprite = CreateSolidSprite(4, 4, Color.yellow);
        bsr.color = Color.yellow;
        bsr.sortingOrder = 15;
        CircleCollider2D bc = bullet.AddComponent<CircleCollider2D>();
        bc.radius = 0.5f;
        bc.isTrigger = true;
        bullet.AddComponent<Bullet>();
        Rigidbody2D br = bullet.AddComponent<Rigidbody2D>();
        br.gravityScale = 0f;
        br.bodyType = RigidbodyType2D.Kinematic;

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc != null) pc.bulletPrefab = bullet;

        // Spit
        GameObject spit = new GameObject("SpitPrefab");
        spit.SetActive(false);
        spit.tag = "EnemyProjectile";
        SpriteRenderer ssr = spit.AddComponent<SpriteRenderer>();
        ssr.sprite = CreateSolidSprite(5, 5, Color.green);
        ssr.color = Color.green;
        ssr.sortingOrder = 15;
        CircleCollider2D sc = spit.AddComponent<CircleCollider2D>();
        sc.radius = 0.6f;
        sc.isTrigger = true;
        spit.AddComponent<Bullet>();
        Rigidbody2D srb = spit.AddComponent<Rigidbody2D>();
        srb.gravityScale = 0f;
        srb.bodyType = RigidbodyType2D.Kinematic;

        // Health pack
        GameObject hp = new GameObject("HealthPackPrefab");
        hp.SetActive(false);
        hp.tag = "Pickup";
        SpriteRenderer hpSr = hp.AddComponent<SpriteRenderer>();
        hpSr.sprite = CreateSolidSprite(6, 6, Color.green);
        hpSr.color = Color.green;
        hpSr.sortingOrder = 5;
        CircleCollider2D hpCol = hp.AddComponent<CircleCollider2D>();
        hpCol.radius = 0.5f;
        hpCol.isTrigger = true;
        Pickup hpPick = hp.AddComponent<Pickup>();
        hpPick.type = Pickup.PickupType.Health;

        // Ammo pack
        GameObject ap = new GameObject("AmmoPackPrefab");
        ap.SetActive(false);
        ap.tag = "Pickup";
        SpriteRenderer apSr = ap.AddComponent<SpriteRenderer>();
        apSr.sprite = CreateSolidSprite(6, 6, Color.yellow);
        apSr.color = Color.yellow;
        apSr.sortingOrder = 5;
        CircleCollider2D apCol = ap.AddComponent<CircleCollider2D>();
        apCol.radius = 0.5f;
        apCol.isTrigger = true;
        Pickup apPick = ap.AddComponent<Pickup>();
        apPick.type = Pickup.PickupType.Ammo;

        GameManager gm = Object.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.healthPackPrefab = hp;
            gm.ammoPackPrefab = ap;

            gm.runnerPrefab = MakeEnemyPrefab("RunnerPrefab", EnemyController.EnemyType.Runner,
                new Color(0.9f, 0.2f, 0.1f), 0.5f, 30f, 4f, 10f, 1.5f, 1f, 100f);
            gm.brutePrefab = MakeEnemyPrefab("BrutePrefab", EnemyController.EnemyType.Brute,
                new Color(0.6f, 0.1f, 0.1f), 0.8f, 80f, 1.8f, 20f, 1.5f, 2f, 200f);
            gm.spitterPrefab = MakeEnemyPrefab("SpitterPrefab", EnemyController.EnemyType.Spitter,
                new Color(0.3f, 0.8f, 0.1f), 0.5f, 40f, 2.5f, 12f, 5f, 1.5f, 150f);

            EnemyController spitter = gm.spitterPrefab.GetComponent<EnemyController>();
            spitter.spitPrefab = spit;
            spitter.spitSpeed = 8f;
        }
    }

    static GameObject MakeEnemyPrefab(string name, EnemyController.EnemyType type, Color color,
        float scale, float health, float speed, float dmg, float range, float cooldown, float score)
    {
        GameObject enemy = new GameObject(name);
        enemy.SetActive(false);
        enemy.tag = "Enemy";

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSolidSprite(8, 8, color);
        sr.color = color;
        sr.sortingOrder = 5;

        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearDamping = 5f;

        CircleCollider2D col = enemy.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

        EnemyController ec = enemy.AddComponent<EnemyController>();
        ec.enemyType = type;
        ec.health = health;
        ec.moveSpeed = speed;
        ec.damage = dmg;
        ec.attackRange = range;
        ec.attackCooldown = cooldown;
        ec.scoreValue = score;
        ec.detectionRange = 12f;

        enemy.transform.localScale = Vector3.one * scale;
        return enemy;
    }

    static Sprite CreateSolidSprite(int w, int h, Color color)
    {
        Texture2D tex = new Texture2D(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
    }
}
