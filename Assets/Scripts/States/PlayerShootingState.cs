using System;
using System.Collections;
using PlayerSystem;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace States
{
    public class PlayerShootingState: BaseCharacterState
    {
        private const float shootingWalkSpeed = 4f;
        private readonly InputController _inputController;

        private Vector2 _moveInput;
        private Vector3 _velocity;
        
        private Transform _cameraTransform;
        //camera
        private readonly Vector2 _sensitivity = new(0.05f, 0.025f);
        private float _verticalAngle;
        private static readonly RaycastHit[] _raycastHits = new RaycastHit[16];//buffer
        //slide
        private Vector3 _slideVelocity = Vector3.zero; 
        private readonly float _slideSpeed = 5f;
        private Vector3 _movement;
        private Vector3 _lerpInputDirection;
        //fire
        private GameObject _muzzleFlashPrefab; 
        private GameObject _hitEffectPrefab;
        private Transform _startFirePoint;
        // private AimTargetController _aimTargetController;

        public override Type[] Transitions => new[] { typeof(PlayerMovementState) };
        
        [Inject]
        public PlayerShootingState(Player player, InputController inputController) : base(player)
        {
            _inputController = inputController;
        }

        public override void Enter(State prevState, params object[] args)
        {
            _inputController.OnShootStopped += OnShootStoppedHandler;
            _inputController.OnMoveInput += OnMoveHandler;
            _inputController.OnMoveStopped += OnMoveStoppedHandler;
            _inputController.OnJumpInput += OnJumpHandler;
            _inputController.OnPrimaryActionInput += OnFireHandler;
            
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
            else
                Debug.LogError("Main Camera is null!");
            
            _velocity = Vector3.zero;
            
            
            if (_inputController.CurrentMoveInput.sqrMagnitude > 0) 
                OnMoveHandler(_inputController.CurrentMoveInput); 
            
            Debug.Log("Enter Shooting State");
        }

        public override void Exit(State nextState, params object[] args)
        {
            // Player.SetAiming(false);
            // Player.SetAimTarget(false);
            
            
            _moveInput = Vector2.zero; 
            _movement = Vector3.zero;
            
            _inputController.OnShootStopped -= OnShootStoppedHandler;
            _inputController.OnMoveInput -= OnMoveHandler;
            _inputController.OnMoveStopped -= OnMoveStoppedHandler;
            _inputController.OnJumpInput -= OnJumpHandler;
            _inputController.OnPrimaryActionInput -= OnFireHandler;
        }

        public override void Service()
        {
            Move();
            SetLookDirection();
        }

        private void Move()
        {
        }

        private void SetLookDirection()
        {
            if (_cameraTransform == null) return;
            
            Vector3 forward = _cameraTransform.forward;
            forward.y = 0f;
            Player.transform.rotation = Quaternion.LookRotation(forward);

            float mouseX = _inputController.CurrentLookInput.x * _sensitivity.x;
            float mouseY = _inputController.CurrentLookInput.y * _sensitivity.y;

            Player.transform.Rotate(Vector3.up * mouseX);
            
            _verticalAngle -= mouseY;
            _verticalAngle = Mathf.Clamp(_verticalAngle, -70f, 70f);
            
            //Player.CameraTarget.localRotation = Quaternion.Euler(_verticalAngle, 0, 0);
        }

        private void SetCameraSettings()
        {
            // _rotationComposer.Damping = Vector2.zero;
            // _orbitalFollow.TrackerSettings.PositionDamping = Vector3.zero;
        }

        private void ClearCameraSettings()
        {
            // _rotationComposer.Damping = new Vector2(0.5f, 0.5f);
            // _orbitalFollow.TrackerSettings.PositionDamping = new Vector3(1, 1, 1);
        }
        
        

        private void UpdateMovementVector()
        {
            if (_moveInput.sqrMagnitude > 0) 
            {
                Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
                moveDirection = _cameraTransform.TransformDirection(moveDirection);
                moveDirection.y = 0f;

                _movement = moveDirection * shootingWalkSpeed;
            }
        }

        private void OnMoveHandler(Vector2 input)
        {
            if (_moveInput == input) return;
            
            _moveInput = input;
            UpdateMovementVector();
        }
        
        private void OnMoveStoppedHandler()
        {
            _moveInput = Vector2.zero;
        }
        
        private void OnShootStoppedHandler()
        {
            Parent.SwitchState<PlayerMovementState>();
        }
        
        private void OnJumpHandler()
        {
        }

        private void OnFireHandler()
        {
        }
    }
}