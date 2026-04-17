using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossAudioManager : MonoBehaviour
{
    [Header("Tracce Audio")]
    public AudioClip[] footstepSounds;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip[] roarSounds;

    [Header("Impostazioni Urla (Roar)")]
    public float minTimeBetweenRoars = 5f;
    public float maxTimeBetweenRoars = 15f;
    private float nextRoarTime;

    private AudioSource audioSource;
    private bool canRoar = true; // Per fermare i ruggiti da morto

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ScheduleNextRoar();
    }

    void Update()
    {
        if (canRoar && Time.time >= nextRoarTime && roarSounds.Length > 0)
        {
            PlayRoar();
            ScheduleNextRoar();
        }
    }

    private void ScheduleNextRoar()
    {
        nextRoarTime = Time.time + Random.Range(minTimeBetweenRoars, maxTimeBetweenRoars);
    }

    public void PlayFootstepSound() { PlaySound(footstepSounds[Random.Range(0, footstepSounds.Length)]); }
    public void PlayAttackSound() { PlaySound(attackSound); }
    public void PlayHurtSound() { PlaySound(hurtSound); }

    public void PlayDeathSound()
    {
        canRoar = false; // Smette di ruggire
        if (deathSound != null)
        {
            Debug.Log("Riproduzione suono di morte!");
            audioSource.PlayOneShot(deathSound);
        }
    }

    public void PlayRoar()
    {
        if (roarSounds.Length > 0)
        {
            Debug.Log("Il Boss sta ruggendo...");
            PlaySound(roarSounds[Random.Range(0, roarSounds.Length)]);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    // Un piccolo trucco: se passi l'array è vuoto, PlaySound non farà nulla senza crashare
    private void PlaySound(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
            PlaySound(clips[Random.Range(0, clips.Length)]);
    }
}