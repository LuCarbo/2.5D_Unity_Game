using System.Collections;
using UnityEngine;

public class GestoreMusica : MonoBehaviour
{
    // Questo crea una "linea diretta" così le zone possono chiamare 
    // questo script senza doverlo cercare ogni volta.
    public static GestoreMusica istanza;

    [Header("Impostazioni Audio")]
    public AudioSource audioSource;
    public float tempoDiFade = 1.5f; // Quanto ci mette a cambiare canzone o a sfumare

    private float volumeMassimo;

    void Awake()
    {
        istanza = this;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        volumeMassimo = audioSource.volume; // Salva il volume iniziale che hai messo nell'Inspector
    }

    public void CambiaCanzone(AudioClip nuovaCanzone)
    {
        // Se sta già suonando questa canzone, non fare nulla!
        if (audioSource.clip == nuovaCanzone) return;

        // Ferma eventuali cambi di musica precedenti a metà
        StopAllCoroutines();
        StartCoroutine(FadeMusica(nuovaCanzone));
    }

    IEnumerator FadeMusica(AudioClip nuovaCanzone)
    {
        // 1. FADE OUT (Abbassa la musica attuale)
        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeMassimo * Time.deltaTime / tempoDiFade;
            yield return null;
        }

        // 2. CAMBIO TRACCIA
        audioSource.clip = nuovaCanzone;
        audioSource.Play();

        // 3. FADE IN (Alza la nuova musica)
        while (audioSource.volume < volumeMassimo)
        {
            audioSource.volume += volumeMassimo * Time.deltaTime / tempoDiFade;
            yield return null;
        }

        audioSource.volume = volumeMassimo; // Assicuriamoci che sia esattamente al massimo
    }

    // --- ECCO LA FUNZIONE CHE MANCAVA! ---
    // Chiama questa funzione per sfumare verso il silenzio totale (es. morte boss)
    public void FermaMusica()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutTotale());
    }

    IEnumerator FadeOutTotale()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeMassimo * Time.deltaTime / tempoDiFade;
            yield return null;
        }

        audioSource.Stop(); // Ferma la traccia
        audioSource.clip = null; // Svuota lo slot
    }
}