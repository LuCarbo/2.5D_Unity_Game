using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalsMovement : MonoBehaviour
{
    [Header("Impostazioni Area")]
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;

    [Header("Morte")]
    public float tempoDiDistruzione = 1f;
    private bool _isDead = false;

    [Header("Grafica")]
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

        if (_isDead) return;

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

    public void Die()
    {
        // Se è già morta, ignora (evita di chiamarlo due volte)
        if (_isDead) return;

        _isDead = true;

        // 1. Ferma fisicamente il NavMeshAgent
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        // 2. Fai partire l'animazione di morte
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }

        // 3. Disabilita il Collider così il Player non ci sbatte più contro
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 5. Distruggi l'oggetto dopo il tempo stabilito
        Destroy(gameObject, tempoDiDistruzione);
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
