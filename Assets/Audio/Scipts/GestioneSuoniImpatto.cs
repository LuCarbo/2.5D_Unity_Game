using UnityEngine;

public class GestioneSuoniImpatto : MonoBehaviour
{
    public AudioClip suonoColpo; // Trascina qui il file audio del colpo dall'Inspector
    private AudioSource audioSource;

    void Start()
    {
        // Prende l'AudioSource attaccato a questo stesso oggetto
        audioSource = GetComponent<AudioSource>();
    }

    // Questa funzione scatta in automatico quando questo oggetto sbatte contro un altro
    void OnCollisionEnter(Collision collision)
    {
        // Opzionale: controlla COSA hai colpito usando i Tag
        if (collision.gameObject.CompareTag("Nemico") || collision.gameObject.CompareTag("OggettoDistruttibile"))
        {
            // Riproduce il suono una volta sola
            audioSource.PlayOneShot(suonoColpo);
        }
    }
}