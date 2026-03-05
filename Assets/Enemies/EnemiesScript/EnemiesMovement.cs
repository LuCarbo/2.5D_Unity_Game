using UnityEngine;

public class EnemiesScript : MonoBehaviour
{
    [Header("Grafica")]
    public Transform graficaNemico; // Qui ci trascino il figlio

    [Header("Settings")]
    public float MaxSpeed = 5f;
    public float SightRange = 10f;      // Quanto lontano vede il raycast
    public float DetectionRange = 10f;  // Raggio della sfera di ricerca
    public LayerMask PlayerMask;        // Layer esclusivo per il Player
    public LayerMask ObstacleMask;      // Layer per i muri/ostacoli

    private float Speed;
    private Rigidbody rb;
    private GameObject Target;
    private bool seePlayer;
    private float pauseEndTime = 0f;
    private Vector3 eyeOffset = new Vector3(0, 1.5f, 0); // Offset per alzare il punto di vista (dagli occhi, non dai piedi)

    
    private Animator _animator;       // Variabili per l'animazione
    private Vector3 _scalaOriginale;  // e la grafica

    void Start()
    {
        Speed = MaxSpeed;
        rb = GetComponent<Rigidbody>();

        // SICUREZZA: Assicuriamoci che il Rigidbody non cada o ruoti male
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        // freeziamo comunque la fisica per evitare che collisioni con muri lo facciano ruotare.

        _animator = GetComponentInChildren<Animator>();

        if (graficaNemico != null)
        {
            _scalaOriginale = graficaNemico.localScale;
        }
    }

    void FixedUpdate()
    {
        // Se il tempo di gioco attuale è minore del tempo in cui finisce la pausa, esci e non muoverti!
        if (Time.time < pauseEndTime)
        {
            UpdateAnimator(false);
            return;
        }

        // Fine pausa: Rimuoviamo i vincoli di movimento (tranne quelli per evitare rotazione e caduta)
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        // Logica di movimento regolare
        if (Target == null)
        {
            FindPlayer();
            UpdateAnimator(false); // È fermo e cerca
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRange, PlayerMask);

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
        // Se il target è nullo, distrutto, o se non ha più il tag "Player"
        if (Target == null || !Target.CompareTag("Player"))
        {
            StopEnemy(); // Usa la tua funzione per fermarlo e resettare le variabili
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

            // --- ROTAZIONE (Solo Destra/Sinistra) ---
            HandleSpriteFlip(direction.x);

            // Passa 'true' perché si muove, passa la X e la Z per il Blend Tree
            UpdateAnimator(true, direction.x, direction.z);
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

        // Il raggio sbatte solo contro Player e Muri (ignora altri nemici o oggetti a terra)
        LayerMask maskToCheck = PlayerMask | ObstacleMask;  

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
        return true;
    }

    // Gestisce la rotazione secca (Flip) usando la scala originale
    void HandleSpriteFlip(float directionX)
    {
        if (graficaNemico == null) return;

        if (directionX > 0.1f)
        {
            // Guarda a Destra (Mantiene la scala originale intatta)
            graficaNemico.localScale = new Vector3(Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
        else if (directionX < -0.1f)
        {
            // Guarda a Sinistra (Rende la X negativa per fare da specchio)
            graficaNemico.localScale = new Vector3(-Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
    }

    void StopEnemy()
    {
        seePlayer = false;
        Target = null; // Perde il target se non lo vede
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        UpdateAnimator(false); // Si ferma
    }

    void UpdateAnimator(bool isMoving, float dirX = 0f, float dirY = 0f)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                _animator.SetFloat("MoveX", dirX);
                _animator.SetFloat("MoveY", dirY); // Usiamo la Z del mondo come Y del BlendTree
            }
        }
    }

    public void Die()
    {
        // 1. Ferma il nemico istantaneamente
        StopEnemy();

        // Disabilita questo script così smette di cercare o inseguire il player
        this.enabled = false;

        // 2. Fai partire l'animazione di morte
        if (_animator != null)
        {
            _animator.SetTrigger("Die"); // Usa lo stesso identico nome messo nell'Animator!
        }

        // 3. Disabilita il Collider per non far sbattere il Player contro il cadavere
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 4. Distruggi l'oggetto definitivamente dopo x tempo
        Destroy(gameObject, 0.7f);
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