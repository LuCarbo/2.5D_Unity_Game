using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalsMovement : MonoBehaviour
{
    [Header("Impostazioni Area")]
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;

    [Header("Grafica 2.5D")]
    [Tooltip("Trascina qui l'oggetto figlio che contiene la grafica/sprite dell'animale")]
    public Transform graficaAnimale;
    private Animator _animator;

    private NavMeshAgent _agent;
    private float _timer;
    private Vector3 _startPosition;
    private Vector3 _scalaOriginale;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;

        // Recupera l'Animator dal figlio se non assegnato manualmente
        _animator = GetComponentInChildren<Animator>();

        _startPosition = transform.position;
        _timer = wanderTimer;

        if (graficaAnimale != null)
        {
            _scalaOriginale = graficaAnimale.localScale;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(_startPosition, wanderRadius, -1);
            _agent.SetDestination(newPos);
            _timer = 0;
        }

        // Chiamiamo la funzione che specchia lo sprite ogni frame
        HandleSpriteFlip();

        // Aggiorna i parametri dell'animazione in base alla velocità del NavMeshAgent
        UpdateAnimator();
    }

    // ===================================================================
    // LOGICA VISIVA (FLIP)
    // ===================================================================
    void HandleSpriteFlip()
    {
        if (graficaAnimale == null) return;

        if (_agent.velocity.x > 0.1f)
        {
            // L'ape va a Destra -> Siccome il disegno base guarda a sinistra, la SPECCHIAMO (segno Meno)
            graficaAnimale.localScale = new Vector3(-Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
        else if (_agent.velocity.x < -0.1f)
        {
            // L'ape va a Sinistra -> Usiamo il disegno normale così com'è (segno Più)
            graficaAnimale.localScale = new Vector3(Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
    }

    // ===================================================================
    // METODO PER TROVARE UN PUNTO CASUALE SUL NAVMESH
    // ===================================================================
    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector2 randomPoint2D = Random.insideUnitCircle * distance;
        Vector3 randomDirection = new Vector3(randomPoint2D.x, 0, randomPoint2D.y);
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    // ===================================================================
    // GESTIONE ANIMAZIONI (BLEND TREE)
    // ===================================================================
    void UpdateAnimator()
    {
        if (_animator == null) return;

        // Prendi la velocità dell'ape. In un mondo 3D il pavimento è su X e Z!
        Vector3 velocity = _agent.velocity;

        // Normalizziamo la velocità (la limitiamo tra -1 e 1)
        Vector2 movementDir = new Vector2(velocity.x, velocity.z).normalized;

        // Invia i valori ai parametri che abbiamo creato in Unity
        // Se l'ape si ferma, i valori andranno a 0 e si fermerà sull'ultimo frame (o su un'animazione Idle se la aggiungi)
        if (velocity.magnitude > 0.1f)
        {
            _animator.SetFloat("MoveX", movementDir.x);
            _animator.SetFloat("MoveY", movementDir.y); // Usiamo la Z del mondo 3D come Y per l'Animator 2D
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}
