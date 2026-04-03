using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salute del Demone")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Riferimenti")]
    [Tooltip("Trascina qui l'oggetto con l'Animator del Boss")]
    public Animator anim;

    // Ci salviamo i riferimenti agli altri componenti per poterli "spegnere" quando muore
    private BossController bossController;
    private Collider2D bossCollider;

    private bool isDead = false;

    void Start()
    {
        // All'inizio riempiamo la vita al massimo
        currentHealth = maxHealth;

        // Otteniamo in automatico i componenti attaccati allo stesso oggetto
        bossController = GetComponent<BossController>();
        bossCollider = GetComponent<Collider2D>();
    }

    // Questo � il metodo che la spada/proiettile del Player dovr� chiamare
    public void TakeDamage(int damageAmount)
    {
        // Se � gi� morto, ignoriamo il colpo
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Il Boss subisce {damageAmount} danni! Salute: {currentHealth}/{maxHealth}");

        // Se hai un'animazione per quando viene colpito, potresti chiamarla qui
        // if (anim != null) anim.SetTrigger("Hit");

        // Controlliamo se la vita � scesa a zero o meno
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Il demone � stato sconfitto!");

        // 1. Facciamo partire l'animazione di morte
        if (anim != null) anim.SetBool("isDead", true);

        // 2. SPEGNIAMO IL CERVELLO: disabilitando il BossController smetter�
        //    immediatamente di muoversi e di attaccare.
        if (bossController != null) bossController.enabled = false;

        // 3. Disabilitiamo il collider (cos� il player non ci sbatte contro mentre � a terra)
        if (bossCollider != null) bossCollider.enabled = false;

        // 4. Disabilitiamo la gravit�/fisica se non vogliamo che il cadavere scivoli via
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Lo fermiamo sul posto
            rb.isKinematic = true;      // Disabilitiamo la simulazione fisica
        }

        // OPZIONALE: Distrugge l'oggetto dopo 3 secondi (tempo per finire l'animazione)
        // Se vuoi che il cadavere rimanga a terra per sempre, lascia questa riga commentata!
        // Destroy(gameObject, 3f);
    }
}
