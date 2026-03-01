using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections; // Aggiunto: Indispensabile per usare le Coroutine!

public class DialogueManager : MonoBehaviour
{
    public GameObject pannelloDialogo;
    public TextMeshProUGUI nomeText;
    public TextMeshProUGUI fraseText;

    // Questa variabile comunica all'NPC se siamo occupati a parlare
    public bool staParlando = false;

    private PlayerInputHandler inputPersonaggio;
    private Queue<string> frasiInCoda;
    private float tempoUltimoInput;

    // --- NUOVE VARIABILI PER L'EFFETTO TESTO ---
    private Coroutine animazioneTesto;
    private bool staScrivendo = false;
    private string fraseCorrente; 
    
    [Header("Impostazioni Testo")]
    public float velocitaScrittura = 0.03f; // Puoi cambiare la velocità direttamente dall'Inspector di Unity

    void Start()
    {
        frasiInCoda = new Queue<string>();
        pannelloDialogo.SetActive(false);
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {
        // Usiamo staParlando al posto di activeInHierarchy
        if (staParlando && inputPersonaggio != null && inputPersonaggio.InteractPressed)
        {
            // Protezione extra: aspetta un istante tra una frase e l'altra
            if (Time.time - tempoUltimoInput > 0.2f)
            {
                tempoUltimoInput = Time.time;
                
                // NUOVO CONTROLLO: Se sta ancora scrivendo, completa la frase istantaneamente.
                // Altrimenti, passa alla frase successiva.
                if (staScrivendo)
                {
                    CompletaFrase();
                }
                else
                {
                    MostraProssimaFrase();
                }
            }
        }
    }

    public void AvviaDialogo(DialogueData dialogo)
    {
        staParlando = true;
        pannelloDialogo.SetActive(true);
        nomeText.text = dialogo.nomePersonaggio;
        frasiInCoda.Clear();

        foreach (string frase in dialogo.frasi)
        {
            frasiInCoda.Enqueue(frase);
        }

        tempoUltimoInput = Time.time;
        MostraProssimaFrase();
    }

    public void MostraProssimaFrase()
    {
        if (frasiInCoda.Count == 0)
        {
            TerminaDialogo();
            return;
        }

        fraseCorrente = frasiInCoda.Dequeue();
        
        // --- QUI ABBIAMO SOSTITUITO IL VECCHIO CODICE ---
        // Se c'è già un'animazione in corso, la fermiamo
        if (animazioneTesto != null)
        {
            StopCoroutine(animazioneTesto);
        }
        // Avviamo l'effetto macchina da scrivere
        animazioneTesto = StartCoroutine(EffettoMacchinaDaScrivere(fraseCorrente));
    }

    // --- LA COROUTINE CHE CREA L'EFFETTO ---
    private IEnumerator EffettoMacchinaDaScrivere(string testo)
    {
        staScrivendo = true; // Segnaliamo che l'animazione è in corso
        fraseText.text = testo;
        fraseText.maxVisibleCharacters = 0;
        
        yield return null; // Aspettiamo un frame per far calcolare le lettere a TextMeshPro

        int totaleLettere = fraseText.textInfo.characterCount;
        int lettereVisibili = 0;

        while (lettereVisibili <= totaleLettere)
        {
            fraseText.maxVisibleCharacters = lettereVisibili;
            lettereVisibili++;
            yield return new WaitForSeconds(velocitaScrittura);
        }
        
        staScrivendo = false; // L'animazione è finita
    }

    // --- FUNZIONE PER SALTARE L'ANIMAZIONE ---
    private void CompletaFrase()
    {
        if (animazioneTesto != null)
        {
            StopCoroutine(animazioneTesto);
        }
        // Mostra tutti i caratteri all'istante
        fraseText.maxVisibleCharacters = fraseText.textInfo.characterCount;
        staScrivendo = false;
    }

    public void TerminaDialogo()
    {
        pannelloDialogo.SetActive(false);
        // Ritardiamo lo "spegnimento" della chiacchierata di 0.2 secondi
        // così l'NPC non equivoca il tuo ultimo clic!
        Invoke("ResettaDialogo", 0.2f);
    }

    void ResettaDialogo()
    {
        staParlando = false;
    }
}