using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public bool staParlando = false;
    private PlayerInputHandler inputPersonaggio;
    private Queue<string> frasiInCoda;
    private float tempoUltimoInput;
    private Coroutine animazioneTesto;
    private bool staScrivendo = false;
    private string fraseCorrente;

    [Header("Impostazioni Testo")]
    public float velocitaScrittura = 0.03f;
    private GameObject pannelloAttivo;
    private TextMeshProUGUI testoAttivo;
    private DialoguePanelResizer resizerAttivo;

    // --- NUOVO: Evita il doppio trigger accidentale ---
    private bool aspettaRilascioTasto = false;

    void Start()
    {
        frasiInCoda = new Queue<string>();
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {
        if (staParlando && inputPersonaggio != null)
        {
            // 1. Se il giocatore rilascia il tasto, sblocchiamo l'input
            if (!inputPersonaggio.InteractPressed)
            {
                aspettaRilascioTasto = false;
            }

            // 2. Avanza solo se il tasto è premuto E precedentemente rilasciato
            if (inputPersonaggio.InteractPressed && !aspettaRilascioTasto)
            {
                if (Time.time - tempoUltimoInput > 0.2f)
                {
                    tempoUltimoInput = Time.time;
                    aspettaRilascioTasto = true; // Blocca fino al prossimo rilascio

                    if (staScrivendo)
                        CompletaFrase();
                    else
                        MostraProssimaFrase();
                }
            }
        }
    }

    public void AvviaDialogo(DialogueData dialogo, GameObject pannelloNPC, TextMeshProUGUI testoNPC, DialoguePanelResizer resizer = null)
    {
        staParlando = true;
        pannelloAttivo = pannelloNPC;
        testoAttivo = testoNPC;
        resizerAttivo = resizer;

        pannelloAttivo.SetActive(true);
        frasiInCoda.Clear();

        foreach (string frase in dialogo.frasi)
            frasiInCoda.Enqueue(frase);

        tempoUltimoInput = Time.time;
        aspettaRilascioTasto = true; // Blocca subito l'input per evitare skip istantanei

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
        if (animazioneTesto != null)
            StopCoroutine(animazioneTesto);
        animazioneTesto = StartCoroutine(EffettoMacchinaDaScrivere(fraseCorrente));
    }
    private IEnumerator EffettoMacchinaDaScrivere(string testo)
    {
        staScrivendo = true;
        testoAttivo.text = testo;
        testoAttivo.maxVisibleCharacters = 0;
        // Aspetta che TMP processi il testo nel suo ciclo interno
        yield return null;
        // *** QUI è il momento giusto: TMP ha i bounds pronti ***
        if (resizerAttivo != null)
            resizerAttivo.AggiornaDimensioni();
        int totaleLettere = testoAttivo.textInfo.characterCount;
        int lettereVisibili = 0;
        while (lettereVisibili <= totaleLettere)
        {
            testoAttivo.maxVisibleCharacters = lettereVisibili;
            lettereVisibili++;
            yield return new WaitForSeconds(velocitaScrittura);
        }
        staScrivendo = false;
    }
    private void CompletaFrase()
    {
        if (animazioneTesto != null)
            StopCoroutine(animazioneTesto);
        testoAttivo.maxVisibleCharacters = testoAttivo.textInfo.characterCount;
        staScrivendo = false;
    }
    public void TerminaDialogo()
    {
        if (animazioneTesto != null)
        {
            StopCoroutine(animazioneTesto);
            animazioneTesto = null;
        }

        if (pannelloAttivo != null)
            pannelloAttivo.SetActive(false);

        Invoke("ResettaDialogo", 0.2f);
    }
    void ResettaDialogo()
    {
        staParlando = false;
        pannelloAttivo = null;
        testoAttivo = null;
        resizerAttivo = null;
    }
}