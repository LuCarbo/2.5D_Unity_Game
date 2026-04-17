using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Fondamentale per gestire le interfacce!

public class DeathScreenManager : MonoBehaviour
{
    [Header("Impostazioni")]
    public Image deathImage; // Trascina qui la tua immagine
    public float fadeDuration = 2f; // Quanti secondi ci mette ad apparire?

    public void ShowDeathScreen()
    {
        // Ferma eventuali dissolvenze precedenti e ne fa partire una nuova
        StopAllCoroutines();
        StartCoroutine(FadeInImage());
    }

    public void HideDeathScreenInstantly()
    {
        // Utile per quando rinasci: nasconde l'immagine di colpo
        Color c = deathImage.color;
        deathImage.color = new Color(c.r, c.g, c.b, 0f);
    }

    private IEnumerator FadeInImage()
    {
        // Ci salviamo il colore attuale (che ha l'Alpha a 0)
        Color startColor = deathImage.color;

        // Creiamo il colore di arrivo (lo stesso, ma con l'Alpha a 1, cioè tutto visibile)
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        float timeElapsed = 0f;

        // Finché non è passato il tempo totale...
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // "Lerp" mischia due colori nel tempo. Magia pura!
            deathImage.color = Color.Lerp(startColor, targetColor, timeElapsed / fadeDuration);

            yield return null; // Aspetta il prossimo fotogramma
        }

        // Assicuriamoci che alla fine sia esattamente a 1
        deathImage.color = targetColor;
    }
}
