using UnityEngine;
using System.Collections; // Necessario per le Coroutine (IEnumerator)
using TMPro; // Se usi TextMeshPro

public class TimedTextTrigger : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [Tooltip("Il GameObject del testo che contiene il Canvas Group")]
    public CanvasGroup textCanvasGroup;

    [Header("Impostazioni Temporali (Secondi)")]
    public float fadeInDuration = 1.0f; // Quanto ci mette ad apparire
    public float displayDuration = 5.0f; // Quanto rimane visibile (la tua richiesta)
    public float fadeOutDuration = 1.0f; // Quanto ci mette a scomparire

    // Variabile di stato per evitare che il trigger si riattivi mentre il testo è già visibile
    private bool isSequenceRunning = false;

    private void Start()
    {
        // Assicuriamoci che il testo sia invisibile e non interagibile all'avvio
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
            textCanvasGroup.gameObject.SetActive(false); // Opzionale, ma pulito
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se è il Player E se la sequenza non è già in corso
        if (other.CompareTag("Player") && !isSequenceRunning)
        {
            // Avvia la Coroutine
            StartCoroutine(TextSequenceRoutine());
        }
    }

    // Questa è la Coroutine che gestisce l'intera logica temporale
    private IEnumerator TextSequenceRoutine()
    {
        isSequenceRunning = true; // Blocca ulteriori attivazioni

        // --- FASE 1: ATTIVAZIONE E FADE IN ---
        textCanvasGroup.gameObject.SetActive(true); // Attiva l'oggetto
        float counter = 0f;

        while (counter < fadeInDuration)
        {
            counter += Time.deltaTime; // Aumenta il contatore in base al tempo passato
            // Calcola l'alpha (va da 0 a 1 nell'arco di fadeInDuration)
            textCanvasGroup.alpha = Mathf.Lerp(0f, 1f, counter / fadeInDuration);
            yield return null; // "Metti in pausa" fino al prossimo frame
        }
        textCanvasGroup.alpha = 1f; // Assicurati che sia perfettamente visibile

        // --- FASE 2: ATTESA (LA TUA RICHIESTA) ---
        // Unity si ferma qui per ESATTAMENTE 5 secondi (o il valore impostato)
        // Il giocatore può uscire dal collider, non importa.
        yield return new WaitForSeconds(displayDuration);

        // --- FASE 3: FADE OUT ---
        counter = 0f;
        while (counter < fadeOutDuration)
        {
            counter += Time.deltaTime;
            // Calcola l'alpha (va da 1 a 0)
            textCanvasGroup.alpha = Mathf.Lerp(1f, 0f, counter / fadeOutDuration);
            yield return null; // Aspetta il prossimo frame
        }
        textCanvasGroup.alpha = 0f; // Assicurati che sia invisibile

        // --- FASE 4: DISATTIVAZIONE ---
        textCanvasGroup.gameObject.SetActive(false); // Disattiva l'oggetto
        isSequenceRunning = false; // Sblocca il trigger per una futura riattivazione
    }
}