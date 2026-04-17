using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salute del Demone")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Riferimenti")]
    [Tooltip("Trascina qui l'oggetto con l'Animator del Boss")]
    public Animator anim;

    private BossController bossController;
    private Collider2D bossCollider;
    private BossAudioManager audioManager; // NUOVO: Riferimento all'audio

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        bossController = GetComponent<BossController>();
        bossCollider = GetComponent<Collider2D>();

        // IL FIX È QUI SOTTO: Aggiungi "InChildren"
        audioManager = GetComponentInChildren<BossAudioManager>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Il Boss subisce {damageAmount} danni! Salute: {currentHealth}/{maxHealth}");

        // NUOVO: Suono del colpo subito
        if (audioManager != null) audioManager.PlayHurtSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Il demone e' stato sconfitto!");

        // NUOVO: Suono della morte. Spegniamo anche l'audio manager così smette di urlare
        if (audioManager != null)
        {
            audioManager.PlayDeathSound();
            audioManager.enabled = false;
        }

        if (anim != null) anim.SetBool("isDead", true);
        if (bossController != null) bossController.enabled = false;
        if (bossCollider != null) bossCollider.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject, 2.2f);
    }
}