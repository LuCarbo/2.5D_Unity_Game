using System.Collections;
using System.Threading;
using System.Threading.Tasks;
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

    // Token to manage the asynchronous background timer
    private CancellationTokenSource _comboCancelToken;

    public bool IsAttacking { get; private set; } = false;

    [Header("Statistiche: Attacco Leggero")]
    public int lightAttackDamage = 1;
    public float lightAttackRange = 1f;          

    [Header("Statistiche: Attacco Pesante")]
    public int heavyAttackDamage = 3;
    public float heavyAttackRange = 1.3f;          

    [Header("Targeting")]
    public LayerMask enemyLayers;           

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
        // GESTIONE DIREZIONE
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
        // Controllo input per Leggero o Pesante
        if (!IsAttacking)
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
        // 1. CONSUME THE INPUT
        // Assuming you can write to this variable. If it's a property, 
        // you may need to reset it inside your PlayerInputHandler script instead.
        // _inputHandler.LightAttackPressed = false; 

        RestartAsyncComboTimer();
        
        _heavyComboStep = 0; 
        _lightComboStep++;

        if (_lightComboStep > _maxLightComboSteps) _lightComboStep = 1;

        StartCoroutine(PerformAttackSequence("LightComboStep", _lightComboStep, lightAttackDamage, lightAttackRange, 0.21f));
    }

    private void ExecuteHeavyAttack()
    {
        RestartAsyncComboTimer();
        
        _lightComboStep = 0; // Azzera la combo leggera
        _heavyComboStep++;

        if (_heavyComboStep > _maxHeavyComboSteps) _heavyComboStep = 1;

        StartCoroutine(PerformAttackSequence("HeavyComboStep", _heavyComboStep, heavyAttackDamage, heavyAttackRange, 0.25f));
    }

    // --- NEW ASYNC TIMER LOGIC ---
    private void RestartAsyncComboTimer()
    {
        // Cancel the existing timer if one is running
        if (_comboCancelToken != null)
        {
            _comboCancelToken.Cancel();
            _comboCancelToken.Dispose();
        }

        // Create a new token for this specific timer
        _comboCancelToken = new CancellationTokenSource();

        // Start the background timer without blocking the game thread
        _ = ComboResetTimerAsync(_comboCancelToken.Token);
    }

    private async Task ComboResetTimerAsync(CancellationToken token)
    {
        try
        {
            // Convert seconds to milliseconds for Task.Delay
            int delayMilliseconds = Mathf.RoundToInt(_comboResetTime * 1000);
            
            // Wait asynchronously 
            await Task.Delay(delayMilliseconds, token);

            // If we finish waiting without the token being cancelled, reset combos!
            // We also check !IsAttacking just to be safe if a long animation is playing.
            if (!IsAttacking) 
            {
                ResetCombos();
            }
        }
        catch (TaskCanceledException)
        {
            // The timer was cancelled because the player attacked again. Do nothing!
        }
    }
    // -----------------------------

    private IEnumerator PerformAttackSequence(string animParameter, int step, int damage, float range, float waitTime)
    {
        IsAttacking = true;
        
        if (_animator != null) 
        {
            _animator.SetInteger(animParameter, step);
        }

        Attack(damage, range);

        yield return new WaitForSeconds(waitTime); 

        // 2. RESET THE ANIMATOR PARAMETER
        // This stops the Animator from automatically looping the animation
        // while we wait for the player to press the button again.
        if (_animator != null)
        {
            _animator.SetInteger(animParameter, 0);
        }

        IsAttacking = false;
    }

    private void ResetCombos()
    {
        // Previene reset multipli inutili

        _lightComboStep = 0;
        _heavyComboStep = 0;
        
        if (_animator != null) 
        {
            _animator.SetInteger("LightComboStep", 0);
            _animator.SetInteger("HeavyComboStep", 0);
        }
        
        Debug.Log("Combo Reset tramite Async Task!");
    }

    public void OnPlayerHit()
    {
        if (_animator != null) _animator.SetTrigger("HitTrigger");
        IsAttacking = false;
        
        // Cancel the timer if the player gets hit
        if (_comboCancelToken != null) _comboCancelToken.Cancel();
        
        ResetCombos();
    }

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
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, lightAttackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, heavyAttackRange);
    }

    private void OnDestroy()
    {
        // Sempre ripulire i token quando l'oggetto viene distrutto!
        if (_comboCancelToken != null)
        {
            _comboCancelToken.Cancel();
            _comboCancelToken.Dispose();
        }
    }
}