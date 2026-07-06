using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, Ammo }
    public PickupType type = PickupType.Health;
    public float healAmount = 30f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController pc = collision.GetComponent<PlayerController>();
        if (pc == null) return;

        if (type == PickupType.Health)
        {
            pc.Heal(healAmount);
        }
        else
        {
            pc.AddAmmo(15, 30);
        }

        if (AudioManager.Instance)
            AudioManager.Instance.PlayPickup();

        Destroy(gameObject);
    }
}
