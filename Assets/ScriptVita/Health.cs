using UnityEngine;
using UnityEngine.Events; // Permette di usare gli Eventi di Unity

public class Health : MonoBehaviour
{
    [Header("Salute")]
    public int currentHealth;
    public int maxHealth;

    [Header("Cooldown Danni")]
    public float invincibilityTime = 1f;
    private float lastHitTime = -10f;

    [Header("Eventi")]
    public UnityEvent OnDeath; // crea lo slot per l'evento di morte

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (Time.time < lastHitTime + invincibilityTime) return;
            lastHitTime = Time.time;
        }

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        // Controllo morte universale
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // Lancia l'evento! Chiunque stia ascoltando reagirà.
            if (OnDeath != null)
            {
                OnDeath.Invoke();
            }
        }
    }
}
