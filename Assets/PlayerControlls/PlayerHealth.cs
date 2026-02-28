using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [Header("Salute")]
    public int currentHealth;
    public int maxHealth;

    [Header("Cooldown Danni")]
    public float invincibilityTime = 1f; // Tempo di respiro tra un danno e l'altro (1 secondo)
    private float lastHitTime = -10f;    // Memorizza quando abbiamo preso l'ultimo colpo

    public void changeHealth(int amount)
    {

        // Se stiamo subendo un danno (quindi amount è un numero negativo)
        if (amount < 0)
        {

            // Se NON è passato ancora 1 secondo dall'ultimo colpo, blocchiamo tutto ed usciamo
            if (Time.time < lastHitTime + invincibilityTime)
            {
                return;
            }

            // Se invece è passato abbastanza tempo, registriamo il momento di questo nuovo colpo
            lastHitTime = Time.time;
        }

        // Applichiamo la cura o il danno
        currentHealth += amount;

        // Controllo morte
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameObject.SetActive(false); // Disattiva il Player
        }
    }
}
