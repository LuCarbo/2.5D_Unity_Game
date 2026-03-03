using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Riferimenti")]
    private CharacterController _controller;
    private PlayerInputHandler _inputHandler; // Script degli input
    private Animator _animator;
    public Transform attackPoint;             // Un oggetto che fa da centro dell'attacco

    [Header("Sistema Combo")]
    [SerializeField] private int _maxComboSteps = 3; 
    [SerializeField] private float _comboResetTime = 0.8f;

    private int _comboStep = 0;
    private float _lastAttackTime = 0f;

    // Proprietà pubblica: PlayerMovement può leggerla, ma solo PlayerCombat può modificarla
    public bool IsAttacking { get; private set; } = false;

    [Header("Statistiche Attacco")]
    public int attackDamage = 1;
    public float attackRange = 1f;          // Raggio della sfera di attacco
    public LayerMask enemyLayers;           // Per colpire solo i nemici (ignora i muri)

    [Header("Cooldown")]
    public float attackRate = 2f;           // Quanti attacchi al secondo
    private float nextAttackTime = 0f;

    // Variabili per memorizzare la posizione
    private float attackDistance;
    private float defaultHeight;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (attackPoint != null)
        {
            // Calcoliamo quanto � distante l'attackPoint dal centro del giocatore (es. 0.8)
            attackDistance = Mathf.Abs(attackPoint.localPosition.x);

            if (attackDistance == 0) attackDistance = Mathf.Abs(attackPoint.localPosition.z);

            defaultHeight = attackPoint.localPosition.y;
        }
    }

    void Update()
    {
        // 1. GESTIONE DELLA DIREZIONE DELL'ATTACCO
        float moveX = inputHandler.MoveInput.x;
        float moveY = inputHandler.MoveInput.y; // Attenzione: la "Y" dell'input � "Su/Gi�" sulla tastiera

        // Controllo se il giocatore si sta muovendo
        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
        {
            // Capisco se sta andando pi� in orizzontale o pi� in verticale
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                // MOVIMENTO ORIZZONTALE (Destra/Sinistra sull'asse X)
                if (moveX > 0)
                {
                    attackPoint.localPosition = new Vector3(attackDistance, defaultHeight, 0); // Destra
                }
                else
                {
                    attackPoint.localPosition = new Vector3(-attackDistance, defaultHeight, 0); // Sinistra
                }
            }
            else
            {
                // MOVIMENTO VERTICALE
                if (moveY > 0)
                {
                    attackPoint.localPosition = new Vector3(0, defaultHeight, attackDistance); // Su (Allontana)
                }
                else
                {
                    attackPoint.localPosition = new Vector3(0, defaultHeight, -attackDistance); // Gi� (Avvicina)
                }
            }
        }

        // 2. LOGICA DI ATTACCO
        if (Time.time >= nextAttackTime)
        {
            if (inputHandler.AttackPressed)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void HandleAttacks()
    {
        // 1. Reset della combo se passa troppo tempo
        if (Time.time - _lastAttackTime > _comboResetTime && !IsAttacking)
        {
            _comboStep = 0;
            if (_animator != null) _animator.SetInteger("ComboStep", 0);
        }

        // 2. Logica di innesco attacco
        if (_input.AttackPressed && _controller.isGrounded && !IsAttacking)
        {
            _lastAttackTime = Time.time;
            _comboStep++;

            if (_comboStep > _maxComboSteps)
            {
                _comboStep = 1; 
            }

            StartCoroutine(PerformAttackSequence());
        }
    }

    private IEnumerator PerformAttackSequence()
    {
        IsAttacking = true;

        if (_animator != null)
        {
            _animator.SetInteger("ComboStep", _comboStep);
            _animator.SetTrigger("AttackTrigger");
        }

        // Finestra in cui non accetti nuovi input di attacco
        yield return new WaitForSeconds(0.35f); 

        IsAttacking = false;
    }

    // Ricordati di associare questo metodo agli Animation Events!
    public void OnAttackAnimationEnd()
    {
        IsAttacking = false;
    }

    public void OnPlayerHit()
    {
        if (_animator != null) _animator.SetTrigger("HitTrigger");
        
        IsAttacking = false;
        _comboStep = 0; // Se vieni colpito, perdi la combo!
    }

    void Attack()
    {
        // 1. (Opzionale in futuro) Fai partire l'animazione di attacco dell'Animator del Player qui

        // 2. Rileva tutti i nemici nel raggio d'azione
        // Crea una sfera invisibile partendo dall'attackPoint
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 3. Infliggi danno a ciascun nemico colpito
        foreach (Collider enemy in hitEnemies)
        {
            // Cerchiamo il nostro famoso script universale 'Health' sul nemico
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-attackDamage);
                Debug.Log("Colpito: " + enemy.name + " per " + attackDamage + " danni!");
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
