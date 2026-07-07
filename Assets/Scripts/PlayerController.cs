using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public float maxHealth = 100f;
    public float health;

    [Header("Weapons")]
    public string[] weaponNames = { "Pistol", "SMG" };
    public float[] fireRates = { 0.3f, 0.1f };
    public int[] magSizes = { 12, 30 };
    public int[] maxAmmo = { 60, 120 };
    public float[] bulletDamages = { 10f, 8f };
    public float[] bulletSpeeds = { 20f, 25f };
    public float reloadTime = 1.5f;

    private int currentWeapon = 0;
    private int[] currentAmmo;
    private int[] reserveAmmo;
    private float fireTimer;
    private bool isReloading;
    private float reloadTimer;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 aimDir;
    private bool dead;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject deathEffect;

    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        currentAmmo = new int[weaponNames.Length];
        reserveAmmo = new int[weaponNames.Length];
        for (int i = 0; i < weaponNames.Length; i++)
        {
            currentAmmo[i] = magSizes[i];
            reserveAmmo[i] = maxAmmo[i];
        }
    }

    void Update()
    {
        if (dead) return;

        Aim();
        HandleShooting();
        HandleReload();
        HandleWeaponSwitch();

        if (UIManager.Instance)
            UIManager.Instance.UpdateHealth(health / maxHealth);
    }

    void FixedUpdate()
    {
        if (dead) return;
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.linearVelocity = move.normalized * moveSpeed;
    }

    void Aim()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        aimDir = (mousePos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (isReloading) return;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            if (currentAmmo[currentWeapon] > 0)
            {
                Shoot();
            }
            else
            {
                StartReload();
            }
        }
    }

    void Shoot()
    {
        fireTimer = fireRates[currentWeapon];
        currentAmmo[currentWeapon]--;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.SetActive(true);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = bulletDamages[currentWeapon];
            b.speed = bulletSpeeds[currentWeapon];
            b.direction = aimDir;
        }

        if (AudioManager.Instance)
            AudioManager.Instance.PlayShoot();

        if (UIManager.Instance)
            UIManager.Instance.UpdateAmmo(currentAmmo[currentWeapon], reserveAmmo[currentWeapon]);
    }

    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            if (currentAmmo[currentWeapon] < magSizes[currentWeapon] && reserveAmmo[currentWeapon] > 0)
                StartReload();
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
                FinishReload();
        }
    }

    void StartReload()
    {
        isReloading = true;
        reloadTimer = reloadTime;
        if (AudioManager.Instance)
            AudioManager.Instance.PlayReload();
    }

    void FinishReload()
    {
        isReloading = false;
        int need = magSizes[currentWeapon] - currentAmmo[currentWeapon];
        int give = Mathf.Min(need, reserveAmmo[currentWeapon]);
        currentAmmo[currentWeapon] += give;
        reserveAmmo[currentWeapon] -= give;

        if (UIManager.Instance)
            UIManager.Instance.UpdateAmmo(currentAmmo[currentWeapon], reserveAmmo[currentWeapon]);
    }

    void HandleWeaponSwitch()
    {
        if (isReloading) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = 1;

        if (UIManager.Instance)
            UIManager.Instance.UpdateAmmo(currentAmmo[currentWeapon], reserveAmmo[currentWeapon]);
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;
        health -= damage;
        if (AudioManager.Instance)
            AudioManager.Instance.PlayHit();

        if (UIManager.Instance)
            UIManager.Instance.UpdateHealth(health / maxHealth);

        if (health <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        if (UIManager.Instance)
            UIManager.Instance.UpdateHealth(health / maxHealth);
    }

    public void AddAmmo(int pistolAmount, int smgAmount)
    {
        reserveAmmo[0] = Mathf.Min(maxAmmo[0], reserveAmmo[0] + pistolAmount);
        reserveAmmo[1] = Mathf.Min(maxAmmo[1], reserveAmmo[1] + smgAmount);
        if (UIManager.Instance)
            UIManager.Instance.UpdateAmmo(currentAmmo[currentWeapon], reserveAmmo[currentWeapon]);
    }

    public void FullReset()
    {
        health = maxHealth;
        dead = false;
        isReloading = false;
        reloadTimer = 0;
        fireTimer = 0;
        currentWeapon = 0;
        for (int i = 0; i < weaponNames.Length; i++)
        {
            currentAmmo[i] = magSizes[i];
            reserveAmmo[i] = maxAmmo[i];
        }
        if (UIManager.Instance)
        {
            UIManager.Instance.UpdateHealth(1f);
            UIManager.Instance.UpdateAmmo(currentAmmo[0], reserveAmmo[0]);
        }
    }

    void Die()
    {
        dead = true;
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        if (GameManager.Instance)
            GameManager.Instance.GameOver();
    }

    public bool IsDead() => dead;
}
