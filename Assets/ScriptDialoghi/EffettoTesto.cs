using System.Collections;
using UnityEngine;
using TMPro; // Indispensabile per usare TextMeshPro

public class EffettoTesto : MonoBehaviour
{
    [Header("Impostazioni UI")]
    public TextMeshProUGUI testoDialogo; // Trascina qui il tuo TestoDialogo
    
    [Header("Impostazioni Velocità")]
    public float ritardoTraLettere = 0.05f; // Quanto tempo passa tra una lettera e l'altra

    // Questa è la funzione che chiamerai per far partire il dialogo
    public void MostraTesto(string nuovoTesto)
    {
        // 1. Imposta il testo nel componente
        testoDialogo.text = nuovoTesto;
        // 2. Rendi visibili 0 caratteri all'inizio
        testoDialogo.maxVisibleCharacters = 0; 
        // 3. Fai partire l'animazione
        StartCoroutine(ScriviTesto());
    }

    private IEnumerator ScriviTesto()
    {
        // Trova quante lettere ci sono in totale
        int totaleLettere = testoDialogo.textInfo.characterCount;
        int lettereVisibili = 0;

        while (lettereVisibili <= totaleLettere)
        {
            // Mostra una lettera in più
            testoDialogo.maxVisibleCharacters = lettereVisibili;
            lettereVisibili++;
            
            // Aspetta un po' prima di mostrare la prossima
            yield return new WaitForSeconds(ritardoTraLettere);
        }
    }
    
    // Variabile per tenere traccia dell'animazione (metti questa all'inizio della classe)
    private Coroutine animazioneTesto;

    // La coroutine che fa l'effetto
    private IEnumerator EffettoMacchinaDaScrivere(string testo)
    {
        // Imposta il testo ma lo rende invisibile
        testoDialogo.text = testo;
        testoDialogo.maxVisibleCharacters = 0;
        
        // Aspetta un frame per permettere a TextMeshPro di contare le lettere ignorando eventuali tag (es. <color=red>)
        yield return null; 

        int totaleLettere = testoDialogo.textInfo.characterCount;
        int lettereVisibili = 0;

        while (lettereVisibili <= totaleLettere)
        {
            testoDialogo.maxVisibleCharacters = lettereVisibili;
            lettereVisibili++;
            yield return new WaitForSeconds(0.03f); // <-- Qui puoi cambiare la velocità!
        }
    }
}