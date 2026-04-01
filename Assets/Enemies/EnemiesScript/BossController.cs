using UnityEngine;

public class BossController : MonoBehaviour {
    public enum BossState { Idle, Chasing, Attacking }

    [Header("Stato Attuale")]
    public BossState currentState = BossState.Idle;

    [Header("Impostazioni di Rilevamento")]
    public Transform playerTransform; // Trascina qui il Player
    public float activationDistance = 8f; // Distanza di risveglio
    public float attackDistance = 2f;     // Distanza per colpire

    // Riferimenti agli altri script
    private BossMovement movementScript;
    private BossCombat combatScript;

    void Awake() {
        // Colleghiamo gli script se sono presenti sullo stesso oggetto
        movementScript = GetComponent<BossMovement>();
        combatScript = GetComponent<BossCombat>();
    }

    void Update() {
        if (playerTransform == null) return;

        // Calcoliamo la distanza una sola volta per frame
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState) {
            case BossState.Idle:
                // Se il giocatore entra nell'area, il boss si sveglia e inizia a inseguire
                if (distanceToPlayer <= activationDistance) {
                    currentState = BossState.Chasing;
                    Debug.Log("Giocatore rilevato! Inizio inseguimento.");
                }
                break;

            case BossState.Chasing:
                // Ordina di muoversi (se lo script esiste)
                if (movementScript != null) movementScript.MoveTowardsPlayer(playerTransform);

                // Se è abbastanza vicino, si ferma e attacca
                if (distanceToPlayer <= attackDistance) {
                    currentState = BossState.Attacking;
                }
                break;

            case BossState.Attacking:
                // Ordina di attaccare
                if (movementScript != null) movementScript.StopMoving();
                if (combatScript != null) combatScript.PerformAttack();

                // Se il giocatore si allontana, torna a inseguirlo
                if (distanceToPlayer > attackDistance) {
                    currentState = BossState.Chasing;
                }
                break;
        }
    }

    // Disegna due cerchi colorati nell'editor per aiutarti a visualizzare le distanze
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance); // Area di risveglio

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance); // Area di attacco
    }
}
