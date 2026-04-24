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

        // --- RIABILITA IL CONTROLLER E TUTTI I COMANDI CHE 'HEALTH' AVEVA SPENTO ---

        gameObject.tag = "Player";

        if (cc != null) cc.enabled = true;

        PlayerCombat combatScript = GetComponent<PlayerCombat>();
        if (combatScript != null) combatScript.enabled = true;

        PlayerInputHandler inputScript = GetComponent<PlayerInputHandler>();
        if (inputScript != null) inputScript.enabled = true;
        // -----------------------------------------------------------------------------

        // --- NASCONDI LA SCHERMATA DI MORTE ---
        DeathScreenManager deathScreen = Object.FindAnyObjectByType<DeathScreenManager>();
        if (deathScreen != null)
        {
            deathScreen.HideDeathScreenInstantly();
        }

        EnemySpawner[] tuttiGliSpawner = Object.FindObjectsByType<EnemySpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        // Passa in rassegna ogni singolo spawner e gli dice di resettarsi
        foreach (EnemySpawner spawner in tuttiGliSpawner)
        {
            spawner.ResetSpawner();
        }

        // Ripristina la vita
        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // Richiama la riattivazione del movimento
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

    // Metodo per aggiornare i checkpoint
    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Nuovo Checkpoint salvato in posizione: " + newPoint.position);
    }
}
