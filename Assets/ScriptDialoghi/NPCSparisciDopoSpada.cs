using UnityEngine;

public class NPCSparisciDopoSpada : MonoBehaviour
{
    public GameObject npc;
    public DialogueTrigger triggerNPC;
    public DialogueData dialogoDopoSpada;

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
            npc.SetActive(false);
        }
    }
}