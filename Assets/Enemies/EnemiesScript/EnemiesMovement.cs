using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Aggiunto per utilizzare la NavMesh

public class EnemiesScript : MonoBehaviour
{
    [Header("Grafica")]
    public Transform graficaNemico;

    [Header("Settings Movimento")]
    public float MaxSpeed = 5f;
    public float SightRange = 10f;
    public float DetectionRange = 10f;
    public LayerMask PlayerMask;
    public LayerMask ObstacleMask;

    [Header("Settings Pattugliamento (NavMesh)")]
    public float RoamRadius = 10f; // Quanto lontano può vagare dal suo punto attuale
    public float RoamWaitTime = 2f; // Quanto tempo aspetta prima di scegliere una nuova destinazione

    [Header("Settings Intelligenza")]
    public float AttackRange = 1.5f; // Distanza a cui decide di attaccare
    [Tooltip("Spunta questa casella se il mostro ha un'animazione 'Attack'")]
    public bool hasAttackAnimation = false;

    private Rigidbody rb;
    private NavMeshAgent agent; // Riferimento al NavMeshAgent
    private GameObject Target;
    private float pauseEndTime = 0f;
    private float roamTimer = 0f;
    private Vector3 eyeOffset = new Vector3(0, 1.5f, 0);

    private Animator _animator;
    private Vector3 _scalaOriginale;
    private EnemiesCombat _combatScript; // Riferimento al "Braccio Armato"

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Quando si usa la NavMesh, è spesso meglio rendere il Rigidbody cinematico 
        // per evitare conflitti tra la fisica e il NavMeshAgent.
        rb.isKinematic = true;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = MaxSpeed;
        agent.updateRotation = false; // Disabilitiamo la rotazione automatica per mantenere il tuo sistema di Sprite Flip!

        _animator = GetComponentInChildren<Animator>();
        _combatScript = GetComponent<EnemiesCombat>();

        if (graficaNemico != null)
        {
            _scalaOriginale = graficaNemico.localScale;
        }

        roamTimer = RoamWaitTime; // Inizializza il timer
    }

    void FixedUpdate()
    {
        if (Time.time < pauseEndTime)
        {
            StopEnemyMovement();
            UpdateAnimator(false);
            return;
        }

        if (Target == null)
        {
            FindPlayer();

            // Se non trova il player, pattuglia
            if (Target == null)
            {
                PatrolRandomly();
            }
        }
        else
        {
            ChasePlayer();
        }

        // --- GESTIONE GRAFICA E ANIMAZIONI ---
        // Usiamo la velocità dell'agent per capire da che parte sta andando
        if (agent.velocity.magnitude > 0.1f && !agent.isStopped)
        {
            Vector3 velocityDir = agent.velocity.normalized;
            HandleSpriteFlip(velocityDir.x);
            UpdateAnimator(true, velocityDir.x, velocityDir.z);
        }
        else
        {
            UpdateAnimator(false);
        }
    }

    public void PauseMovement(float duration)
    {
        pauseEndTime = Time.time + duration;
        StopEnemyMovement();
    }

    void PatrolRandomly()
    {
        // AGGIUNTA: Se l'agente non è sulla NavMesh, non fare nulla per questo frame
        if (!agent.isOnNavMesh) return;

        // Controlla se il nemico ha raggiunto la sua destinazione attuale
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Si ferma
            agent.isStopped = true;

            // Inizia a contare il tempo di attesa
            roamTimer += Time.fixedDeltaTime;

            // Se il tempo di attesa è terminato, cerca un nuovo punto
            if (roamTimer >= RoamWaitTime)
            {
                Vector3 randomDirection = Random.insideUnitSphere * RoamRadius;
                randomDirection += transform.position;

                NavMeshHit hit;
                // Cerca il punto valido più vicino sulla NavMesh
                if (NavMesh.SamplePosition(randomDirection, out hit, RoamRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    agent.isStopped = false; // Fai ripartire l'agente
                }

                // Resetta il timer per la prossima sosta
                roamTimer = 0f;
            }
        }
        else
        {
            // Mentre cammina verso la destinazione, tienilo in movimento e mantieni il timer a 0
            agent.isStopped = false;
            roamTimer = 0f;
        }
    }

    void FindPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRange, PlayerMask);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && CheckLineOfSight(hitCollider.gameObject))
            {
                Target = hitCollider.gameObject;
                break;
            }
        }
    }

    void ChasePlayer()
    {
        if (Target == null || !Target.CompareTag("Player"))
        {
            StopEnemy();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        if (distanceToTarget > SightRange)
        {
            StopEnemy();
            return;
        }

        // --- DECISIONE DI ATTACCO (Basata sulla distanza) ---
        if (distanceToTarget <= AttackRange)
        {
            StopEnemyMovement();
            UpdateAnimator(false);

            if (_combatScript != null)
            {
                _combatScript.TryAttack(Target);
            }
            return;
        }
        // ---------------------------------------------------

        if (CheckLineOfSight(Target))
        {
            // AGGIUNTA: Controllo di sicurezza NavMesh
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(Target.transform.position);
            }
        }
        else
        {
            StopEnemy();
        }
    }

    public void PlayAttackAnimation()
    {
        if (hasAttackAnimation && _animator != null)
        {
            _animator.SetTrigger("Attack");
        }
    }

    bool CheckLineOfSight(GameObject targetObj)
    {
        Vector3 origin = transform.position + eyeOffset;
        Vector3 targetPos = targetObj.transform.position + eyeOffset;

        Vector3 direction = (targetPos - origin).normalized;
        float distance = Vector3.Distance(origin, targetPos);

        Vector3 startPoint = origin + (direction * 1.0f);
        float adjustedDistance = distance - 1.0f;
        if (adjustedDistance < 0) adjustedDistance = 0;

        RaycastHit hit;
        LayerMask maskToCheck = PlayerMask | ObstacleMask;

        if (Physics.Raycast(startPoint, direction, out hit, adjustedDistance, maskToCheck))
        {
            if (hit.collider.gameObject == targetObj)
                return true;
            else
                return false;
        }
        return true;
    }

    void HandleSpriteFlip(float directionX)
    {
        if (graficaNemico == null) return;

        if (directionX > 0.1f)
            graficaNemico.localScale = new Vector3(Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
        else if (directionX < -0.1f)
            graficaNemico.localScale = new Vector3(-Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
    }

    // Ferma solo il movimento (utile per attaccare o mettere in pausa)
    void StopEnemyMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    // Dimentica il bersaglio e si ferma
    void StopEnemy()
    {
        Target = null;
        StopEnemyMovement();
        UpdateAnimator(false);
    }

    void UpdateAnimator(bool isMoving, float dirX = 0f, float dirY = 0f)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", isMoving);
            if (isMoving)
            {
                _animator.SetFloat("MoveX", dirX);
                _animator.SetFloat("MoveY", dirY); // Attenzione: dirY qui è in realtà l'asse Z in 3D
            }
        }
    }

    public void Die()
    {
        StopEnemy();
        this.enabled = false;

        if (_combatScript != null) _combatScript.enabled = false;
        if (agent != null) agent.enabled = false; // Disabilita il NavMeshAgent per farlo cadere/fermare

        if (_animator != null) _animator.SetTrigger("IsDead");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.8f);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // Disegna anche l'area di pattugliamento in ciano
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, RoamRadius);

        if (Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + eyeOffset, Target.transform.position + eyeOffset);
        }
    }
}