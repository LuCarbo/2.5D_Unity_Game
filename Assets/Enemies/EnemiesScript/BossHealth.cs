using System.Collections;
using UnityEngine;
using UnityEngine.Playables; 

public class BossHealth : MonoBehaviour
{
    [Header("Salute del Demone")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Riferimenti")]
    public PlayableDirector endingDirector;
    [Tooltip("Trascina qui l'oggetto con l'Animator del Boss")]
    public Animator anim;

    private BossController bossController;
    private Collider2D bossCollider;
    private BossAudioManager audioManager;

    private BossCombat bossCombat;
    private Coroutine stunCoroutine;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        bossController = GetComponent<BossController>();
        bossCollider = GetComponent<Collider2D>();
        audioManager = GetComponentInChildren<BossAudioManager>();

        
        bossCombat = GetComponent<BossCombat>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Il Boss subisce {damageAmount} danni! Salute: {currentHealth}/{maxHealth}");

        if (audioManager != null) audioManager.PlayHurtSound();

        // Interrompiamo qualsiasi attacco in corso
        if (bossCombat != null) bossCombat.InterruptAttack();

        // Facciamo partire l'animazione
        if (anim != null) anim.SetTrigger("TakeDamage");

        if (currentHealth <= 0)
        {
            Die();
            return; // Se muore, fermiamo tutto qui
        }

        // NUOVO: Stordiamo il boss se è ancora vivo!
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        // 1. Spegniamo il "cervello" del boss per non fargli prendere decisioni
        if (bossController != null) bossController.enabled = false;

        // 2. Fermiamo i motori e sblocchiamo le gambe
        BossMovement bm = GetComponent<BossMovement>();
        if (bm != null)
        {
            bm.StopMoving();
            bm.UnlockMovement();
        }

        // 3. Aspettiamo che finisca di soffrire (0.5 secondi di solito è perfetto, ma puoi alzarlo)
        yield return new WaitForSeconds(0.5f);

        // 4. Riaccendiamo il cervello e resettiamo il suo stato!
        if (bossController != null)
        {
            bossController.currentState = BossController.BossState.Idle; // Torna neutro
            bossController.enabled = true; // Si rimette in moto
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Il demone e' stato sconfitto!");

        // --- NUOVO (SICURO AL 100%): Ferma la musica solo se il Gestore esiste ---
        if (GestoreMusica.istanza != null)
        {
            GestoreMusica.istanza.FermaMusica();
        }
        // -----------------------------------------------------------------------

        // FAI PARTIRE LA CINEMATICA QUI
        if (endingDirector != null)
        {
            endingDirector.Play();
        }

        if (audioManager != null)
        {
            audioManager.PlayDeathSound();
            audioManager.enabled = false;
        }

        if (anim != null) anim.SetBool("isDead", true);
        if (bossController != null) bossController.enabled = false;
        if (bossCollider != null) bossCollider.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject, 5f);
    }
}