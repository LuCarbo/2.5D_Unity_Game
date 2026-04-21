using UnityEngine;

public class SnowTrigger : MonoBehaviour
{
    public ParticleSystem snowTempest;
    public Transform player; // We need to track the player's exact position now

    [Header("Mountain Elevation")]
    public float bottomOfMountainY = 0f;  // The Y coordinate where the storm is calmest
    public float topOfMountainY = 100f;   // The Y coordinate where the storm is at its worst

    [Header("Snow Intensity")]
    public float normalSpeed = 5f;        // Speed at the bottom
    public float blizzardSpeed = 25f;     // Speed at the top

    private bool isPlayerInZone = false;

    private void Update()
    {
        // Only run this math if the player is actually inside the tempest zone
        if (isPlayerInZone)
        {
            // 1. Figure out how far up the mountain the player is (returns a percentage from 0.0 to 1.0)
            float heightPercent = Mathf.InverseLerp(bottomOfMountainY, topOfMountainY, player.position.y);

            // 2. Calculate the exact snow speed based on that percentage
            float currentSpeed = Mathf.Lerp(normalSpeed, blizzardSpeed, heightPercent);

            // 3. Apply the new speed to the particle system
            // (In Unity, you have to extract the 'main' module first to change its variables)
            var mainModule = snowTempest.main;
            mainModule.startSpeed = currentSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            snowTempest.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            snowTempest.Stop();
        }
    }
}