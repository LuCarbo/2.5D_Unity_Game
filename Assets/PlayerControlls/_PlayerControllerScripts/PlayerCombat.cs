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

    [Header("Audio Combattimento")]
    public GestioneSuoniCombattimento suoniCombattimento;

    [Header("Sistema Combo")]
    [SerializeField] private float _comboResetTime = 0.8f;
    [SerializeField] private int _maxLightComboSteps = 3;
    [SerializeField] private int _maxHeavyComboSteps = 2;

    private int _lightComboStep = 0;
    private int _heavyComboStep = 0;

    private CancellationTokenSource _comboCancelToken;

    public bool IsAttacking { get; private set; } = false;

    // Dati dell'attacco in attesa — verranno usati dall'Animation Event
    private int _pendingDamage = 0;
    private float _pendingRange = 0f;

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
        RestartAsyncComboTimer();

        _heavyComboStep = 0;
        _lightComboStep++;

        if (_lightComboStep > _maxLightComboSteps) _lightComboStep = 1;

        StartCoroutine(PerformAttackSequence("LightComboStep", _lightComboStep, lightAttackDamage, lightAttackRange, 0.21f));
    }

    private void ExecuteHeavyAttack()
    {
        RestartAsyncComboTimer();

        _lightComboStep = 0;
        _heavyComboStep++;

        if (_heavyComboStep > _maxHeavyComboSteps) _heavyComboStep = 1;

        StartCoroutine(PerformAttackSequence("HeavyComboStep", _heavyComboStep, heavyAttackDamage, heavyAttackRange, 0.25f));
    }

    private void RestartAsyncComboTimer()
    {
        if (_comboCancelToken != null)
        {
            _comboCancelToken.Cancel();
            _comboCancelToken.Dispose();
        }

        _comboCancelToken = new CancellationTokenSource();
        _ = ComboResetTimerAsync(_comboCancelToken.Token);
    }

    private async Task ComboResetTimerAsync(CancellationToken token)
    {
        try
        {
            int delayMilliseconds = Mathf.RoundToInt(_comboResetTime * 1000);
            await Task.Delay(delayMilliseconds, token);

            if (!IsAttacking)
            {
                ResetCombos();
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private IEnumerator PerformAttackSequence(string animParameter, int step, int damage, float range, float waitTime)
    {
        IsAttacking = true;

        // Salviamo i dati del colpo: verranno usati da OnAttackHitFrame()
        // che è chiamato tramite Animation Event nel momento esatto del fendente
        _pendingDamage = damage;
        _pendingRange = range;

        if (_animator != null)
        {
            _animator.SetInteger(animParameter, step);
        }

        // NON chiamiamo più Attack() qui — ci pensa l'Animation Event!

        yield return new WaitForSeconds(waitTime);

        if (_animator != null)
        {
            _animator.SetInteger(animParameter, 0);
        }

        IsAttacking = false;
    }

    /// <summary>
    /// Chiamato da un Animation Event nel frame esatto in cui la spada colpisce.
    /// Aggiungilo a ogni clip di attacco nell'Animator di Unity.
    /// </summary>
    public void OnAttackHitFrame()
    {
        Attack(_pendingDamage, _pendingRange);
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

        Debug.Log("Combo Reset tramite Async Task!");
    }

    public void OnPlayerHit()
    {
        if (_animator != null) _animator.SetTrigger("HitTrigger");
        IsAttacking = false;

        if (_comboCancelToken != null) _comboCancelToken.Cancel();

        ResetCombos();
    }

    void Attack(int damage, float range)
    {
        Collider[] oggettiColpiti = Physics.OverlapSphere(attackPoint.position, range);

        bool colpitoNemico = false;
        bool colpitoMuro = false;

        foreach (Collider oggetto in oggettiColpiti)
        {
            if (oggetto.gameObject == this.gameObject) continue;
            if (oggetto.CompareTag("Pavimento")) continue;
            if (oggetto.isTrigger) continue;

            if (oggetto.CompareTag("Enemie"))
            {
                colpitoNemico = true;

                Health enemyHealth = oggetto.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.ChangeHealth(-damage);
                }

                BossHealth bossHealth = oggetto.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(damage);
                }
            }
            else
            {
                colpitoMuro = true;
            }
        }

        // --- RIPRODUZIONE SUONI ---
        if (suoniCombattimento != null)
        {
            if (colpitoNemico)
            {
                suoniCombattimento.RiproduciImpatto("Nemico");
            }
            else if (colpitoMuro)
            {
                suoniCombattimento.RiproduciImpatto("Muro");
            }
            else
            {
                suoniCombattimento.RiproduciFendente();
            }
        }
    }
}