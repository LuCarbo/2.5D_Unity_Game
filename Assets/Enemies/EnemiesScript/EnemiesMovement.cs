using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesScript : MonoBehaviour
{
    // Il "Cervello" del nemico: sa sempre in che stato si trova
    public enum EnemyState { Wandering, Chasing, Attacking, Dead }
    public EnemyState currentState = EnemyState.Wandering;

    [Header("Grafica")]
    public Transform graficaNemico;

    [Header("Settings Movimento")]
    public float WanderSpeed = 2f;
    public float ChaseSpeed = 5f;

    [Header("Settings Visione & Intelligenza")]
    public float DetectionRange = 8f;   // Distanza per scoprirti (Cerchio Giallo)
    public float SightRange = 12f;      // Distanza a cui si arrende (Cerchio Rosso)
    public float AttackRange = 1.5f;    // Distanza di attacco (Cerchio Magenta)
    public LayerMask ObstacleMask;      // Layer dei muri (per non vedere attraverso)
    public bool hasAttackAnimation = false;

    [Header("Settings Pattugliamento (NavMesh)")]
    public float RoamRadius = 10f;
    public float RoamWaitTime = 4f;     // Cambia direzione ogni 4 secondi
    private float roamTimer;

    // Componenti interni
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Animator _animator;
    private EnemiesCombat _combatScript;
    public Transform playerTarget;

    private Vector3 _scalaOriginale;
    private float pauseEndTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disabilitato per permettere il nostro Sprite Flip

        _animator = GetComponentInChildren<Animator>();
        _combatScript = GetComponent<EnemiesCombat>();

        if (graficaNemico != null)
            _scalaOriginale = graficaNemico.localScale;

        roamTimer = RoamWaitTime; // Parte a 4 per cercare subito la prima destinazione

        // Troviamo il player in automatico all'avvio
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTarget = p.transform;
    }

    void Update()
    {
        // Se è morto o in pausa forzata (es. dopo aver attaccato), non fa nulla
        if (currentState == EnemyState.Dead) return;
        if (Time.time < pauseEndTime)
        {
            agent.isStopped = true;
            UpdateGraphics();
            return;
        }

        // 1. AGGIORNA LO STATO (Priorità assoluta al Player)
        UpdateBrainState();

        // 2. ESEGUE L'AZIONE IN BASE ALLO STATO
        switch (currentState)
        {
            case EnemyState.Wandering:
                Wander();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }

        // 3. AGGIORNA ANIMAZIONI E FLIP
        UpdateGraphics();
    }

    // --- IL CERVELLO DECISIONALE ---
    void UpdateBrainState()
    {
        // Trova il player in caso di Respawn
        if (playerTarget == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTarget = p.transform;
            else return;
        }

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // 1. Se sei a distanza di morso, attacca senza esitare!
        if (distance <= AttackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            // 2. IL FIX: Se NON sei più a distanza di attacco, ma il lupo stava attaccando,
            // deve smettere immediatamente e rimettersi a inseguirti!
            if (currentState == EnemyState.Attacking)
            {
                currentState = EnemyState.Chasing;
            }

            // 3. Gestione dell'Inseguimento e del Pattugliamento
            if (currentState == EnemyState.Chasing)
            {
                // Si arrende e torna a pattugliare SOLO se scappi fuori dal cerchio rosso
                if (distance > SightRange)
                {
                    currentState = EnemyState.Wandering;
                }
            }
            else // Se currentState == EnemyState.Wandering
            {
                // Inizia a cacciarti se entri nel cerchio giallo e ti vede
                if (distance <= DetectionRange && CheckLineOfSight())
                {
                    currentState = EnemyState.Chasing;
                }
            }
        }
    }

    // --- PATTUGLIAMENTO CASUALE SULLA NAVMESH ---
    void Wander()
    {
        agent.isStopped = false;
        agent.speed = WanderSpeed;
        roamTimer += Time.deltaTime;

        // Ogni 4 secondi cambia punto
        if (roamTimer >= RoamWaitTime)
        {
            Vector3 randomDir = Random.insideUnitSphere * RoamRadius;
            randomDir += transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, RoamRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
            roamTimer = 0f;
        }
    }

    // --- INSEGUIMENTO DEL GIOCATORE ---
    void Chase()
    {
        agent.isStopped = false;
        agent.speed = ChaseSpeed;

        // Aggiorna la destinazione solo se il player si sposta, per non far laggare la NavMesh
        if (Vector3.Distance(agent.destination, playerTarget.position) > 0.5f)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    // --- FASE DI ATTACCO ---
    void Attack()
    {
        agent.isStopped = true; // Si ferma per colpire

        if (_combatScript != null)
        {
            _combatScript.TryAttack(playerTarget.gameObject);
        }
    }

    // --- CONTROLLO VISIVO SEMPLIFICATO ---
    bool CheckLineOfSight()
    {
        // IL FIX DELLA MIRA: Alziamo a 1 metro da terra SIA gli occhi del lupo SIA il petto del player
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 targetPos = playerTarget.position + Vector3.up * 1.0f;

        Vector3 direction = (targetPos - origin).normalized;
        float distance = Vector3.Distance(origin, targetPos);

        // Disegna un laser rosso nella schermata "Scene" di Unity!
        // Così se il lupo non ti rincorre, puoi vedere esattamente contro cosa sbatte il laser.
        Debug.DrawRay(origin, direction * distance, Color.red);

        // Spara un raggio. Se colpisce l'ObstacleMask (i muri), significa che non ti vede.
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ObstacleMask))
        {
            return false; // Ha colpito un ostacolo
        }

        return true; // Visuale libera!
    }

    // --- ANIMAZIONI E GESTIONE SPRITE ---
    void UpdateGraphics()
    {
        bool isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f;

        if (_animator != null)
        {
            _animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                Vector3 velocityDir = agent.velocity.normalized;
                _animator.SetFloat("MoveX", velocityDir.x);
                _animator.SetFloat("MoveY", velocityDir.z); // Asse Z mappato su Y per il 2D top-down

                // Sprite Flip
                if (graficaNemico != null)
                {
                    if (velocityDir.x > 0.1f)
                        graficaNemico.localScale = new Vector3(Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
                    else if (velocityDir.x < -0.1f)
                        graficaNemico.localScale = new Vector3(-Mathf.Abs(_scalaOriginale.x), _scalaOriginale.y, _scalaOriginale.z);
                }
            }
        }
    }

    // --- METODI RICHIAMATI DA ALTRI SCRIPT ---
    public void PauseMovement(float duration)
    {
        pauseEndTime = Time.time + duration;
        if (agent.isOnNavMesh) agent.isStopped = true;
    }

    public void PlayAttackAnimation()
    {
        if (hasAttackAnimation && _animator != null)
        {
            _animator.SetTrigger("Attack");
        }
    }

    public void Die()
    {
        currentState = EnemyState.Dead;
        this.enabled = false;

        if (_combatScript != null) _combatScript.enabled = false;
        if (agent != null) agent.enabled = false;

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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, RoamRadius);
    }
}