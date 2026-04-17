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

    [Header("Audio")]
    public AudioSource audioSource;
    private AudioClip voceCorrente;
    private float tempoUltimoSuono = 0f;
    public float intervalloSuono = 0.08f;

    private bool aspettaRilascioTasto = false;

    void Start()
    {
        frasiInCoda = new Queue<string>();
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        DialogueManager[] tuttiIManager = FindObjectsByType<DialogueManager>(FindObjectsSortMode.None);
        if (tuttiIManager.Length > 1)
            Debug.LogWarning($"ATTENZIONE: trovate {tuttiIManager.Length} istanze di DialogueManager! Rimuovi il componente da tutti i GameObject tranne '{gameObject.name}'.");
    }

    void Update()
    {
        if (staParlando && inputPersonaggio != null)
        {
            if (!inputPersonaggio.InteractPressed)
                aspettaRilascioTasto = false;

            if (inputPersonaggio.InteractPressed && !aspettaRilascioTasto)
            {
                if (Time.time - tempoUltimoInput > 0.2f)
                {
                    tempoUltimoInput = Time.time;
                    aspettaRilascioTasto = true;

                    if (staScrivendo)
                        CompletaFrase();
                    else
                        MostraProssimaFrase();
                }
            }
        }
    }

    public void AvviaDialogo(DialogueData dialogo, GameObject pannelloNPC, TextMeshProUGUI testoNPC, DialoguePanelResizer resizer = null, AudioClip voceNPC = null)
    {
        staParlando = true;
        pannelloAttivo = pannelloNPC;
        testoAttivo = testoNPC;
        resizerAttivo = resizer;
        voceCorrente = voceNPC;

        pannelloAttivo.SetActive(true);
        frasiInCoda.Clear();

        foreach (string frase in dialogo.frasi)
            frasiInCoda.Enqueue(frase);

        tempoUltimoInput = Time.time;
        aspettaRilascioTasto = true;

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

        yield return null;

        if (resizerAttivo != null)
            resizerAttivo.AggiornaDimensioni();

        int totaleLettere = testoAttivo.textInfo.characterCount;
        int lettereVisibili = 0;

        while (lettereVisibili <= totaleLettere)
        {
            testoAttivo.maxVisibleCharacters = lettereVisibili;

            if (audioSource != null && voceCorrente != null && lettereVisibili > 0 && lettereVisibili <= totaleLettere)
            {
                char carattereCorrente = testoAttivo.textInfo.characterInfo[lettereVisibili - 1].character;

                if (!char.IsWhiteSpace(carattereCorrente) && Time.time - tempoUltimoSuono >= intervalloSuono)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(voceCorrente);
                    tempoUltimoSuono = Time.time;
                }
            }

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
        voceCorrente = null;
    }
}