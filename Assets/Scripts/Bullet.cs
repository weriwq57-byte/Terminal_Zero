using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Работаем через обычное жесткое столкновение
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Уничтожаем пулю без шансов на отскок
        Destroy(gameObject);
    }
}