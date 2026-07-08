using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Waves")]
    public int totalWaves = 3;
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 2f;
    public float spawnRadius = 6f;

    [Header("Enemy Prefabs")]
    public GameObject runnerPrefab;
    public GameObject brutePrefab;
    public GameObject spitterPrefab;

    [Header("Difficulty Scaling")]
    public float enemiesPerWaveScale = 1.3f;
    public float bruteChance = 0.2f;
    public float spitterChance = 0.1f;

    [Header("Drops")]
    public GameObject healthPackPrefab;
    public GameObject ammoPackPrefab;

    private int currentWave;
    private int aliveEnemies;
    private float score;
    private Transform player;
    private PlayerController playerController;
    private List<Transform> spawnPoints = new List<Transform>();
    private bool gameActive = true;
    private bool playerDied;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            spawnPoints.Add(obj.transform);

        StartCoroutine(BeginGame());
    }

    IEnumerator BeginGame()
    {
        if (UIManager.Instance)
            UIManager.Instance.ShowMessage("GET READY");

        yield return new WaitForSeconds(2f);

        if (UIManager.Instance)
            UIManager.Instance.ShowMessage("SECTOR LOCKED");

        LockDoors(true);

        yield return new WaitForSeconds(1f);

        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (currentWave < totalWaves && gameActive)
        {
            if (UIManager.Instance)
                UIManager.Instance.UpdateWave(currentWave + 1, totalWaves);

            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnWave(currentWave);
            currentWave++;
            yield return new WaitUntil(() => aliveEnemies == 0 && gameActive);
        }

        if (gameActive && !playerDied)
            RoomCleared();
    }

    void SpawnWave(int waveIndex)
    {
        int count = Mathf.RoundToInt(enemiesPerWave * Mathf.Pow(enemiesPerWaveScale, waveIndex));
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetSpawnPosition();
            GameObject prefab = ChooseEnemyPrefab();
            if (prefab != null)
            {
                GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
                enemy.SetActive(true);
                aliveEnemies++;
            }
        }
    }

    Vector2 GetSpawnPosition()
    {
        if (spawnPoints.Count > 0)
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        if (player == null) return Vector2.zero;
        return (Vector2)player.position + Random.insideUnitCircle * spawnRadius;
    }

    GameObject ChooseEnemyPrefab()
    {
        float roll = Random.value;
        if (roll < bruteChance) return brutePrefab;
        if (roll < bruteChance + spitterChance) return spitterPrefab;
        return runnerPrefab;
    }

    public void EnemyDied()
    {
        aliveEnemies--;
        if (aliveEnemies < 0) aliveEnemies = 0;

        if (Random.value < 0.25f && player != null)
            DropLoot((Vector2)player.position + Random.insideUnitCircle * 2f);
    }

    void DropLoot(Vector2 pos)
    {
        GameObject prefab = Random.value < 0.5f ? healthPackPrefab : ammoPackPrefab;
        if (prefab != null)
        {
            GameObject drop = Instantiate(prefab, pos, Quaternion.identity);
            drop.SetActive(true);
        }
    }

    void RoomCleared()
    {
        LockDoors(false);

        DropLoot(player.position + Vector3.right * 2f);

        if (UIManager.Instance)
            UIManager.Instance.ShowMessage("SECTOR CLEARED");

        if (AudioManager.Instance)
            AudioManager.Instance.PlayDoorOpen();

        if (!playerDied)
            WinGame();
    }

    void LockDoors(bool locked)
    {
        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            SpriteRenderer sr = door.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = locked ? Color.red : Color.green;
        }
    }

    public void AddScore(float points)
    {
        score += points;
        if (UIManager.Instance)
            UIManager.Instance.UpdateScore(Mathf.RoundToInt(score));
    }

    public void GameOver()
    {
        playerDied = true;
        gameActive = false;
        if (UIManager.Instance)
            UIManager.Instance.ShowGameOver(score);
    }

    void WinGame()
    {
        if (UIManager.Instance)
            UIManager.Instance.ShowWin(score);
    }

    public void RestartGame()
    {
        StopAllCoroutines();

        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(e);
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Pickup"))
            Destroy(p);
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Bullet"))
            Destroy(b);
        foreach (GameObject ep in GameObject.FindGameObjectsWithTag("EnemyProjectile"))
            Destroy(ep);

        currentWave = 0;
        aliveEnemies = 0;
        score = 0;
        gameActive = true;
        playerDied = false;

        if (player != null)
        {
            player.position = Vector3.zero;
            player.gameObject.SetActive(true);
            playerController.FullReset();
        }

        if (UIManager.Instance)
            UIManager.Instance.ResetHUD();

        LockDoors(false);
        StartCoroutine(BeginGame());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance)
                UIManager.Instance.TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.F11) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return)))
            Screen.fullScreen = !Screen.fullScreen;
    }
}
