using System.Collections;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;

    [Header("Animator")]
    public Animator animator;
    [Tooltip("Exact name of the disappear state")]
    public string teleportOutState = "Teleport_Out";
    [Tooltip("Exact name of the reappear state")]
    public string teleportInState = "Teleport_In";
    [Tooltip("Exact name of your default movement/idle state")]
    public string idleState = "Idle"; // <--- NEW VARIABLE

    [Header("Timing")]
    [Tooltip("How long until the cloud fully covers the character (e.g., 0.5)")]
    public float coverDelay = 0.5f;
    [Tooltip("How long the cloud stays visible after arrival before allowing movement")]
    public float revealDelay = 0.5f;

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

        // --- PHASE 1: DISAPPEAR ---
        if (animator != null)
        {
            animator.CrossFadeInFixedTime(teleportOutState, 0.1f, 0, 0f);
        }

        yield return new WaitForSeconds(coverDelay);

        // --- PHASE 2: PHYSICS BYPASS & MOVE ---
        if (characterController != null)
            characterController.enabled = false;

        transform.position = target.position;
        transform.rotation = target.rotation;

        if (characterController != null)
            characterController.enabled = true;

        yield return null;

        // --- PHASE 3: REAPPEAR ---
        if (animator != null)
        {
            animator.CrossFadeInFixedTime(teleportInState, 0.1f, 0, 0f);
        }

        // Wait for the exact duration of the reverse animation
        yield return new WaitForSeconds(revealDelay);

        // --- PHASE 4: RETURN TO IDLE ---
        if (animator != null)
        {
            // Force a smooth 0.1s transition back to your normal stance
            animator.CrossFadeInFixedTime(idleState, 0.1f);
        }

        isTeleporting = false;
    }
}