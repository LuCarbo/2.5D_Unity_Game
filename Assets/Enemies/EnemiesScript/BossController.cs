using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Chasing, Attacking }

    [Header("Stato Attuale")]
    public BossState currentState = BossState.Idle;

    [Header("Impostazioni di Rilevamento")]
    public Transform playerTransform; // Trascina qui il Player
    public float activationDistance = 8f; // Distanza di risveglio
    public float attackDistance = 2f;     // Distanza per colpire

    [Header("Animazione")]
    [Tooltip("Trascina qui l'oggetto del Boss che contiene l'Animator")]
    public Animator anim;

    // Riferimenti agli altri script
    private BossMovement movementScript;
    private BossCombat combatScript;

    void Awake()
    {
        // Colleghiamo gli script se sono presenti sullo stesso oggetto
        movementScript = GetComponent<BossMovement>();
        combatScript = GetComponent<BossCombat>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case BossState.Idle:
                if (anim != null) anim.SetBool("isChasing", false);

                if (distanceToPlayer <= activationDistance)
                {
                    currentState = BossState.Chasing;
                }
                break;

            case BossState.Chasing:
                if (anim != null) anim.SetBool("isChasing", true);

                if (movementScript != null) movementScript.MoveTowardsPlayer(playerTransform);

                if (distanceToPlayer <= attackDistance)
                {
                    currentState = BossState.Attacking;
                }
                break;

            case BossState.Attacking:
                if (anim != null) anim.SetBool("isChasing", false);

                // Mettiamo il lucchetto fisico alle gambe del boss!
                if (movementScript != null) movementScript.LockMovement();

                // Ordiniamo di attaccare
                if (combatScript != null) combatScript.PerformAttack();

                // Se sta ancora attaccando, ci fermiamo qui
                if (combatScript != null && combatScript.isAttacking)
                {
                    break;
                }

                // --- SE ARRIVA QUI SOTTO, L'ATTACCO È FINITO ---

                // Togliamo il lucchetto fisico!
                if (movementScript != null) movementScript.UnlockMovement();

                // Ricontrolliamo la distanza
                if (distanceToPlayer > attackDistance)
                {
                    currentState = BossState.Chasing;
                }
                break;
        }
    }

    // Disegna due cerchi colorati nell'editor per aiutarti a visualizzare le distanze
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance); // Area di risveglio

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance); // Area di attacco
    }
}