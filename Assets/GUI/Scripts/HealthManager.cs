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

        // Controllo di sicurezza estrema: se l'array dei cuori non esiste, fermati
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            // --- LO SCUDO ANTI-CRASH ---
            // Se nell'Inspector ti sei dimenticato di assegnare un'immagine in questo slot, saltalo!
            if (hearts[i] == null) continue;

            // Abilita i contenitori necessari e nascondi quelli in eccesso
            if (i < numOfHeartContainers)
            {
                hearts[i].enabled = true;

                // Calcola quanti HP spettano a QUESTO specifico cuore
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