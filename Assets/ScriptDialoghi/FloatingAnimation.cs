using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    [Header("Impostazioni")]
    public float ampiezza = 0.05f;  // quanto si sposta su e giù
    public float velocita = 2f;      // quanto veloce oscilla

    private Vector3 posizioneIniziale;

    void OnEnable()
    {
        // Salva la posizione ogni volta che il canvas si attiva
        posizioneIniziale = transform.position;
    }

    void Update()
    {
        transform.position = posizioneIniziale + new Vector3(0f, Mathf.Sin(Time.time * velocita) * ampiezza, 0f);
    }
}