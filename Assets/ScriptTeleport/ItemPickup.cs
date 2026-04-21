using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportDestination;   // Assign the target location in Inspector
    public TeleportManager teleportManager; // Assign the TeleportManager in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide or destroy the pickup object immediately
            gameObject.SetActive(false);

            // Kick off the teleport sequence
            teleportManager.StartTeleport(other.gameObject, teleportDestination);
        }
    }
}
