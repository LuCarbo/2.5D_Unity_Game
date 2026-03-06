using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;  // Riferimento al "cervello" della vita
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite threeQuarterHeart;
    public Sprite halfHeart;
    public Sprite oneQuarterHeart;
    public Sprite emptyHeart;

    private int healthPerHeart = 4; // 4 Punti Vita = 1 Cuore intero

    void Update()
    {
        // Se non abbiamo assegnato lo script Health, fermati per evitare errori
        if (playerHealth == null) return;

        // Quanti contenitori di cuori dobbiamo mostrare in totale? (es. 12 / 4 = 3 cuori)
        int numOfHeartContainers = playerHealth.maxHealth / healthPerHeart;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Abilita i contenitori necessari e nascondi quelli in eccesso
            if (i < numOfHeartContainers)
            {
                hearts[i].enabled = true;

                // Calcola quanti HP spettano a QUESTO specifico cuore
                // Esempio: se currentHealth è 5 e siamo al secondo cuore (i=1): 5 - (1 * 4) = 1 HP rimanente per questo cuore
                int heartSegmentHealth = playerHealth.currentHealth - (i * healthPerHeart);

                // Determina quale sprite mostrare usando i numeri interi
                if (heartSegmentHealth >= 4)
                {
                    hearts[i].sprite = fullHeart;
                }
                else if (heartSegmentHealth == 3)
                {
                    hearts[i].sprite = threeQuarterHeart;
                }
                else if (heartSegmentHealth == 2)
                {
                    hearts[i].sprite = halfHeart;
                }
                else if (heartSegmentHealth == 1)
                {
                    hearts[i].sprite = oneQuarterHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}