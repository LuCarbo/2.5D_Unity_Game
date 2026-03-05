using UnityEngine;
using UnityEngine.Events; // Aggiunto per usare gli Eventi

public class DialogueTrigger : MonoBehaviour
{
    [Header("Impostazioni Dialogo")]
    public DialogueData dialogo;

    [Header("Chi può far partire il dialogo?")]
    public GameObject ilTuoPersonaggio;

    [Header("Raggio d'azione")]
    [Tooltip("Distanza massima per poter parlare con l'NPC")]
    public float raggioDiAzione = 3f;

    [Header("Eventi Speciali")]
    [Tooltip("Cosa succede quando il dialogo finisce normalmente?")]
    public UnityEvent EventoFineDialogo; // Lo slot dove metteremo la pozione!

    private DialogueManager manager;
    private PlayerInputHandler inputPersonaggio;

    private bool playerVicino = false;
    private bool dialogoInCorso = false; // Ci serve per capire se stavamo parlando

    void Start()
    {
        manager = FindFirstObjectByType<DialogueManager>();
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {
        float distanza = Vector3.Distance(transform.position, ilTuoPersonaggio.transform.position);

        if (distanza <= raggioDiAzione)
        {
            playerVicino = true;

            // 1. Avvio del Dialogo
            if (inputPersonaggio != null && inputPersonaggio.InteractPressed)
            {
                if (manager != null && !manager.staParlando)
                {
                    manager.AvviaDialogo(dialogo);
                    dialogoInCorso = true; // Segniamo che la conversazione è iniziata
                }
            }

            // 2. Controllo Fine Dialogo
            // Se c'era un dialogo in corso, ma il manager dice che non stiamo più parlando...
            if (dialogoInCorso && manager != null && !manager.staParlando)
            {
                dialogoInCorso = false; // Il dialogo è finito
                
                // Lancia l'evento di fine dialogo!
                if (EventoFineDialogo != null)
                {
                    EventoFineDialogo.Invoke();
                }
            }
        }
        else
        {
            if (playerVicino)
            {
                playerVicino = false;

                if (manager != null && manager.staParlando)
                {
                    manager.TerminaDialogo();
                    dialogoInCorso = false; // Resetta se ci allontaniamo bruscamente
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