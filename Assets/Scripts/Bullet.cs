using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 5f; // Урон от одной пули
    private bool hasHit = false; 

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();

        if (health != null)
        {
            hasHit = true; 
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}