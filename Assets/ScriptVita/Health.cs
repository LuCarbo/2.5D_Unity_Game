using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using UnityEngine;

public class Health : MonoBehaviour

{

    [Header("Salute")]

    public int currentHealth;

    public int maxHealth; // Es: 12 significa 3 cuori interi (12 / 4)



    // NUOVO: Aggiungiamo un flag per sapere se è già morto

    private bool isDeadFlag = false;



    [Header("Cooldown Danni")]

    public float invincibilityTime = 1f;

    private float lastHitTime = -10f;



    [Header("Animazione")]

    [Tooltip("Trascina qui l'Animator del tuo personaggio")]

    [SerializeField] private Animator animator;



    void Start()

    {

        currentHealth = maxHealth;



        if (animator == null)

        {

            animator = GetComponentInChildren<Animator>();

        }

    }



    public void ChangeHealth(int amount)

    {

        if (isDeadFlag) return;



        // Calcoliamo in anticipo quale sarà la vita dopo il danno/cura

        int expectedHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);



        if (amount < 0)

        {

            if (Time.time < lastHitTime + invincibilityTime) return;

            lastHitTime = Time.time;



            // Se questo colpo è letale, si muore e basta (niente isHit)

            if (expectedHealth == 0)

            {

                currentHealth = 0;

                Die();

                return; // Esce dal metodo qui, così non legge la parte di "isHit"

            }



            // Se sopravvive, fa partire l'animazione del danno

            if (animator != null)

            {

                animator.SetBool("IsHit", true);

                StartCoroutine(ResetHitBool());

            }

        }



        // Applica la cura o il danno (se è sopravvissuto)

        currentHealth = expectedHealth;

    }



    private IEnumerator ResetHitBool()

    {

        yield return new WaitForSeconds(0.1f);

        if (animator != null)

        {

            animator.SetBool("IsHit", false);

        }

    }



    // NUOVO: Metodo che gestisce la vera morte

    private void Die()
    {
        isDeadFlag = true;
        Debug.Log($"{gameObject.name} ha la vita a zero!");

        EnemiesScript enemyLogic = GetComponent<EnemiesScript>();

        if (enemyLogic != null)
        {
            // === È UN NEMICO ===
            enemyLogic.Die();
        }
        else
        {
            // === È IL PLAYER ===
            if (animator != null)
            {
                animator.SetBool("IsDead", true);
            }

            // Spegniamo i comandi
            PlayerCombat combatScript = GetComponent<PlayerCombat>();
            if (combatScript != null) combatScript.enabled = false;

            PlayerInputHandler inputScript = GetComponent<PlayerInputHandler>();
            if (inputScript != null) inputScript.enabled = false;

            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // INIZIA IL COUNTDOWN PER IL RESPAWN (Es. aspetta 3 secondi prima di rinascere)
            StartCoroutine(WaitAndRespawn());
        }
    }

    // NUOVO: La Coroutine che aspetta e poi chiama il tuo script di Respawn
    private IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(3f); // Cambia questo numero per aspettare di più o di meno

        PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();
        if (respawnScript != null)
        {
            respawnScript.Respawn();
        }
    }



    public void AddHeartContainer()

    {

        if (isDeadFlag) return; // Non resuscitiamo con un contenitore

        maxHealth += 4;

        currentHealth = maxHealth;

    }



    public void ResetHealth()

    {

        isDeadFlag = false; // Resettiamo la morte

        currentHealth = maxHealth;

        lastHitTime = -10f;



        if (animator != null)

        {

            animator.SetBool("IsDead", false);

        }

    }

}