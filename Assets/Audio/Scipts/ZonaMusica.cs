using UnityEngine;

public class ZonaMusica : MonoBehaviour
{
    [Tooltip("La canzone che deve suonare quando entri qui")]
    public AudioClip canzoneZona;

    // Questa funzione scatta IN AUTOMATICO quando qualcuno entra nel cubo invisibile (Fisica 3D)
    private void OnTriggerEnter(Collider other)
    {
        // Controlliamo che ad entrare sia il Player e non un NPC o un nemico a caso
        if (other.CompareTag("Player"))
        {
            if (GestoreMusica.istanza != null && canzoneZona != null)
            {
                // Chiamiamo il direttore d'orchestra!
                GestoreMusica.istanza.CambiaCanzone(canzoneZona);
            }
        }
    }
}