using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Riferimenti")]
    private CharacterController _controller;
    private PlayerInputHandler _inputHandler; 
    private Animator _animator;
    public Transform attackPoint;             

    [Header("Sistema Combo")]
    [SerializeField] private float _comboResetTime = 0.8f;
    [SerializeField] private int _maxLightComboSteps = 3; 
    [SerializeField] private int _maxHeavyComboSteps = 2; 

    private int _lightComboStep = 0;
    private int _heavyComboStep = 0;
    private float _lastAttackTime = 0f;

    public bool IsAttacking { get; private set; } = false;

    [Header("Statistiche: Attacco Leggero")]
    public int lightAttackDamage = 1;
    public float lightAttackRange = 1f;          
    public float lightAttackRate = 3f; // Veloce

    [Header("Statistiche: Attacco Pesante")]
    public int heavyAttackDamage = 3;
    public float heavyAttackRange = 1.3f;          
    public float heavyAttackRate = 1f; // Lento

    [Header("Targeting")]
    public LayerMask enemyLayers;           
    private float nextAttackTime = 0f;

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
            attackDistance = Mathf.Abs(attackPoint.localPosition.x);
            if (attackDistance == 0) attackDistance = Mathf.Abs(attackPoint.localPosition.z);
            defaultHeight = attackPoint.localPosition.y;
        }
    }

    void Update()
    {
        // GESTIONE DIREZIONE (uguale a prima)
        float moveX = _inputHandler.MoveInput.x;
        float moveY = _inputHandler.MoveInput.y; 

        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
        {
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                attackPoint.localPosition = new Vector3(moveX > 0 ? attackDistance : -attackDistance, defaultHeight, 0);
            }
            else
            {
                attackPoint.localPosition = new Vector3(0, defaultHeight, moveY > 0 ? attackDistance : -attackDistance);
            }
        }

        HandleAttacks();
    }

    private void HandleAttacks()
    {
        // 1. Reset combo se passa troppo tempo
        if (Time.time - _lastAttackTime > _comboResetTime && !IsAttacking)
        {
            ResetCombos();
        }

        // 2. Controllo input per Leggero o Pesante
        if (Time.time >= nextAttackTime && _controller.isGrounded && !IsAttacking)
        {
            if (_inputHandler.LightAttackPressed)
            {
                ExecuteLightAttack();
            }
            else if (_inputHandler.HeavyAttackPressed)
            {
                ExecuteHeavyAttack();
            }
        }
    }

    private void ExecuteLightAttack()
    {
        _lastAttackTime = Time.time;
        _heavyComboStep = 0; // Azzera la combo pesante
        _lightComboStep++;

        if (_lightComboStep > _maxLightComboSteps) _lightComboStep = 1;

        nextAttackTime = Time.time + 1f / lightAttackRate;
        StartCoroutine(PerformAttackSequence("LightComboStep", _lightComboStep, lightAttackDamage, lightAttackRange, 0.35f));
    }

    private void ExecuteHeavyAttack()
    {
        _lastAttackTime = Time.time;
        _lightComboStep = 0; // Azzera la combo leggera
        _heavyComboStep++;

        if (_heavyComboStep > _maxHeavyComboSteps) _heavyComboStep = 1;

        nextAttackTime = Time.time + 1f / heavyAttackRate;
        StartCoroutine(PerformAttackSequence("HeavyComboStep", _heavyComboStep, heavyAttackDamage, heavyAttackRange, 0.65f));
    }

    private IEnumerator PerformAttackSequence(string animParameter, int step, int damage, float range, float waitTime)
    {
        IsAttacking = true;
        
        if (_animator != null) 
        {
            // Reset grafico degli stati
            _animator.SetInteger("LightComboStep", 0);
            _animator.SetInteger("HeavyComboStep", 0);
            // Attiva l'animazione corretta
            _animator.SetInteger(animParameter, step);
        }

        // Infligge il danno calcolato in base al tipo di attacco
        Attack(damage, range);

        // Finestra di animazione
        yield return new WaitForSeconds(waitTime); 

        IsAttacking = false;
    }

    private void ResetCombos()
    {
        _lightComboStep = 0;
        _heavyComboStep = 0;
        if (_animator != null) 
        {
            _animator.SetInteger("LightComboStep", 0);
            _animator.SetInteger("HeavyComboStep", 0);
        }
    }

    public void OnAttackAnimationEnd() => IsAttacking = false;

    public void OnPlayerHit()
    {
        if (_animator != null) _animator.SetTrigger("HitTrigger");
        IsAttacking = false;
        ResetCombos();
    }

    // Nota: Ora Attack richiede danni e raggio come parametri!
    void Attack(int damage, float range)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, range, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
                Debug.Log($"Colpito: {enemy.name} per {damage} danni!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        // Disegna una sfera gialla per l'attacco leggero e una rossa per il pesante
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, lightAttackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, heavyAttackRange);
    }
}