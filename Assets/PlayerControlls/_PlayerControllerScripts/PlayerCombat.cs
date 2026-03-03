using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Riferimenti")]
    private Animator _animator;
    private PlayerInputHandler _input;

    [Header("Stato Combattimento")]
    public bool IsAttacking { get; private set; }

    [Header("Impostazioni Combo")]
    [SerializeField] private float _comboResetTime = 1.0f; // Tempo prima che la combo si resetti
    [SerializeField] private int _maxLightComboSteps = 3;  // Numero massimo di attacchi nella combo leggera

    private float _lastAttackTime;
    private int _currentLightComboStep = 0;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (!this.enabled) return;

        CheckComboReset();
        HandleCombatInputs();
    }

    private void HandleCombatInputs()
    {
        // Se l'input per l'attacco leggero è stato premuto
        if (_input.LightAttackPressed)
        {
            PerformLightAttack();
        }
        // Se l'input per l'attacco pesante è stato premuto
        else if (_input.HeavyAttackPressed)
        {
            PerformHeavyAttack();
        }
    }

    private void PerformLightAttack()
    {

        Debug.Log("ATTACCO LEGGERO ESEGUITO DAL CODICE!"); // <-- Aggiungi questa riga

        _lastAttackTime = Time.time;
        IsAttacking = true;
        _currentLightComboStep++;

        // Aggiorniamo il tempo dell'ultimo attacco
        _lastAttackTime = Time.time;
        IsAttacking = true;

        // Avanziamo nella combo
        _currentLightComboStep++;

        // Se superiamo il limite della combo, ricominciamo dal primo colpo
        if (_currentLightComboStep > _maxLightComboSteps)
        {
            _currentLightComboStep = 1;
        }

        // Comunichiamo all'Animator quale colpo della combo eseguire
        _animator.SetTrigger("LightAttack");
        _animator.SetInteger("LightComboStep", _currentLightComboStep);
    }

    private void PerformHeavyAttack()
    {
        // Un attacco pesante resetta la combo leggera
        _currentLightComboStep = 0;
        _lastAttackTime = Time.time;
        IsAttacking = true;

        _animator.SetTrigger("HeavyAttack");
    }

    private void CheckComboReset()
    {
        // Se è passato troppo tempo dall'ultimo attacco, azzeriamo la combo
        if (Time.time - _lastAttackTime > _comboResetTime)
        {
            _currentLightComboStep = 0;
        }
    }

    // ==========================================
    // ATTENZIONE: EVENTI DI ANIMAZIONE
    // ==========================================

    /// <summary>
    /// Questo metodo deve essere richiamato tramite un "Animation Event" 
    /// nell'ultimo frame di OGNI animazione di attacco.
    /// </summary>
    public void EndAttack()
    {
        IsAttacking = false;

        // Opzionale: Resetta i trigger per evitare che gli attacchi si accumulino se spammi il tasto
        _animator.ResetTrigger("LightAttack");
        _animator.ResetTrigger("HeavyAttack");
    }
}