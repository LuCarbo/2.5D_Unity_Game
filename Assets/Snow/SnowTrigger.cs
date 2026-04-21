using UnityEngine;

public class SnowTrigger : MonoBehaviour
{
    // Drag your SnowTempest particle system into this slot in the Inspector
    public ParticleSystem snowTempest;

    // This runs when something enters the Box Collider
    private void OnTriggerEnter(Collider other)
    {
        // This will print a message to your Console window
        Debug.Log("Something entered the zone: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("The Player was detected! Playing snow.");
            snowTempest.Play();
        }
    }

    // This runs when something leaves the Box Collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            snowTempest.Stop();
        }
    }
}