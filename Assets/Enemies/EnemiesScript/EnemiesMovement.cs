using UnityEngine;

public class EnemiesScript : MonoBehaviour
{
    [Header("Grafica")]
    public Transform graficaSlime; // Qui ci trascino il figlio
    [Header("Settings")]
    public float MaxSpeed = 5f;
    public float SightRange = 10f;      // Quanto lontano vede il raycast
    public float DetectionRange = 10f;  // Raggio della sfera di ricerca
    public LayerMask ObstacleMask;      // Layer per i muri (per evitare che il raycast colpisca oggetti a caso)

    private float Speed;
    private Rigidbody rb;
    private GameObject Target;
    private bool seePlayer;
    private float pauseEndTime = 0f;

    // Offset per alzare il punto di vista (dagli occhi, non dai piedi)
    private Vector3 eyeOffset = new Vector3(0, 1.5f, 0);

    void Start()
    {
        Speed = MaxSpeed;
        rb = GetComponent<Rigidbody>();

        // SICUREZZA: Assicuriamoci che il Rigidbody non cada o ruoti male
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        // freeziamo comunque la fisica per evitare che collisioni con muri lo facciano ruotare.
    }

    void FixedUpdate()
    {
        // Se il tempo di gioco attuale è minore del tempo in cui finisce la pausa, esci e non muoverti!
        if (Time.time < pauseEndTime) return;

        // Fine pausa: Rimuoviamo i vincoli di movimento (tranne quelli per evitare rotazione e caduta)
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        // Logica di movimento regolare
        if (Target == null)
        {
            FindPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    // Mette in pausa il movimento e azzera la velocità
    public void PauseMovement(float duration) {
        pauseEndTime = Time.time + duration;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Lo frena di colpo (mantenendo la gravità)
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

        void FindPlayer()
    {
        // Fase di ricerca: cerchiamo il player entro un certo raggio
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Controllo preliminare Line of Sight prima di agganciare
                if (CheckLineOfSight(hitCollider.gameObject))
                {
                    Target = hitCollider.gameObject;
                    seePlayer = true;
                }
                break;
            }
        }
    }

    void ChasePlayer()
    {
        // Se il target è distrutto o nullo, resetta
        if (Target == null)
        {
            seePlayer = false;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        // Se il player scappa troppo lontano oltre il raggio visivo
        if (distanceToTarget > SightRange)
        {
            StopEnemy();
            return;
        }

        if (CheckLineOfSight(Target))
        {
            // --- MOVIMENTO ---
            Vector3 direction = (Target.transform.position - transform.position).normalized;

            // Mantieni la velocità Y originale per la gravità
            Vector3 move = new Vector3(direction.x * Speed, rb.linearVelocity.y, direction.z * Speed);
            rb.linearVelocity = move;

            // --- ROTAZIONE 2.5D (Solo Destra/Sinistra) ---
            HandleSpriteFlip(direction.x);
        }
        else
        {
            // Muro in mezzo
            StopEnemy();
        }
    }

    // Funzione dedicata per controllare se vedo il player (evita muri)
    bool CheckLineOfSight(GameObject targetObj)
    {
        // 1. ALZIAMO LO SGUARDO:
        // Invece di partire dai piedi, partiamo da 1.5 unità in altezza (altezza occhi)
        Vector3 eyeHeight = new Vector3(0, 1.5f, 0);

        Vector3 origin = transform.position + eyeHeight;
        Vector3 targetPos = targetObj.transform.position + eyeHeight;

        Vector3 direction = (targetPos - origin).normalized;
        float distance = Vector3.Distance(origin, targetPos);

        // 2. EVITIAMO IL NEMICO STESSO (Self-Hit):
        // Spostiamo l'origine del raggio un po' in avanti nella direzione in cui guarda,
        // così esce fuori dal proprio Collider (es. 1 metro avanti).
        Vector3 startPoint = origin + (direction * 1.0f);

        // Calcoliamo la nuova distanza (perché ci siamo avvicinati partendo più avanti)
        float adjustedDistance = distance - 1.0f;
        if (adjustedDistance < 0) adjustedDistance = 0;

        RaycastHit hit;

        // DEBUG: Disegna il raggio nella scena così vedi cosa sta colpendo
        Debug.DrawRay(startPoint, direction * adjustedDistance, Color.cyan);

        // Lanciamo il raggio
        if (Physics.Raycast(startPoint, direction, out hit, adjustedDistance))
        {
            if (hit.collider.gameObject == targetObj)
            {
                return true; // Vedo il player libero da ostacoli
            }
            else
            {
                // Sto colpendo qualcos'altro (un muro?)
                // Debug per capire cosa blocca la vista
                // Debug.Log("Vista bloccata da: " + hit.collider.name); 
                return false;
            }
        }

        // Se il raycast non colpisce nulla (nemmeno il player),
        // ma il player è nel raggio teorico, consideriamolo visto se non ci sono ostacoli
        // Oppure il raggio era troppo corto.
        // In questo caso specifico, assumiamo che se il raycast non colpisce nulla fino al player, 
        // allora la via è libera (ma Raycast dovrebbe colpire il player se il layer è giusto).

        // ATTENZIONE: Se il raycast non colpisce nulla, potrebbe significare che il player 
        // non ha un collider o è su un layer ignorato.
        // Assumiamo true se il raycast arriva a destinazione senza hit (caso raro se miriamo al player)
        // Ma per sicurezza, nel 99% dei casi il raycast DEVE colpire il player.
        return true;
    }

    // Gestisce la rotazione secca (Flip) invece di LookAt
    void HandleSpriteFlip(float directionX)
    {
        if (graficaSlime == null) return;

        // Oso la Scale per "specchiare" il nemico.
        if (directionX > 0.1f)
        {
            // Guarda a Destra (Scala normale)
            graficaSlime.localScale = new Vector3(1, 1, 1);
        }
        else if (directionX < -0.1f)
        {
            // Guarda a Sinistra (Specchiato orizzontalmente sull'asse X)
            graficaSlime.localScale = new Vector3(-1, 1, 1);
        }
    }

    void StopEnemy()
    {
        seePlayer = false;
        Target = null; // Perde il target se non lo vede
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        // Disegna il raggio visivo per debug
        if (Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + eyeOffset, Target.transform.position + eyeOffset);
        }
    }
}