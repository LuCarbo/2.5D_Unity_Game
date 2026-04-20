using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Impostazioni Spawn")]
    [Tooltip("Trascina qui il PREFAB del nemico dalla cartella Project")]
    public GameObject enemyPrefab;

    private GameObject spawnedEnemy; // Il nemico fisicamente presente in scena

    void Start()
    {
        // Appena inizia il gioco, fa nascere il primo nemico
        SpawnEnemy();
    }

    // Questo metodo verrà chiamato dal Player quando muore
    public void ResetSpawner()
    {
        // 1. Se il nemico è ancora vivo (magari si è solo spostato), lo eliminiamo per fare pulizia
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
        }

        // 2. Creiamo un nemico nuovo e intatto al punto di partenza
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // Crea il nemico esattamente nella posizione e rotazione di questo Spawner
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        }
    }

    // Un piccolo aiuto visivo: disegna un cubo rosso semitrasparente nell'editor di Unity
    // così sai dove nasceranno i nemici anche se non c'è il modello 3D!
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawCube(transform.position + Vector3.up * 1f, new Vector3(1f, 2f, 1f));
    }
}
