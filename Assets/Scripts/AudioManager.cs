using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource ambienceSource;

    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip hitSound;
    public AudioClip pickupSound;
    public AudioClip doorOpenSound;
    public AudioClip enemyDeathSound;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayShoot()
    {
        if (sfxSource && shootSound) sfxSource.PlayOneShot(shootSound);
    }

    public void PlayReload()
    {
        if (sfxSource && reloadSound) sfxSource.PlayOneShot(reloadSound);
    }

    public void PlayHit()
    {
        if (sfxSource && hitSound) sfxSource.PlayOneShot(hitSound);
    }

    public void PlayPickup()
    {
        if (sfxSource && pickupSound) sfxSource.PlayOneShot(pickupSound);
    }

    public void PlayDoorOpen()
    {
        if (sfxSource && doorOpenSound) sfxSource.PlayOneShot(doorOpenSound);
    }

    public void PlayEnemyDeath()
    {
        if (sfxSource && enemyDeathSound) sfxSource.PlayOneShot(enemyDeathSound);
    }
}
