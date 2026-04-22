using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Salute")]
    public int currentHealth;
    public int maxHealth;

    private bool isDeadFlag = false;

    [Header("Cooldown Danni")]
    public float invincibilityTime = 1f;
    private float lastHitTime = -10f;

    [Header("Animazione")]
    [Tooltip("Trascina qui l'Animator del tuo personaggio")]
    [SerializeField] private Animator animator;

    // NUOVO: Riferimento allo script audio del nemico
    private EnemyAudioManager enemyAudio;

    void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // NUOVO: Cerchiamo l'audio manager nel nipote
        enemyAudio = GetComponentInChildren<EnemyAudioManager>();
    }

    public void ChangeHealth(int amount)
    {
        if (isDeadFlag) return;

        int expectedHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (amount < 0)
        {
            if (Time.time < lastHitTime + invincibilityTime) return;
            lastHitTime = Time.time;

            if (expectedHealth == 0)
            {
                currentHealth = 0;
                Die();
                return;
            }

            // NUOVO: Se il lupo prende danno ma non muore, suona!
            if (enemyAudio != null) enemyAudio.PlayHurtSound();

            if (animator != null)
            {
                animator.SetBool("IsHit", true);
                StartCoroutine(ResetHitBool());
            }
        }

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

    private void Die()
    {
        isDeadFlag = true;
        Debug.Log($"{gameObject.name} ha la vita a zero!");

        if (enemyAudio != null) enemyAudio.PlayDeathSound();

        // 1. Cerca se l'oggetto è un nemico standard (Lupo, Slime)
        EnemiesScript enemyLogic = GetComponent<EnemiesScript>();

        // 2. NUOVO: Cerca se l'oggetto è un animale (Ape)
        AnimalsMovement animalLogic = GetComponent<AnimalsMovement>();

        if (enemyLogic != null)
        {
            // È un nemico normale
            enemyLogic.Die();
        }
        else if (animalLogic != null)
        {
            // È L'APE! Chiama il metodo Die() dello script AnimalsMovement
            animalLogic.Die();
        }
        else if (gameObject.CompareTag("Player"))
        {
            // È IL GIOCATORE
            if (animator != null) animator.SetBool("IsDead", true);

            PlayerCombat combatScript = GetComponent<PlayerCombat>();
            if (combatScript != null) combatScript.enabled = false;

            PlayerInputHandler inputScript = GetComponent<PlayerInputHandler>();
            if (inputScript != null) inputScript.enabled = false;

            CharacterController cc = GetComponent<CharacterController>();
            //if (cc != null) cc.enabled = false;

            DeathScreenManager deathScreen = Object.FindAnyObjectByType<DeathScreenManager>();
            if (deathScreen != null)
            {
                deathScreen.ShowDeathScreen();
            }
        }
        else
        {
            // Fallback: se non è nulla di tutto ciò, distruggilo e basta
            if (animator != null) animator.SetBool("IsDead", true);
            Destroy(gameObject, 1f);
        }
    }

    private IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(3f);

        PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();
        if (respawnScript != null)
        {
            respawnScript.Respawn();
        }
    }

    public void AddHeartContainer()
    {
        if (isDeadFlag) return;
        maxHealth += 4;
        currentHealth = maxHealth;
    }

    public void ResetHealth()
    {
        isDeadFlag = false;
        currentHealth = maxHealth;
        lastHitTime = -10f;

        if (animator != null)
        {
            animator.SetBool("IsDead", false);
        }
    }
}