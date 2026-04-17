using System.Collections;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;

    [Header("Animator")]
    public Animator animator;
    [Tooltip("Trigger parameter to start disappearing")]
    public string teleportOutTrigger = "TeleportOut";
    [Tooltip("Trigger parameter to start reappearing")]
    public string teleportInTrigger = "TeleportIn";

    [Header("Timing")]
    [Tooltip("How long until the cloud fully covers the character")]
    public float coverDelay = 0.4f;
    [Tooltip("How long the cloud stays visible after arrival before the script finishes")]
    public float revealDelay = 0.4f;

    private bool isTeleporting = false;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void TeleportToTarget()
    {
        TeleportTo(teleportTarget);
    }

    public void Teleport()
    {
        if (!isTeleporting && teleportTarget != null)
            StartCoroutine(TeleportRoutine(teleportTarget));
    }

    public void TeleportTo(Transform target)
    {
        if (!isTeleporting && target != null)
            StartCoroutine(TeleportRoutine(target));
    }

    private IEnumerator TeleportRoutine(Transform target)
    {
        isTeleporting = true;

        if (animator != null)
        {
            // Force the animator to instantly play the Out state
            // Make sure "Teleport_Out" matches the exact name of the STATE in the Animator (the box itself)
            animator.Play("Teleport_Out");
        }

        yield return new WaitForSeconds(coverDelay);

        if (characterController != null)
            characterController.enabled = false;

        transform.position = target.position;
        transform.rotation = target.rotation;

        if (characterController != null)
            characterController.enabled = true;

        if (animator != null)
        {
            // Force the animator to instantly play the In state
            // Make sure "Teleport_In" matches the exact name of the STATE in the Animator
            animator.Play("Teleport_In");
        }

        yield return new WaitForSeconds(revealDelay);

        isTeleporting = false;
    }
}