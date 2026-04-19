using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _playerControls;

    // Proprietà pubbliche
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }

    // InteractPressed si auto-consuma al primo accesso
    private bool _interactPressed;
    public bool InteractPressed
    {
        get
        {
            if (_interactPressed)
            {
                _interactPressed = false;
                return true;
            }
            return false;
        }
    }

    public bool CancelPressed { get; private set; }
    public bool JumpPressed { get; private set; }

    // --- INPUT DI ATTACCO ---
    public bool LightAttackPressed { get; private set; }
    public bool HeavyAttackPressed { get; private set; }

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
        _playerControls.Player.Interact.started += OnInteractPerformed;
        _playerControls.Player.Cancel.performed += OnCancelPerformed;
        _playerControls.Player.Jump.performed += OnJumpPerformed;

        _playerControls.Player.LightAttack.performed += OnLightAttackPerformed;
        _playerControls.Player.HeavyAttack.performed += OnHeavyAttackPerformed;
    }

    private void OnDisable()
    {
        _playerControls.Player.Move.performed -= OnMovePerformed;
        _playerControls.Player.Move.canceled -= OnMoveCanceled;
        _playerControls.Player.Run.performed -= OnRunPerformed;
        _playerControls.Player.Run.canceled -= OnRunCanceled;
        _playerControls.Player.Interact.started -= OnInteractPerformed;
        _playerControls.Player.Cancel.performed -= OnCancelPerformed;
        _playerControls.Player.Jump.performed -= OnJumpPerformed;

        _playerControls.Player.LightAttack.performed -= OnLightAttackPerformed;
        _playerControls.Player.HeavyAttack.performed -= OnHeavyAttackPerformed;

        _playerControls.Player.Disable();
    }

    // --- Metodi Callback ---
    private void OnMovePerformed(InputAction.CallbackContext context) { MoveInput = context.ReadValue<Vector2>(); }
    private void OnMoveCanceled(InputAction.CallbackContext context) { MoveInput = Vector2.zero; }
    private void OnRunPerformed(InputAction.CallbackContext context) { IsRunning = true; }
    private void OnRunCanceled(InputAction.CallbackContext context) { IsRunning = false; }
    private void OnInteractPerformed(InputAction.CallbackContext context) { _interactPressed = true; }
    private void OnCancelPerformed(InputAction.CallbackContext context) { CancelPressed = true; }
    private void OnJumpPerformed(InputAction.CallbackContext context) { JumpPressed = true; }

    private void OnLightAttackPerformed(InputAction.CallbackContext context) { LightAttackPressed = true; }
    private void OnHeavyAttackPerformed(InputAction.CallbackContext context) { HeavyAttackPressed = true; }

    private void LateUpdate()
    {
        // InteractPressed si auto-consuma nel getter, non serve resettarlo qui

        if (CancelPressed) CancelPressed = false;
        if (JumpPressed) JumpPressed = false;

        if (LightAttackPressed) LightAttackPressed = false;
        if (HeavyAttackPressed) HeavyAttackPressed = false;
    }
}