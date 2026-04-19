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
                // 1. Se sei lontano, ti rincorre normalmente
                if (distanceToPlayer > attackDistance)
                {
                    if (anim != null) anim.SetBool("isChasing", true);
                    if (movementScript != null) movementScript.MoveTowardsPlayer(playerTransform);
                }
                // 2. Se sei a tiro...
                else
                {
                    // Controlla se la spada è pronta (non in cooldown)
                    if (combatScript != null && !combatScript.isOnCooldown)
                    {
                        currentState = BossState.Attacking;
                    }
                    else
                    {
                        // Se è in cooldown, si ferma davanti a te minaccioso in Idle e aspetta!
                        if (anim != null) anim.SetBool("isChasing", false);
                        if (movementScript != null) movementScript.StopMoving();
                    }
                }
                break;

            case BossState.Attacking:
                // Mentre attacca, assicuriamoci che non cammini
                if (anim != null) anim.SetBool("isChasing", false);

                if (movementScript != null) movementScript.LockMovement();
                if (combatScript != null) combatScript.PerformAttack();

                // Finché l'animazione sta andando, non fa nient'altro
                if (combatScript != null && combatScript.isAttacking)
                {
                    break;
                }

                // --- L'ATTACCO È FINITO ---
                if (movementScript != null) movementScript.UnlockMovement();

                // IL FIX DEFINITIVO: Torniamo SEMPRE a Chasing. 
                // Sarà lui a decidere al prossimo frame se riattaccarti o camminare!
                currentState = BossState.Chasing;
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