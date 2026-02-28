using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    // ===================================================================
    // DICHIARAZIONE DELLE VARIABILI
    // ===================================================================
    [Header("Riferimenti")]
    private CharacterController _controller;
    private PlayerInputHandler _input;
    private Animator _animator;
    
    [Tooltip("Trascina qui l'oggetto FIGLIO che contiene lo SpriteRenderer")]
    [SerializeField] private Transform _visualModel; 

    [Header("Impostazioni Movimento")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    [Header("Fisica e Salto")]
    [SerializeField] private float _gravity = -20.0f; // Gravità un po' più forte per feel migliore
    [SerializeField] private float _jumpHeight = 1.5f;

    // Variabili di stato
    private float _verticalVelocityY;
    private bool _isAttacking = false;
    private bool _isFacingRight = true;

    // Timeout sicurezza attacco
    private Coroutine _attackCoroutine;

    // ===================================================================
    // METODI DI UNITY
    // ===================================================================
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        
        // Cerca l'animator nel figlio se non assegnato manualmente
        _animator = GetComponentInChildren<Animator>();

        // Auto-assegnazione del visual model se dimenticato nell'inspector
        if (_visualModel == null && _animator != null)
        {
            _visualModel = _animator.transform;
        }

        if (_controller == null) Debug.LogError("CharacterController mancante!");
        if (_visualModel == null) Debug.LogError("ATTENZIONE: Visual Model non assegnato! Il flip non funzionerà.");
    }

    private void Update()
    {
        // Se il personaggio è morto o disabilitato, non fare nulla
        if (!this.enabled) return;

        HandleGravityAndJump();
        HandleHorizontalMovement();
        HandleInteraction();
        HandleAttacks();
        HandleSpriteFlip();
        UpdateAnimatorParameters();
    }

    // ===================================================================
    // LOGICA FISICA E MOVIMENTO
    // ===================================================================
    private void HandleGravityAndJump()
    {
        bool isGrounded = _controller.isGrounded;

        // Reset velocità verticale a terra (piccolo valore negativo per tenerlo incollato)
        if (isGrounded && _verticalVelocityY < 0)
        {
            _verticalVelocityY = -2f;
        }

        // Salto
        if (_input.JumpPressed && isGrounded && !_isAttacking)
        {
            // Formula fisica standard: v = sqrt(h * -2 * g)
            _verticalVelocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        // Applicazione Gravità
        _verticalVelocityY += _gravity * Time.deltaTime;
    }

    private void HandleHorizontalMovement()
    {
        // Se attacca, applica solo gravità, niente movimento orizzontale
        if (_isAttacking)
        {
            _controller.Move(new Vector3(0, _verticalVelocityY, 0) * Time.deltaTime);
            return;
        }

        float currentSpeed = _input.IsRunning ? runSpeed : moveSpeed;
        
        // Calcolo movimento
        Vector3 moveDir = new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y);
        
        // Normalizziamo solo se la lunghezza è > 1 per evitare scatti coi pad, 
        // ma mantenendo controllo preciso con tastiera
        if (moveDir.magnitude > 1f) moveDir.Normalize();

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = _verticalVelocityY; // Aggiungiamo la gravità calcolata prima

        _controller.Move(velocity * Time.deltaTime);
    }

    // ===================================================================
    // LOGICA INTERAZIONE E COMBATTIMENTO
    // ===================================================================
    private void HandleInteraction()
    {
        if (_input.InteractPressed && !_isAttacking)
        {
            Debug.Log("Interazione...");
            // Qui potresti lanciare un Raycast o un OverlapSphere
        }
    }

    private void HandleAttacks()
    {
        if (_input.AttackPressed && !_isAttacking && _controller.isGrounded)
        {
            StartCoroutine(PerformAttack());
        }
    }

    // Coroutine per gestire l'attacco in sicurezza
    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        
        // Ferma il movimento orizzontale istantaneamente per evitare scivolamenti
        // (Opzionale, dipende dal game feel che vuoi)
        
        _animator.ResetTrigger("AttackTrigger");
        _animator.SetTrigger("AttackTrigger");

        // SICUREZZA: Aspetta max 1 secondo (o la durata dell'animazione)
        // Se l'evento OnAttackAnimationEnd non arriva, questo sblocca il player
        yield return new WaitForSeconds(0.8f); 

        // Se siamo ancora in attacco dopo il tempo limite, forziamo l'uscita
        if (_isAttacking)
        {
            _isAttacking = false;
        }
    }

    // Questo metodo va chiamato dagli Animation Events nell'Animation Clip
    // È il modo "pulito" di finire l'attacco, la coroutine sopra è solo la rete di salvataggio
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        _animator.ResetTrigger("AttackTrigger");
    }

    public void OnPlayerHit()
    {
        _animator.SetTrigger("HitTrigger");
        // Opzionale: Interrompi attacco se colpito
        _isAttacking = false; 
    }

    public void OnPlayerDeath()
    {
        _animator.SetTrigger("DieTrigger");
        _controller.enabled = false; // Disabilita collisioni
        this.enabled = false;        // Disabilita questo script
        _input.enabled = false;      // Disabilita input
    }

    // ===================================================================
    // LOGICA VISIVA
    // ===================================================================
    private void HandleSpriteFlip()
    {
        if (_visualModel == null) return;

        // Determina direzione basata sull'input, non sulla velocità (più reattivo)
        if (_input.MoveInput.x < -0.01f)
        {
            _isFacingRight = false;
        }
        else if (_input.MoveInput.x > 0.01f)
        {
            _isFacingRight = true;
        }

        // Applica il flip SOLO al modello visivo figlio
        // Mantiene la scala Y e Z originali, inverte solo la X
        Vector3 currentScale = _visualModel.localScale;
        currentScale.x = _isFacingRight ? Mathf.Abs(currentScale.x) : -Mathf.Abs(currentScale.x);
        _visualModel.localScale = currentScale;
    }

    private void UpdateAnimatorParameters()
    {
        if (_animator == null) return;

        // Usa la velocità reale del controller per l'animazione, non l'input
        // Questo evita che il personaggio "corra sul posto" se sbatte contro un muro
        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        bool isMoving = horizontalVelocity.sqrMagnitude > 0.1f;

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetBool("IsRunning", _input.IsRunning);
        _animator.SetBool("IsJumping", !_controller.isGrounded);

        // Passiamo i parametri per Blend Tree (se li usi)
        _animator.SetFloat("MoveX", _input.MoveInput.x);
        _animator.SetFloat("MoveY", _input.MoveInput.y);
    }
}