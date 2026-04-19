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
            // 1. Trigger the normal (forward) animation to disappear
            animator.SetTrigger(teleportOutTrigger);
        }

        // Wait for the animation to cover the character
        yield return new WaitForSeconds(coverDelay);

        // --- PHYSICS BYPASS ---
        if (characterController != null)
            characterController.enabled = false;

        // Move the character instantly while they are hidden
        transform.position = target.position;
        transform.rotation = target.rotation;

        if (characterController != null)
            characterController.enabled = true;
        // ----------------------

        if (animator != null)
        {
            // 2. We have arrived! Trigger the reverse (-1 speed) animation to reappear
            animator.SetTrigger(teleportInTrigger);
        }

        // Wait for the reverse animation to finish before allowing another teleport
        yield return new WaitForSeconds(revealDelay);

        isTeleporting = false;
    }
}