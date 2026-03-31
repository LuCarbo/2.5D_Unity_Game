using UnityEngine;
using UnityEngine.Events;
using System.Collections; // Required for Coroutines

public class Health : MonoBehaviour
{
    [Header("Salute")]
    public int currentHealth;
    public int maxHealth; // Es: 12 significa 3 cuori interi (12 / 4)

    [Header("Cooldown Danni")]
    public float invincibilityTime = 1f;
    private float lastHitTime = -10f;

    [Header("Animazione")]
    [Tooltip("Trascina qui l'Animator del tuo personaggio")]
    [SerializeField] private Animator animator; // Riferimento all'Animator

    [Header("Eventi")]
    public UnityEvent OnDeath;

    void Start()
    {
        // Imposta la vita al massimo all'inizio del gioco
        currentHealth = maxHealth;

        // Auto-assegnazione dell'animator se dimentichi di trascinarlo nell'Inspector
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void ChangeHealth(int amount)
    {
        // Se stiamo subendo danno (valore negativo), controlliamo l'invincibilità
        if (amount < 0)
        {
            if (Time.time < lastHitTime + invincibilityTime) return;
            lastHitTime = Time.time;

            Debug.Log($"{gameObject.name} è stato colpito! Danno subito: {amount}");

            // Attiva il bool dell'animazione e avvia il timer per spegnerlo
            if (animator != null)
            {
                animator.SetBool("isHit", true);
                StartCoroutine(ResetHitBool()); // Avvia il ritardo
            }
        }

        // Modifica la vita e assicurati che non superi il massimo o scenda sotto lo zero
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Controllo morte
        if (currentHealth <= 0)
        {
            if (OnDeath != null)
            {
                OnDeath.Invoke();
            }
        }
    }

    // Questa Coroutine aspetta 0.1 secondi prima di spegnere il bool
    private IEnumerator ResetHitBool()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
        {
            animator.SetBool("isHit", false);
        }
    }

    // Metodo extra utile se vuoi aggiungere un contenitore di cuore (aumenta la vita massima di 4)
    public void AddHeartContainer()
    {
        maxHealth += 4;
        currentHealth = maxHealth; // Cura completamente il giocatore
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        lastHitTime = -10f; // Resetta anche il timer di invincibilità per sicurezza
    }
}