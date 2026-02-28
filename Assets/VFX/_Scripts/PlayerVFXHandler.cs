using UnityEngine;

public class PlayerVFXHandler : MonoBehaviour
{
    [Header("Impostazioni Effetti")]
    [SerializeField] private GameObject walkDustPrefab; // Trascina qui il prefab polvere camminata
    [SerializeField] private GameObject jumpDustPrefab; // Trascina qui il prefab polvere salto
    [SerializeField] private Transform feetPosition;    // Un oggetto vuoto ai piedi del player

    [Header("Settings Atterraggio")]
    // Variabili per gestire l'atterraggio (se non le hai già nel movement script)
    private bool wasGrounded;
    private CharacterController characterController; // O Rigidbody2D, dipende cosa usi
    
    void Start()
    {
        // Se usi un punto specifico per i piedi, assicurati che sia assegnato
        if (feetPosition == null) feetPosition = transform;
    }

    // --- QUESTE FUNZIONI VENGONO CHIAMATE DAGLI ANIMATION EVENTS ---

    // Chiama questa funzione nell'evento dell'animazione CAMMINATA/CORSA
    public void SpawnFootDust()
    {
        // 1. Calcoliamo la rotazione PRIMA di creare la polvere
        Quaternion dustRotation = Quaternion.Euler(25,0,0); // Di base guarda a destra (0,0,0)

        // Se il player ha la scala X negativa (quindi è girato a sinistra)
        if (transform.localScale.x < 0)
        {
            // Ruota la polvere di 180 gradi sull'asse Y
            dustRotation = Quaternion.Euler(-25, 180, 0);
        }

        // 2. Istanziamento con la rotazione corretta
        if (walkDustPrefab != null)
        {
            Instantiate(walkDustPrefab, feetPosition.position, dustRotation);
        }
        else
        {
            Debug.LogWarning("Manca il prefab della polvere!");
        }
    }

    // Chiama questa funzione nell'evento dell'animazione SALTO (Start)
    public void SpawnJumpDust()
    {
        if (jumpDustPrefab != null)
        {
            RaycastHit hit;
            // Lancia un raggio da feetPosition verso il basso per max 1 metro (o adatta la distanza)
            if (Physics.Raycast(feetPosition.position, Vector3.down, out hit, 1.0f))
            {
                // Usa hit.point: è il punto esatto di contatto col terreno
                Instantiate(jumpDustPrefab, hit.point, Quaternion.Euler(25, 0, 0));
            }
            else
            {
                // Fallback: se sei molto in alto e il raggio non tocca nulla, usa la vecchia logica
                Instantiate(jumpDustPrefab, feetPosition.position, Quaternion.Euler(25, 0, 0));
            }
        }
    }

    // --- LOGICA PER L'ATTERRAGGIO (LANDING) ---
    
    // Esempio generico: dovresti integrare questo check nel tuo script di movimento
    // per evitare duplicati, ma ecco la logica base:
    public void CheckLandingDust(bool isGroundedNow)
    {
        // Se prima non eravamo a terra, e ora lo siamo -> ATTERRAGGIO
        if (!wasGrounded && isGroundedNow)
        {
            // Riutilizziamo la polvere del salto per l'impatto, o creane una specifica
            SpawnJumpDust(); 
        }

        wasGrounded = isGroundedNow;
    }
}