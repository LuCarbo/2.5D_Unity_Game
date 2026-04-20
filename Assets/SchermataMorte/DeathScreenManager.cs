using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [Header("Impostazioni Dissolvenza")]
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1.5f;

    private Coroutine currentFade;

    void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowDeathScreen()
    {
        if (currentFade != null) StopCoroutine(currentFade);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Se il tuo gioco nasconde il cursore del mouse mentre giochi, devi riattivarlo qui!
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentFade = StartCoroutine(FadeTo(1f));
    }

    public void HideDeathScreenInstantly()
    {
        if (currentFade != null) StopCoroutine(currentFade);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Possibilita` di nascondere il cursore
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (canvasGroup == null) yield break;
        float startAlpha = canvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    // Metodo da collegare al bottone "Riprova"
    public void RestartFromCheckpoint()
    {
        // Cerca lo script di respawn nel gioco e fallo partire
        PlayerRespawn respawnScript = Object.FindAnyObjectByType<PlayerRespawn>();

        if (respawnScript != null)
        {
            respawnScript.Respawn();
        }
        else
        {
            Debug.LogError("ERRORE: Impossibile trovare PlayerRespawn nella scena!");
        }
    }

    // Metodo da collegare al bottone "Menu Principale"
    public void ReturnToMainMenu()
    {
        // ASSICURATI che la scena del menu sia nelle Build Settings!
        SceneManager.LoadScene("StartingMenu");
    }
}