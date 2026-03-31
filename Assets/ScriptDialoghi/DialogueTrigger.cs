using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Impostazioni Dialogo")]
    public DialogueData dialogo;

    [Header("Riferimenti Nuvoletta (World Space)")]
    [Tooltip("Trascina qui il Panel posizionato sopra l'NPC")]
    public GameObject pannelloNuvoletta;
    [Tooltip("Trascina qui il TextMeshProUGUI dentro la nuvoletta")]
    public TextMeshProUGUI testoNuvoletta;
    [Tooltip("Trascina qui il componente DialoguePanelResizer sul Panel")]
    public DialoguePanelResizer resizerNuvoletta;

    [Header("Chi può far partire il dialogo?")]
    public GameObject ilTuoPersonaggio;

    [Header("Raggio d'azione")]
    public float raggioDiAzione = 3f;

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
        float distanza = Vector3.Distance(transform.position, ilTuoPersonaggio.transform.position);

        if (distanza <= raggioDiAzione)
        {
            playerVicino = true;

            if (inputPersonaggio != null && inputPersonaggio.InteractPressed)
            {
                if (manager != null && !manager.staParlando)
                {
                    manager.AvviaDialogo(dialogo, pannelloNuvoletta, testoNuvoletta, resizerNuvoletta);
                    dialogoInCorso = true;
                }
            }

            if (dialogoInCorso && manager != null && !manager.staParlando)
            {
                dialogoInCorso = false;
                if (EventoFineDialogo != null)
                    EventoFineDialogo.Invoke();
            }
        }
        else
        {
            if (playerVicino)
            {
                playerVicino = false;

                if (manager != null && manager.staParlando && dialogoInCorso)
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
    }
}