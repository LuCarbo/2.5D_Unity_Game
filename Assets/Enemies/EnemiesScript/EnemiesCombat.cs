using UnityEngine;

public class EnemiesCombat : MonoBehaviour
{
    [Header("Statistiche Combattimento")]
    public int damage = 1;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;
    private EnemiesScript movementScript;

    private void Start()
    {
        movementScript = GetComponent<EnemiesScript>();
    }

    public void TryAttack(GameObject target)
    {
        // Se il cooldown è finito, possiamo attaccare!
        if (Time.time >= nextAttackTime)
        {
            Debug.Log($"[{gameObject.name}] Il Player è a tiro! Lancio l'attacco...");

            // 1. IL FIX: Cerchiamo la vita anche nell'oggetto "Padre" del Player
            Health targetHealth = target.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                targetHealth.ChangeHealth(-damage);
                Debug.Log("SUCCESSO: Danno inflitto al Player!");
            }
            else
            {
                Debug.LogError("ERRORE: Il lupo ti ha preso, ma non trova lo script Health sul Player!");
            }

            // 2. Imposta il timer per il prossimo attacco
            nextAttackTime = Time.time + attackCooldown;

            // 3. Mette in pausa e avvia l'animazione
            if (movementScript != null)
            {
                movementScript.PauseMovement(attackCooldown);
                movementScript.PlayAttackAnimation();
            }
        }
    }
}
