using UnityEngine;

public class EnemiesCombat : MonoBehaviour
{

    public int damage = 1;
    public float attackCooldown = 1.5f; // Tempo di attesa tra un attacco e l'altro
    public float nextAttackTime = 0f; // Tempo in cui il nemico può attaccare di nuovo

    private void OnCollisionStay(Collision collision) {

        // 1. Controlliamo prima di tutto se l'oggetto toccato è il Player
        if (collision.gameObject.CompareTag("Player")) {

            // 2. Controlliamo se è passato abbastanza tempo dall'ultimo attacco
            if (Time.time >= nextAttackTime)
            {
                // Infligge danno
                Health player = collision.gameObject.GetComponent<Health>();
                if (player != null)
                {
                    player.ChangeHealth(-damage);
                }

                // Imposta il tempo del prossimo attacco
                nextAttackTime = Time.time + attackCooldown;

                // 3. Crea lo script di movimento sul nemico per metterlo in pausa
                EnemiesScript movementScript = GetComponent<EnemiesScript>();
                if (movementScript != null)
                {
                    movementScript.PauseMovement(attackCooldown);
                }
            }
        }
    }
}
