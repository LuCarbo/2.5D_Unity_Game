using UnityEngine;

public class NPCSparisciDopoSpada : MonoBehaviour
{
    public GameObject npc;
    public DialogueTrigger triggerNPC;
    public DialogueData dialogoDopoSpada;
    public Animator animatorNPC;
    public NPCVaiVia npcVaiVia;
    public float tempoDisattivazione = 3f;

    private bool spadaPresa = false;

    public void SpadaRaccolta()
    {
        spadaPresa = true;

        if (triggerNPC != null && dialogoDopoSpada != null)
        {
            triggerNPC.dialogo = dialogoDopoSpada;
        }
    }

    public void ControllaSeDeveSparire()
    {
        if (spadaPresa && npc != null)
        {
            // Disattiva il dialogo
            if (triggerNPC != null)
                triggerNPC.enabled = false;

            // Disattiva il collider
            BoxCollider col = npc.GetComponent<BoxCollider>();
            if (col != null) col.enabled = false;

            // Avvia animazione
            if (animatorNPC != null)
                animatorNPC.SetTrigger("VaiVia");

            // Avvia camminata
            if (npcVaiVia != null)
                npcVaiVia.Cammina();

            Destroy(npc, tempoDisattivazione);
        }
    }
}