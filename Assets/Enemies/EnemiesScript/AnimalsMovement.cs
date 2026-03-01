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
            Vector3 newPos = RandomNavSphere(_startPosition, wanderRadius, NavMesh.AllAreas);

            // Sicurezza: se newPos è diverso da Vector3.zero, significa che ha trovato un punto valido
            if (newPos != Vector3.zero)
            {
                _agent.SetDestination(newPos);
            }
            _timer = 0;
        }

        HandleSpriteFlip();
        UpdateAnimator();
    }

    void HandleSpriteFlip()
    {
        if (graficaAnimale == null) return;

        if (_agent.velocity.x > 0.1f)
        {
            graficaAnimale.localScale = new Vector3(-Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
        else if (_agent.velocity.x < -0.1f)
        {
            graficaAnimale.localScale = new Vector3(Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector2 randomPoint2D = Random.insideUnitCircle * distance;
        Vector3 randomDirection = new Vector3(randomPoint2D.x, 0, randomPoint2D.y);
        randomDirection += origin;

        NavMeshHit navHit;

        // CONTROLLO DI SICUREZZA: Controlliamo se la funzione ha successo
        if (NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask))
        {
            return navHit.position; // Punto valido trovato
        }

        // Se fallisce, restituiamo un Vector3.zero (così nel timer lo ignoriamo)
        return Vector3.zero;
    }

    void UpdateAnimator()
    {
        if (_animator == null) return;

        Vector3 velocity = _agent.velocity;
        Vector2 movementDir = new Vector2(velocity.x, velocity.z).normalized;

        if (velocity.magnitude > 0.1f)
        {
            _animator.SetFloat("MoveX", movementDir.x);
            _animator.SetFloat("MoveY", movementDir.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}
