using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioManager : MonoBehaviour
{
    [Header("Tracce Audio Lupo")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Assicuriamoci che l'audio 3D sia impostato bene (opzionale ma consigliato)
        audioSource.spatialBlend = 1f;
    }

    // Da chiamare con un Animation Event!
    public void PlayAttackSound()
    {
        if (attackSound != null) audioSource.PlayOneShot(attackSound);
    }

    // Questi li chiamerà lo script Health del Padre
    public void PlayHurtSound()
    {
        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);
    }

    public void PlayDeathSound()
    {
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
    }
}