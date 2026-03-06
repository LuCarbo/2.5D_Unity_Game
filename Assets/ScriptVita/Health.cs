using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Salute")]
    public int currentHealth;
    public int maxHealth; // Es: 12 significa 3 cuori interi (12 / 4)

    [Header("Cooldown Danni")]
    public float invincibilityTime = 1f;
    private float lastHitTime = -10f;

    [Header("Eventi")]
    public UnityEvent OnDeath;

    void Start()
    {
        // Imposta la vita al massimo all'inizio del gioco
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        // Se stiamo subendo danno (valore negativo), controlliamo l'invincibilità
        if (amount < 0)
        {
            if (Time.time < lastHitTime + invincibilityTime) return;
            lastHitTime = Time.time;
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

    // Metodo extra utile se vuoi aggiungere un contenitore di cuore (aumenta la vita massima di 4)
    public void AddHeartContainer()
    {
        maxHealth += 4;
        currentHealth = maxHealth; // Cura completamente il giocatore
    }
}
