using System.Collections;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    [Header("Impostazioni Spada")]
    [Tooltip("L'oggetto vuoto posizionato sulla spada/mano del boss")]
    public Transform attackPoint;
    public float attackRadius = 1.5f;   // Quanto è grande l'area del fendente
    public int attackDamage = 4;        // Quanti danni fa
    public float attackCooldown = 2f;   // Pausa tra una spadata e l'altra

    [Tooltip("Tempo prima che il colpo faccia danno (per sincronizzarlo con l'animazione)")]
    public float attackDelay = 0.5f;

    [Header("Maschere di Collisione")]
    public LayerMask playerLayer;       // Il layer assegnato al tuo Player

    // Variabili di stato
    public bool isAttacking = false;
    private bool isOnCooldown = false;

    public Animator anim;              // Oggetto per l'animazione

    // Metodo chiamato dal BossController
    public void PerformAttack()
    {
        // Se non sta già attaccando e non è in cooldown, attacca
        if (!isAttacking && !isOnCooldown)
        {
            StartCoroutine(SwordAttackRoutine());
        }
    }

    private IEnumerator SwordAttackRoutine()
    {
        isAttacking = true;

        // 1. Facciamo partire l'animazione!
        if (anim != null) anim.SetTrigger("Attack");

        // 2. Aspettiamo l'attackDelay
        yield return new WaitForSeconds(attackDelay);

        // 3. IL COLPO 3D: Creiamo una Sfera invisibile usando il motore fisico 3D!
        // Restituisce un array (una lista) di tutti i Collider 3D che si trovano dentro la sfera
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

        // 4. Se la lista contiene almeno un elemento (Length > 0), significa che abbiamo colpito il Player
        if (hitPlayers.Length > 0)
        {
            // Prendiamo il primo oggetto della lista (il nostro Player)
            Collider hitPlayer = hitPlayers[0];

            // Cerchiamo lo script Health su questo Collider 3D
            Health playerHealth = hitPlayer.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-(int)attackDamage);
                Debug.Log($"Spadata a segno! Inflitti {(int)attackDamage} danni.");
            }
        }
        else
        {
            Debug.Log("Spadata mancata!");
        }

        // 5. Fine attacco e cooldown
        isAttacking = false;

        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    // Disegna un cerchio rosso su Unity per aiutarti a piazzare l'Attack Point
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
