using UnityEngine;

public class PickupTeleportActivator : MonoBehaviour
{
    [Header("Teleporter Reference")]
    [Tooltip("Drag the GameObject containing your Teleporter here")]
    public GameObject teleporterToReveal;

    private void Start()
    {
        // Safety check: ensure the teleporter is hidden when the level starts
        if (teleporterToReveal != null)
        {
            teleporterToReveal.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the pickup is the Player
        if (other.CompareTag("Player"))
        {
            // 1. Make the teleporter appear
            if (teleporterToReveal != null)
            {
                teleporterToReveal.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Teleporter to reveal is not assigned in the inspector!");
            }

            // 2. (Optional) Play a sound or add particle effects here

            // 3. Destroy the picked-up object so it disappears
            Destroy(gameObject);
        }
    }
}