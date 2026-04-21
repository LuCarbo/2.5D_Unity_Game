using UnityEngine;

public class SnowTrigger : MonoBehaviour
{
    public ParticleSystem snowTempest;
    public Transform player;

    [Header("Mountain Elevation")]
    public float bottomOfMountainY = 0f;
    public float topOfMountainY = 100f;

    [Header("Snow Intensity (Speed)")]
    public float normalSpeed = 5f;
    public float blizzardSpeed = 25f;

    [Header("Snow Quantity (Emission)")]
    public float normalEmission = 100f;    // A light dusting at the bottom
    public float blizzardEmission = 2000f; // A total whiteout at the top

    private bool isPlayerInZone = false;

    private void Update()
    {
        if (isPlayerInZone)
        {
            // 1. Figure out how far up the mountain the player is (0.0 to 1.0)
            float heightPercent = Mathf.InverseLerp(bottomOfMountainY, topOfMountainY, player.position.y);

            // 2. Calculate the exact speed AND exact quantity based on that percentage
            float currentSpeed = Mathf.Lerp(normalSpeed, blizzardSpeed, heightPercent);
            float currentEmission = Mathf.Lerp(normalEmission, blizzardEmission, heightPercent);

            // 3. Apply the new speed to the Main module
            var mainModule = snowTempest.main;
            mainModule.startSpeed = currentSpeed;

            // 4. Apply the new quantity to the Emission module
            var emissionModule = snowTempest.emission;
            emissionModule.rateOverTime = currentEmission;
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