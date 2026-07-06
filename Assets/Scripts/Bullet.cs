using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;
    public Vector2 direction;
    public float lifetime = 2f;

    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > lifetime) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("EnemyProjectile"))
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController pc = collision.GetComponent<PlayerController>();
                if (pc != null) pc.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                EnemyController enemy = collision.GetComponent<EnemyController>();
                if (enemy != null) enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        if (collision.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
