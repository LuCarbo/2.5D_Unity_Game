using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioManager : MonoBehaviour
{
    [Header("Tracce Audio Lupo")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Suoni Ambientali / Idle")]
    public AudioClip[] idleSounds; // Puoi metterne quanti ne vuoi!
    public float minTimeBetweenIdle = 4f;
    public float maxTimeBetweenIdle = 10f;

    private AudioSource audioSource;
    private float nextIdleTime;
    private bool isDead = false; // Serve per farlo zittire quando muore

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // Audio 3D per capire da dove viene il lupo

        // Calcola subito quando fare il primo verso
        ScheduleNextIdle();
    }

    void Update()
    {
        // Se non è morto, se ci sono suoni nella lista, e se il timer è scaduto... suona!
        if (!isDead && idleSounds.Length > 0 && Time.time >= nextIdleTime)
        {
            PlayIdleSound();
            ScheduleNextIdle(); // Ricalcola il timer per la prossima volta
        }
    }

    private void ScheduleNextIdle()
    {
        // Sceglie un tempo a caso tra il minimo e il massimo
        nextIdleTime = Time.time + Random.Range(minTimeBetweenIdle, maxTimeBetweenIdle);
    }

    private void PlayIdleSound()
    {
        int randomIndex = Random.Range(0, idleSounds.Length);
        audioSource.PlayOneShot(idleSounds[randomIndex]);
    }

    // --- METODI GIA' ESISTENTI (Non toccarli) ---

    public void PlayAttackSound()
    {
        Debug.Log("L'animazione ha lanciato il comando del suono di attacco!");
        if (attackSound != null) audioSource.PlayOneShot(attackSound);
    }

    public void PlayHurtSound()
    {
        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);
    }

    public void PlayDeathSound()
    {
        isDead = true; // Zittisce i suoni idle per sempre
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
    }
}