using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _playerControls;

    // Proprietà pubbliche
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    
    // Azioni "one-shot"
    public bool InteractPressed { get; private set; }
    public bool CancelPressed { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; } // --- NUOVO ---

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Player.Enable();

        _playerControls.Player.Move.performed += OnMovePerformed;
        _playerControls.Player.Move.canceled += OnMoveCanceled;

        _playerControls.Player.Run.performed += OnRunPerformed;
        _playerControls.Player.Run.canceled += OnRunCanceled;

        _playerControls.Player.Interact.performed += OnInteractPerformed;
        _playerControls.Player.Cancel.performed += OnCancelPerformed;
        _playerControls.Player.Jump.performed += OnJumpPerformed;
        
        // --- NUOVO ---
        _playerControls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        _playerControls.Player.Move.performed -= OnMovePerformed;
        _playerControls.Player.Move.canceled -= OnMoveCanceled;
        _playerControls.Player.Run.performed -= OnRunPerformed;
        _playerControls.Player.Run.canceled -= OnRunCanceled;
        _playerControls.Player.Interact.performed -= OnInteractPerformed;
        _playerControls.Player.Cancel.performed -= OnCancelPerformed;
        _playerControls.Player.Jump.performed -= OnJumpPerformed;

        // --- NUOVO ---
        _playerControls.Player.Attack.performed -= OnAttackPerformed;
        
        _playerControls.Player.Disable();
    }

    // --- Metodi Callback ---

    private void OnMovePerformed(InputAction.CallbackContext context) { MoveInput = context.ReadValue<Vector2>(); }
    private void OnMoveCanceled(InputAction.CallbackContext context) { MoveInput = Vector2.zero; }
    private void OnRunPerformed(InputAction.CallbackContext context) { IsRunning = true; }
    private void OnRunCanceled(InputAction.CallbackContext context) { IsRunning = false; }
    private void OnInteractPerformed(InputAction.CallbackContext context) { InteractPressed = true; }
    private void OnCancelPerformed(InputAction.CallbackContext context) { CancelPressed = true; }
    private void OnJumpPerformed(InputAction.CallbackContext context) { JumpPressed = true; }

    // --- NUOVO ---
    private void OnAttackPerformed(InputAction.CallbackContext context) { AttackPressed = true; }
    

    private void LateUpdate()
    {
        // Resetta i flag "one-shot"
        if (InteractPressed) InteractPressed = false;
        if (CancelPressed) CancelPressed = false;
        if (JumpPressed) JumpPressed = false;
        
        // --- NUOVO ---
        if (AttackPressed) AttackPressed = false;
    }
}