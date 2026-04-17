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

            if (inputPersonaggio != null && inputPersonaggio.InteractPressed)
            {
                if (!manager.staParlando)
                {
                    manager.AvviaDialogo(dialogo, pannelloNuvoletta, testoNuvoletta, resizerNuvoletta, voceNPC);
                    dialogoInCorso = true;
                }
            }

            if (dialogoInCorso && !manager.staParlando)
            {
                dialogoInCorso = false;
                EventoFineDialogo?.Invoke();
            }
        }
        else
        {
            if (playerVicino)
            {
                playerVicino = false;

                if (manager.staParlando && dialogoInCorso)
                {
                    manager.TerminaDialogo();
                    dialogoInCorso = false;
                }
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