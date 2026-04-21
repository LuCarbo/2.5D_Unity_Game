using System.Collections;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [Header("Animator Trigger Names")]
    public string teleportOutTrigger = "TeleportOut";
    public string teleportInTrigger = "TeleportIn";

    [Header("Animator State Names")]
    public string teleportOutStateName = "TeleportOut";

    public void StartTeleport(GameObject player, Transform destination)
    {
        Debug.Log("<color=cyan>1. Starting Teleport Sequence</color>");
        StartCoroutine(TeleportSequence(player, destination));
    }

    private IEnumerator TeleportSequence(GameObject player, Transform destination)
    {
        Animator animator = player.GetComponent<Animator>();
        if (animator == null) yield break;

        animator.ResetTrigger(teleportOutTrigger);
        animator.ResetTrigger(teleportInTrigger);

        Debug.Log("<color=cyan>2. Firing TeleportOut Trigger</color>");
        animator.SetTrigger(teleportOutTrigger);

        // Wait two frames to ensure the Animator actually switches states
        yield return null;
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = 0.5f; // Default fallback time

        if (stateInfo.IsName(teleportOutStateName))
        {
            // Calculate 95% of the animation's actual length
            waitTime = stateInfo.length * 0.95f;
            Debug.Log($"<color=yellow>3. Found State! Waiting exactly {waitTime} seconds for animation to finish.</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>3. WARNING: Did not find state '{teleportOutStateName}'. Using default 0.5s wait.</color>");
        }

        // Wait the calculated time
        yield return new WaitForSeconds(waitTime);

        Debug.Log("<color=green>4. Moving Player Instantly</color>");
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.SetPositionAndRotation(destination.position, destination.rotation);
            Physics.SyncTransforms();
            cc.enabled = true;
        }
        else
        {
            player.transform.SetPositionAndRotation(destination.position, destination.rotation);
        }

        Debug.Log("<color=cyan>5. Firing TeleportIn Trigger NOW</color>");
        animator.SetTrigger(teleportInTrigger);
    }
}