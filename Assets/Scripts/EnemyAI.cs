using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3.5f; 
    private Transform player;   
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            
            Vector2 direction = (player.position - transform.position).normalized;
            
            rb.linearVelocity = direction * speed;
        }
    }
}