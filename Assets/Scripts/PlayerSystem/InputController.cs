using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace PlayerSystem
{
    public class InputController: IInitializable, IDisposable
    {
        public event Action<bool> OnWalkInput;
        public event Action OnWalkStopped;
        public event Action<Vector2> OnMoveInput;
        public event Action OnMoveStopped;
        public event Action<bool> OnSprintInput;
        public event Action OnSprintStopped;
        public event Action OnShootInput;
        public event Action OnShootStopped;
        public event Action OnJumpInput;
        public event Action<float> OnZoomInput;
        public event Action OnPrimaryActionInput;
        public event Action<float> OnScrollInput; 
        public event Action OnSubmitInput; 
        public event Action OnBackquotePressed;
        

        public Vector2 CurrentMoveInput { get; private set; } 
        public Vector2 CurrentLookInput { get; private set; }

        private readonly PlayerInputActions _inputActions = new();
        private PlayerInput _playerInput;
        private bool _isPlayerActionMapActive = true;

        [Inject]
        public InputController(PlayerInput input)
        {
            _playerInput = input;
        }

        public void Initialize()
        {
            if (_inputActions == null)
            {
                Debug.LogError("PlayerInputActions не инициализирован!");
                return;
            }

            _inputActions.Enable();
            
            _inputActions.Player.Move.performed += OnMovePerformedHandler;
            _inputActions.Player.Move.canceled += OnMoveCanceledHandler;

            _inputActions.Player.Sprint.performed += OnSprintPerformedHandler;
            _inputActions.Player.Sprint.canceled += OnSprintCanceledHandler;
            
            _inputActions.Player.Walk.performed += OnWalkPerformedHandler;
            _inputActions.Player.Walk.canceled += OnWalkCanceledHandler;

            _inputActions.Player.Jump.performed += OnJumpPerformedHandler;

            _inputActions.Player.Zoom.performed += OnZoomPerformedHandler;

            _inputActions.Player.Shooting.performed += OnShootingPerformedHandler;
            _inputActions.Player.Shooting.canceled += OnShootingCanceledHandler;

            _inputActions.Player.Look.performed += OnLookPerformedHandler;
            _inputActions.Player.Look.canceled += OnLookCanceledHandler;
            
            _inputActions.Player.PrimaryAction.performed += OnPrimaryActionPerformedHandler;
            
        }

        public void Dispose()
        {
            _inputActions.Player.Move.performed -= OnMovePerformedHandler;
            _inputActions.Player.Move.canceled -= OnMoveCanceledHandler;

            _inputActions.Player.Sprint.performed -= OnSprintPerformedHandler;
            _inputActions.Player.Sprint.canceled -= OnSprintCanceledHandler;
            
            _inputActions.Player.Walk.performed -= OnWalkPerformedHandler;
            _inputActions.Player.Walk.canceled -= OnWalkCanceledHandler;

            _inputActions.Player.Jump.performed -= OnJumpPerformedHandler;

            _inputActions.Player.Zoom.performed -= OnZoomPerformedHandler;
            
            _inputActions.Player.Shooting.performed -= OnShootingPerformedHandler;
            _inputActions.Player.Shooting.canceled -= OnShootingCanceledHandler;
            
            _inputActions.Player.Look.performed -= OnLookPerformedHandler;
            _inputActions.Player.Look.canceled -= OnLookCanceledHandler;
            
            _inputActions.Player.PrimaryAction.performed -= OnPrimaryActionPerformedHandler;
            
            _inputActions.Disable();
        }
        
        private void OnWalkPerformedHandler(InputAction.CallbackContext context)
        {
            if (OnWalkInput != null)
                OnWalkInput.Invoke(true);
        }

        private void OnWalkCanceledHandler(InputAction.CallbackContext context)
        {
            OnWalkStopped?.Invoke();
        }
        
        private void OnMovePerformedHandler(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            CurrentMoveInput = moveInput;
            OnMoveInput?.Invoke(moveInput);
        }

        private void OnMoveCanceledHandler(InputAction.CallbackContext context)
        {
            CurrentMoveInput = Vector2.zero; 
            OnMoveStopped?.Invoke();
        }

        private void OnSprintPerformedHandler(InputAction.CallbackContext context)
        {
            OnSprintInput?.Invoke(true);
        }
        
        private void OnSprintCanceledHandler(InputAction.CallbackContext context)
        {
            OnSprintInput?.Invoke(false);
            OnSprintStopped?.Invoke();
        }

        private void OnJumpPerformedHandler(InputAction.CallbackContext context)
        {
            OnJumpInput?.Invoke();
        }
        
        private void OnShootingPerformedHandler(InputAction.CallbackContext context)
        {
            OnShootInput?.Invoke();
        }


        private void OnShootingCanceledHandler(InputAction.CallbackContext context)
        {
            OnShootStopped?.Invoke();
        }

        private void OnPrimaryActionPerformedHandler(InputAction.CallbackContext context)
        {
            OnPrimaryActionInput?.Invoke();
        }
        
        private void OnZoomPerformedHandler(InputAction.CallbackContext context)
        {
            float scrollValue = context.ReadValue<float>();
            OnZoomInput?.Invoke(scrollValue);
        }

        private void OnLookPerformedHandler(InputAction.CallbackContext context)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            CurrentLookInput = lookInput;
        }
        
        private void OnLookCanceledHandler(InputAction.CallbackContext context)
        {
            CurrentLookInput = Vector2.zero; 
        }
        
        public bool IsMoveKeyPressed() 
        {
            return CurrentMoveInput.sqrMagnitude > 0;
        }
    }
}