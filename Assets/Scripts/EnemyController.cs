using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Runner, Brute, Spitter }
    public EnemyType enemyType = EnemyType.Runner;

    [Header("Stats")]
    public float health = 30f;
    public float moveSpeed = 3f;
    public float damage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 10f;

    [Header("Ranged (Spitter)")]
    public GameObject spitPrefab;
    public float spitSpeed = 8f;

    public float scoreValue = 100f;

    private Transform player;
    private Rigidbody2D rb;
    private float attackTimer;
    private bool dead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (dead || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            Chase();
            if (dist <= attackRange)
                TryAttack();
        }
    }

    void Chase()
    {
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void TryAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;
        attackTimer = attackCooldown;

        if (enemyType == EnemyType.Spitter)
            SpitAttack();
        else
            MeleeAttack();
    }

    void MeleeAttack()
    {
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(damage);
        }
    }

    void SpitAttack()
    {
        if (spitPrefab == null || player == null) return;
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        GameObject spit = Instantiate(spitPrefab, transform.position, Quaternion.identity);
        spit.SetActive(true);
        Bullet b = spit.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = damage;
            b.speed = spitSpeed;
            b.direction = dir;
            b.gameObject.tag = "EnemyProjectile";
        }
    }

    public void TakeDamage(float dmg)
    {
        if (dead) return;
        health -= dmg;

        if (AudioManager.Instance)
            AudioManager.Instance.PlayHit();

        if (health <= 0f) Die();
    }

    void Die()
    {
        dead = true;
        rb.linearVelocity = Vector2.zero;
        if (GameManager.Instance)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.EnemyDied();
        }

        GameObject effect = new GameObject("DeathParticle");
        effect.transform.position = transform.position;
        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(4, Color.white);
        sr.color = Color.red;
        Destroy(effect, 0.3f);

        Destroy(gameObject);
    }

    Sprite CreateCircleSprite(int radius, Color color)
    {
        Texture2D tex = new Texture2D(radius * 2, radius * 2);
        for (int y = 0; y < tex.height; y++)
            for (int x = 0; x < tex.width; x++)
            {
                float dx = x - radius, dy = y - radius;
                tex.SetPixel(x, y, (dx * dx + dy * dy <= radius * radius) ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    }

    public bool IsDead() => dead;
}
