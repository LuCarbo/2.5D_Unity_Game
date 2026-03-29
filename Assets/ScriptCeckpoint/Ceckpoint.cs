using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Impostazioni Checkpoint")]
    [Tooltip("Punto esatto in cui il player rinascerà (può essere l'oggetto stesso)")]
    public Transform spawnLocation;

    // Variabile per evitare di riattivare lo stesso checkpoint mille volte
    private bool isActivated = false;

    void Awake()
    {
        // Se non assegni un punto specifico nell'Inspector, usa la posizione di questo oggetto
        if (spawnLocation == null)
        {
            spawnLocation = this.transform;
        }
    }

    // Questo metodo scatta in automatico quando qualcosa entra nell'area Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Controlliamo se chi è entrato ha il tag "Player" e se il checkpoint non è già attivo
        if (other.CompareTag("Player") && !isActivated)
        {
            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();

            if (playerRespawn != null)
            {
                // Diciamo al player di aggiornare il suo punto di respawn
                playerRespawn.SetRespawnPoint(spawnLocation);
                isActivated = true; // Segnamo il checkpoint come preso

                Debug.Log("Checkpoint Raggiunto!");

                // QUI POTRAI AGGIUNGERE EFFETTI VISIVI O SONORI
                // Esempio: GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
