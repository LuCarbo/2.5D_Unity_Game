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
    // Flag per sapere se il dialogo e' davvero terminato e non solo tra una frase e l'altra
    private bool dialogoTerminato = false;

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

        float distanza = Vector3.Distance(transform.position, ilTuoPersonaggio.transform.position);
        float raggioAttuale = dialogoInCorso ? raggioDiChiusura : raggioDiAzione;

        if (distanza <= raggioAttuale)
        {
            playerVicino = true;

            // Avvia il dialogo solo se non sta gia' parlando con qualcuno
            if (inputPersonaggio != null && inputPersonaggio.InteractPressed)
            {
                if (!manager.staParlando && !dialogoInCorso)
                {
                    manager.AvviaDialogo(dialogo, pannelloNuvoletta, testoNuvoletta, resizerNuvoletta, voceNPC);
                    dialogoInCorso = true;
                    dialogoTerminato = false;
                }
            }

            // Controlla se il dialogo e' finito SOLO se stava parlando
            // e ora manager.staParlando e' false
            if (dialogoInCorso && !manager.staParlando && !dialogoTerminato)
            {
                dialogoTerminato = true;
                dialogoInCorso = false;
                EventoFineDialogo?.Invoke();
            }

            // Reset del flag cosi' si puo' riparlare
            if (!dialogoInCorso && dialogoTerminato && !inputPersonaggio.InteractPressed)
            {
                dialogoTerminato = false;
            }
        }
        else
        {
            if (playerVicino)
            {
                playerVicino = false;

                if (dialogoInCorso && manager.staParlando)
                {
                    manager.TerminaDialogo();
                }

                dialogoInCorso = false;
                dialogoTerminato = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raggioDiAzione);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raggioDiChiusura);
    }
}