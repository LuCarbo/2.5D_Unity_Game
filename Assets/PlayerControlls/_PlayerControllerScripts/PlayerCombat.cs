using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Riferimenti")]
    private CharacterController _controller;
    private PlayerInputHandler _input;
    private Animator _animator;

    [Header("Sistema Combo")]
    [SerializeField] private int _maxComboSteps = 3; 
    [SerializeField] private float _comboResetTime = 0.8f; 

    private int _comboStep = 0;
    private float _lastAttackTime = 0f;

    // Proprietà pubblica: PlayerMovement può leggerla, ma solo PlayerCombat può modificarla
    public bool IsAttacking { get; private set; } = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!this.enabled) return;
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

    public void OnPlayerDeath()
    {
        if (_animator != null) _animator.SetTrigger("DieTrigger");
        _controller.enabled = false; 
        
        // Spegniamo tutto
        this.enabled = false;
        _input.enabled = false;
        
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }
}