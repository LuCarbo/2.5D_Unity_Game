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

    // RIMOSSO: attackDelay non serve più!

    [Header("Maschere di Collisione")]
    public LayerMask playerLayer;       // Il layer assegnato al tuo Player
    public LayerMask groundLayer;       // Layer del terreno

    [Tooltip("Tempo massimo di attesa per toccare terra prima di forzare lo sblocco del boss")]
    public float groundCheckTimeout = 2f;

    // Variabili di stato
    public bool isAttacking = false;
    public bool isOnCooldown = false;

    public Animator anim;
    private BossMovement bossMovement;

    private void Awake()
    {
        bossMovement = GetComponent<BossMovement>();
    }

    // 1. Questo viene chiamato quando il boss DECIDE di attaccare
    public void PerformAttack()
    {
        if (!isAttacking && !isOnCooldown)
        {
            isAttacking = true;

            // FERMA IL BOSS
            if (bossMovement != null)
            {
                bossMovement.StopMoving();
                bossMovement.LockMovement();
            }

            // Fai partire l'animazione. Sarà l'animazione a far scattare il resto!
            if (anim != null) anim.SetTrigger("Attack");
        }
    }

    // 2. NUOVO: Questo metodo DEVE essere "public". Lo chiameremo tramite l'Animation Event su Unity!
    public void AnimationEvent_Strike()
    {
        // Quando l'animazione arriva al frame giusto, facciamo partire il danno e il controllo del terreno
        StartCoroutine(StrikeAndRecoverRoutine());
    }

    // 3. La logica del colpo e del recupero
    private IEnumerator StrikeAndRecoverRoutine()
    {
        // IL COLPO 3D
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayers.Length > 0)
        {
            Collider hitPlayer = hitPlayers[0];
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

        // ATTESA CHE LA SPADA TOCCHI TERRA
        float timer = 0f;

        while (!Physics.CheckSphere(attackPoint.position, attackRadius, groundLayer) && timer < groundCheckTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer >= groundCheckTimeout)
        {
            Debug.LogWarning("La spada non ha toccato terra in tempo! Sblocco forzato attivato.");
        }

        // SBLOCCA IL BOSS
        if (bossMovement != null)
        {
            bossMovement.UnlockMovement();
        }

        // Fine attacco e cooldown
        isAttacking = false;
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void CancelAttack()
    {
        StopAllCoroutines(); // Ferma la logica del colpo

        isAttacking = false;
        isOnCooldown = false;

        // Sblocca immediatamente le gambe
        if (bossMovement != null)
        {
            bossMovement.UnlockMovement();
        }

        Debug.Log("Attacco interrotto! Il boss si è sbloccato e ha azzerato il timer.");
    }
}