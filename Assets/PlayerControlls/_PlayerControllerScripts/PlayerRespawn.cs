using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Impostazioni Respawn")]
    [Tooltip("L'oggetto vuoto che fa da punto di rinascita")]
    public Transform respawnPoint;

    public void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogError("ERRORE: Non hai assegnato il RespawnPoint nell'Inspector!");
            return;
        }

        Debug.Log("Eseguo il teletrasporto...");

        // Disabilita temporaneamente per evitare attriti fisici
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Sposta il player
        transform.position = respawnPoint.position;

        // Forza Unity a registrare la nuova posizione istantaneamente
        Physics.SyncTransforms();

        // Riabilita il controller
        if (cc != null) cc.enabled = true;

        // Ripristina la vita
        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // Richiama la riattivazione
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.Revive();
        }
        else
        {
            Debug.LogError("ERRORE: PlayerMovement non trovato sul Player!");
        }
    }

    // Metodo per aggiornare i ceckpoint
    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Nuovo Checkpoint salvato in posizione: " + newPoint.position);
    }
}
