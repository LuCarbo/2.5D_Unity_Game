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
        _inputHandler = GetComponent<PlayerInputHandler>();
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
        float moveX = _inputHandler.MoveInput.x;
        float moveY = _inputHandler.MoveInput.y;

        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
        {
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                if (moveX > 0) attackPoint.localPosition = new Vector3(attackDistance, defaultHeight, 0);
                else attackPoint.localPosition = new Vector3(-attackDistance, defaultHeight, 0);
            }
            else
            {
                if (moveY > 0) attackPoint.localPosition = new Vector3(0, defaultHeight, attackDistance);
                else attackPoint.localPosition = new Vector3(0, defaultHeight, -attackDistance);
            }
        }

        // ---> MODIFICA QUI: Chiamiamo la tua nuova funzione per le combo e le animazioni! <---
        HandleAttacks();
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
        if (_inputHandler.AttackPressed && _controller.isGrounded && !IsAttacking)
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

        // 1. Fai partire l'animazione
        if (_animator != null)
        {
            _animator.SetInteger("ComboStep", _comboStep);
            _animator.SetTrigger("AttackTrigger");
        }

        Attack(); // Infliggi il danno

        yield return new WaitForSeconds(0.35f); // Finestra in cui non accetti nuovi input di attacco

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
