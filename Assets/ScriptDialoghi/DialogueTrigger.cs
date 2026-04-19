using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Impostazioni Dialogo")]
    public DialogueData dialogo;

    [Header("Audio NPC")]
    public AudioClip voceNPC;

    [Header("Riferimenti Nuvoletta (World Space)")]
    public GameObject pannelloNuvoletta;
    public TextMeshProUGUI testoNuvoletta;
    public DialoguePanelResizer resizerNuvoletta;

    [Header("Chi può far partire il dialogo?")]
    public GameObject ilTuoPersonaggio;

    [Header("Raggio d'azione")]
    public float raggioDiAzione = 3f;
    public float raggioDiChiusura = 4.5f;

    [Header("Eventi Speciali")]
    public UnityEvent EventoFineDialogo;

    private DialogueManager manager;
    private PlayerInputHandler inputPersonaggio;
    private bool playerVicino = false;
    private bool dialogoInCorso = false;
    private bool aspettaRilascioDopoFine = false;

    void Start()
    {
        manager = FindFirstObjectByType<DialogueManager>();
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();

        if (pannelloNuvoletta != null)
            pannelloNuvoletta.SetActive(false);
    }

    void Update()
    {
        if (ilTuoPersonaggio == null || manager == null) return;

        bool interactOra = inputPersonaggio != null && inputPersonaggio.InteractPressed;

        if (!interactOra)
            aspettaRilascioDopoFine = false;

        Vector3 diff = transform.position - ilTuoPersonaggio.transform.position;
        diff.y = 0;
        float distanza = diff.magnitude;
        float raggioAttuale = dialogoInCorso ? raggioDiChiusura : raggioDiAzione;

        if (distanza <= raggioAttuale)
        {
            playerVicino = true;

            if (interactOra)
            {
                if (dialogoInCorso)
                {
                    manager.AvanzaDialogo();
                }
                else if (!aspettaRilascioDopoFine && !manager.staParlando)
                {
                    manager.AvviaDialogo(this, dialogo, pannelloNuvoletta, testoNuvoletta, resizerNuvoletta, voceNPC);
                    dialogoInCorso = true;
                }
            }
        }
        else
        {
            if (playerVicino)
            {
                playerVicino = false;

                if (dialogoInCorso && manager.staParlando)
                {
                    if (manager.triggerCorrente == this)
                    {
                        
                        manager.TerminaDialogo();
                    }
                }

                dialogoInCorso = false;
                aspettaRilascioDopoFine = false;
            }
        }
    }

    public void OnDialogoFinito()
    {
        dialogoInCorso = false;
        aspettaRilascioDopoFine = true;
        EventoFineDialogo?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raggioDiAzione);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raggioDiChiusura);
    }
}