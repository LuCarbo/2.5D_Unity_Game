using UnityEngine;

public class BossAnimationRelay : MonoBehaviour
{
    [Tooltip("Trascina qui il genitore (il Boss) che contiene lo script BossCombat")]
    public BossCombat parentCombatScript;

    private void Awake()
    {
        // Se dimentichi di trascinarlo nell'Inspector, Unity lo cerca automaticamente nel genitore!
        if (parentCombatScript == null)
        {
            parentCombatScript = GetComponentInParent<BossCombat>();
        }
    }

    // QUESTO è il metodo che chiamerai dall'Animation Event sulla timeline
    public void TriggerStrikeEvent()
    {
        if (parentCombatScript != null)
        {
            // Passa il comando allo script principale
            parentCombatScript.AnimationEvent_Strike();
        }
        else
        {
            Debug.LogError("Errore: BossCombat non trovato dal Relay!");
        }
    }
}