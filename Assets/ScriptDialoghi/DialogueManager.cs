using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
                MostraProssimaFrase();
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

        string fraseCorrente = frasiInCoda.Dequeue();
        fraseText.text = fraseCorrente;
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
