using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Impostazioni Dialogo")]
    public DialogueData dialogo;

    [Header("Chi può far partire il dialogo?")]
    public GameObject ilTuoPersonaggio;

    [Header("Raggio d'azione")]
    [Tooltip("Distanza massima per poter parlare con l'NPC")]
    public float raggioDiAzione = 3f;

    private DialogueManager manager;
    private PlayerInputHandler inputPersonaggio;

    private bool playerVicino = false;

    void Start()
    {
        manager = FindFirstObjectByType<DialogueManager>();
        inputPersonaggio = FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {
        // Misura la distanza esatta tra l'NPC e il personaggio
        float distanza = Vector3.Distance(transform.position, ilTuoPersonaggio.transform.position);

        // Controlla se il personaggio è dentro il raggio d'azione
        if (distanza <= raggioDiAzione)
        {
            playerVicino = true;

            // Se siamo vicini, premiamo E e il Manager non sta già parlando
            if (inputPersonaggio != null && inputPersonaggio.InteractPressed)
            {
                if (manager != null && !manager.staParlando)
                {
                    manager.AvviaDialogo(dialogo);
                }
            }
        }
        else
        {
            // Se usciamo dal raggio d'azione
            if (playerVicino)
            {
                playerVicino = false;

                // Chiude il dialogo in automatico se ci allontaniamo
                if (manager != null && manager.staParlando)
                {
                    manager.TerminaDialogo();
                }
            }
        }
    }

    // BONUS VISIVO: Disegna una sfera gialla attorno all'NPC nella finestra Scene!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raggioDiAzione);
    }
}