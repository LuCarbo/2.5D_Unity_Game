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
    private Collider bossCollider; // CORRETTO: Tolto il 2D!
    private BossAudioManager audioManager;
    private BossCombat bossCombat;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        bossController = GetComponent<BossController>();
        bossCollider = GetComponent<Collider>(); // CORRETTO: Tolto il 2D!
        audioManager = GetComponentInChildren<BossAudioManager>();
        bossCombat = GetComponent<BossCombat>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Il Boss subisce {damageAmount} danni! Salute: {currentHealth}/{maxHealth}");

        if (audioManager != null) audioManager.PlayHurtSound();

        // 1. Fai partire l'animazione di danno
        if (anim != null) anim.SetTrigger("TakeDamage");

        // 2. IL SALVAVITA: Annulliamo eventuali attacchi in corso per non farlo bloccare!
        if (bossCombat != null)
        {
            bossCombat.CancelAttack();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Il demone e' stato sconfitto!");

        if (audioManager != null)
        {
            audioManager.PlayDeathSound();
            audioManager.enabled = false;
        }

        if (anim != null) anim.SetBool("isDead", true);
        if (bossController != null) bossController.enabled = false;
        if (bossCollider != null) bossCollider.enabled = false;

        // CORRETTO: Fisica 3D per fermare il cadavere
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Usiamo Vector3 per il 3D
            rb.isKinematic = true;
        }

        Destroy(gameObject, 2.2f);
    }
}