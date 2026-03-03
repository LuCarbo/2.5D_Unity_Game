using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Riferimenti")]
    public PlayerInputHandler inputHandler; // Script degli input
    public Transform attackPoint;           // Un oggetto che fa da centro dell'attacco

    [Header("Statistiche Attacco")]
    public int attackDamage = 1;
    public float attackRange = 1f;          // Raggio della sfera di attacco
    public LayerMask enemyLayers;           // Per colpire solo i nemici (ignora i muri)

    [Header("Cooldown")]
    public float attackRate = 2f;           // Quanti attacchi al secondo
    private float nextAttackTime = 0f;

    // Variabili per memorizzare la posizione
    private float attackDistance;
    private float defaultHeight;

    void Start()
    {
        if (attackPoint != null)
        {
            // Calcoliamo quanto è distante l'attackPoint dal centro del giocatore (es. 0.8)
            attackDistance = Mathf.Abs(attackPoint.localPosition.x);

            if (attackDistance == 0) attackDistance = Mathf.Abs(attackPoint.localPosition.z);

            defaultHeight = attackPoint.localPosition.y;
        }
    }

    void Update()
    {
        // 1. GESTIONE DELLA DIREZIONE DELL'ATTACCO
        float moveX = inputHandler.MoveInput.x;
        float moveY = inputHandler.MoveInput.y; // Attenzione: la "Y" dell'input è "Su/Giù" sulla tastiera

        // Controllo se il giocatore si sta muovendo
        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
        {
            // Capisco se sta andando più in orizzontale o più in verticale
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                // MOVIMENTO ORIZZONTALE (Destra/Sinistra sull'asse X)
                if (moveX > 0)
                {
                    attackPoint.localPosition = new Vector3(attackDistance, defaultHeight, 0); // Destra
                }
                else
                {
                    attackPoint.localPosition = new Vector3(-attackDistance, defaultHeight, 0); // Sinistra
                }
            }
            else
            {
                // MOVIMENTO VERTICALE
                if (moveY > 0)
                {
                    attackPoint.localPosition = new Vector3(0, defaultHeight, attackDistance); // Su (Allontana)
                }
                else
                {
                    attackPoint.localPosition = new Vector3(0, defaultHeight, -attackDistance); // Giù (Avvicina)
                }
            }
        }

        // 2. LOGICA DI ATTACCO
        if (Time.time >= nextAttackTime)
        {
            if (inputHandler.AttackPressed)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. (Opzionale in futuro) Fai partire l'animazione di attacco dell'Animator del Player qui

        // 2. Rileva tutti i nemici nel raggio d'azione
        // Crea una sfera invisibile partendo dall'attackPoint
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 3. Infliggi danno a ciascun nemico colpito
        foreach (Collider enemy in hitEnemies)
        {
            // Cerchiamo il nostro famoso script universale 'Health' sul nemico
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-attackDamage);
                Debug.Log("Colpito: " + enemy.name + " per " + attackDamage + " danni!");
            }
        }
    }

    // Questa funzione serve solo a noi per vedere la sfera di attacco in Unity
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
