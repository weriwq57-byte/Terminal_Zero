using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 mousePos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
{
    // Считываем клавиатуру
    moveInput.x = Input.GetAxisRaw("Horizontal");
    moveInput.y = Input.GetAxisRaw("Vertical");

    // Получаем позицию мыши в мировых координатах (ПРОВЕРЬ ЭТО!)
    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
}

    void FixedUpdate()
    {
        // 3. Двигаем персонажа через физику
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        // 4. Поворачиваем персонажа лицом к курсору
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}