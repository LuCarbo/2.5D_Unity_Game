using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Riferimenti")]
    private CharacterController _controller;
    private PlayerInputHandler _input;
    private PlayerCombat _combat;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Transform _visualModel;

    [Header("Impostazioni Movimento")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    [Tooltip("Layer dei nemici per non scavalcarli")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Fisica e Salto")]
    [SerializeField] private float _gravity = -20.0f;
    [SerializeField] private float _jumpHeight = 1.5f;

    private float _verticalVelocityY;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _combat = GetComponent<PlayerCombat>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (_visualModel == null && _animator != null)
        {
            _visualModel = _animator.transform;
        }
    }

    private void Update()
    {
        if (!this.enabled) return;

        HandleGravityAndJump();
        HandleHorizontalMovement();
        // HandleInteraction RIMOSSO - rubava InteractPressed al DialogueTrigger
        HandleSpriteFlip();
        UpdateAnimatorParameters();
    }

    private void HandleGravityAndJump()
    {
        bool isGrounded = _controller.isGrounded;

        if (isGrounded && _verticalVelocityY < 0)
        {
            _verticalVelocityY = -2f;
        }

        bool isAttacking = _combat != null && _combat.IsAttacking;

        if (_input.JumpPressed && isGrounded)
        {
            _verticalVelocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        _verticalVelocityY += _gravity * Time.deltaTime;
    }

    private void HandleHorizontalMovement()
    {
        float currentSpeed = _input.IsRunning ? runSpeed : moveSpeed;

        Vector3 moveDir = new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y);
        if (moveDir.magnitude > 1f) moveDir.Normalize();

        if (moveDir.magnitude > 0.1f)
        {
            float playerRadius = _controller.radius;
            Vector3 startPoint = transform.position + new Vector3(0, 0.2f, 0);

            if (Physics.SphereCast(startPoint, playerRadius, moveDir, out RaycastHit hit, 0.2f, enemyLayer))
            {
                moveDir = Vector3.zero;
            }
        }

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = _verticalVelocityY;
        _controller.Move(velocity * Time.deltaTime);
    }

    private void HandleSpriteFlip()
    {
        if (_spriteRenderer == null) return;

        if (_input.MoveInput.x < -0.01f) _spriteRenderer.flipX = true;
        else if (_input.MoveInput.x > 0.01f) _spriteRenderer.flipX = false;
    }

    private void UpdateAnimatorParameters()
    {
        if (_animator == null) return;

        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        bool isMoving = horizontalVelocity.sqrMagnitude > 0.1f;

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetBool("IsRunning", _input.IsRunning);
        _animator.SetBool("IsJumping", !_controller.isGrounded);
        _animator.SetFloat("MoveX", _input.MoveInput.x);
        _animator.SetFloat("MoveY", _input.MoveInput.y);
    }

    public void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("DieTrigger");
        }

        this.enabled = false;

        if (_combat != null)
        {
            _combat.enabled = false;
        }

        gameObject.tag = "Untagged";

        Debug.Log("Il Player è morto!");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1.2f);

        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = false;
        }

        Debug.Log("Il corpo è sparito. Cerco lo script di Respawn...");

        PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();

        if (respawnScript != null)
        {
            respawnScript.Respawn();
        }
        else
        {
            Debug.LogError("ERRORE CRITICO: Lo script PlayerRespawn non è attaccato al Player!");
        }
    }

    public void Revive()
    {
        Debug.Log("--- 1. Inizio rianimazione del Player ---");

        this.enabled = true;
        if (_combat != null) _combat.enabled = true;

        gameObject.tag = "Player";

        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = true;
            Debug.Log("Sprite riattivato!");
        }

        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        Debug.Log("--- 2. Player pronto e comandi riattivati! ---");
    }
}