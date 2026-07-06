using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 10f; 
    private float currentHealth;

    // Время, когда можно получить следующий урон
    private float nextDamageTime = 0f; 
    // Минимальный перерыв между получением урона 
    private float damageCooldown = 0.05f; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Если с момента прошлого урона прошло мало времени 
        if (Time.time < nextDamageTime) return;

        // Обновляем время следующего возможного удара
        nextDamageTime = Time.time + damageCooldown;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " получил урон. Осталось ХП: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}