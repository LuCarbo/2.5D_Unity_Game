using System.Collections;
using UnityEngine;

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

    [Header("Settings Intelligenza")]
    public float AttackRange = 1.5f; // Distanza a cui decide di attaccare
    [Tooltip("Spunta questa casella se il mostro ha un'animazione 'Attack'")]
    public bool hasAttackAnimation = false;

    private float Speed;
    private Rigidbody rb;
    private GameObject Target;
    private float pauseEndTime = 0f;
    private Vector3 eyeOffset = new Vector3(0, 1.5f, 0);

    private Animator _animator;
    private Vector3 _scalaOriginale;
    private EnemiesCombat _combatScript; // Riferimento al "Braccio Armato"

    void Start()
    {
        Speed = MaxSpeed;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        _animator = GetComponentInChildren<Animator>();
        _combatScript = GetComponent<EnemiesCombat>(); // Trova lo script dei danni

        if (graficaNemico != null)
        {
            _scalaOriginale = graficaNemico.localScale;
        }
    }

    void FixedUpdate()
    {
        if (Time.time < pauseEndTime)
        {
            UpdateAnimator(false);
            return;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        if (Target == null)
        {
            FindPlayer();
            UpdateAnimator(false);
        }
        else
        {
            ChasePlayer();
        }
    }

    public void PauseMovement(float duration)
    {
        pauseEndTime = Time.time + duration;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
            // FIX: Fermiamo le gambe e l'animazione, MA SENZA usare StopEnemy()
            // così il lupo non imposta "Target = null" e non si dimentica chi sta attaccando!
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            UpdateAnimator(false);

            if (_combatScript != null)
            {
                // Ordina allo script di combattimento di colpire
                _combatScript.TryAttack(Target);
            }
            return; // Esce dalla funzione per non camminare
        }
        // ---------------------------------------------------

        if (CheckLineOfSight(Target))
        {
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            Vector3 move = new Vector3(direction.x * Speed, rb.linearVelocity.y, direction.z * Speed);
            rb.linearVelocity = move;

            HandleSpriteFlip(direction.x);
            UpdateAnimator(true, direction.x, direction.z);
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
        Vector3 eyeHeight = new Vector3(0, 1.5f, 0);
        Vector3 origin = transform.position + eyeHeight;
        Vector3 targetPos = targetObj.transform.position + eyeHeight;

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

    void StopEnemy()
    {
        Target = null;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
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
                _animator.SetFloat("MoveY", dirY);
            }
        }
    }

    public void Die()
    {
        StopEnemy();
        this.enabled = false;

        // Spegniamo anche il combattimento, così non morde mentre cade a terra!
        if (_combatScript != null) _combatScript.enabled = false;

        if (_animator != null) _animator.SetTrigger("IsDead");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.8f);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

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

        // Disegna anche l'Attack Range in magenta per regolarlo facilmente dall'Inspector!
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        if (Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + eyeOffset, Target.transform.position + eyeOffset);
        }
    }
}