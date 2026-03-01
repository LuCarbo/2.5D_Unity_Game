using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("Settings")]
    public float currentHealth;   // Ora è un float per i quarti
    public float maxHealth;       // Vita massima totale (es. 3 cuori * 4 = 12)
    public int numOfHeartContainers; // Numero di cuori interi visibili (es. 3)

    [Header("References")]
    public Image[] hearts;       // Array di oggetti Image per i cuori
    public Sprite fullHeart;    
    public Sprite threeQuarterHeart;
    public Sprite halfHeart;
    public Sprite oneQuarterHeart;
    public Sprite emptyHeart;

    private float healthPerHeart = 4f; // Ogni cuore intero ha 4 "punti salute" (per i quarti)

    void Start()
    {
        // Inizializza la vita massima basandoti sul numero di cuori
        maxHealth = numOfHeartContainers * healthPerHeart;
        currentHealth = maxHealth; // Inizia con vita piena
    }

    void Update()
    {
        // Assicurati che la vita non vada oltre il massimo o sotto lo zero
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            // Abilita/disabilita i contenitori dei cuori
            if (i < numOfHeartContainers)
            {
                hearts[i].enabled = true;

                // Calcola la vita rimanente per questo specifico contenitore di cuore
                float heartSegmentHealth = currentHealth - (i * healthPerHeart);

                // Determina quale sprite mostrare
                if (heartSegmentHealth >= healthPerHeart)
                {
                    hearts[i].sprite = fullHeart;
                }
                else if (heartSegmentHealth >= healthPerHeart * 0.75f) // 3/4
                {
                    hearts[i].sprite = threeQuarterHeart;
                }
                else if (heartSegmentHealth >= healthPerHeart * 0.5f) // 1/2
                {
                    hearts[i].sprite = halfHeart;
                }
                else if (heartSegmentHealth >= healthPerHeart * 0.25f) // 1/4
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
                hearts[i].enabled = false; // Nasconde i cuori in eccesso
            }
        }
    }

    // Metodo per subire danni (ora accetta float per supportare i quarti)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    // Metodo per curarsi
    public void Heal(float amount)
    {
        currentHealth += amount;
    }

    // Metodo per aumentare i contenitori di cuori (es. dopo un boss)
    public void AddHeartContainer()
    {
        numOfHeartContainers++;
        maxHealth = numOfHeartContainers * healthPerHeart; // Aggiorna la vita massima
        currentHealth = maxHealth; // Di solito, quando si aggiunge un cuore, si ripristina la vita
    }
}