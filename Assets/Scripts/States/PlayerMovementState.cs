using System;
using PlayerSystem;
using UnityEngine;
using Zenject;

namespace States
{
    public class PlayerMovementState: BaseCharacterState
    {
        public override Type[] Transitions { get; }
        
        private readonly InputController _inputController;
        private Vector2 _moveInput;
        private Transform _cameraTransform;
        private bool _isRunning;
        private bool _isMoving;
        private Vector3 _lastMoveDirection = Vector3.zero;

        private float _walkSpeed;
        private float _runSpeed;
        private float _jumpHeight;
        private float _rotationSpeed;

        private readonly Player _player;
        private PlayerView _animations;

        [Inject]
        public PlayerMovementState(Player player, InputController inputController) : base(player)
        {
            _inputController = inputController;
            _player = player;
        }

        public override void Enter(State prevState, params object[] args)
        {
            Debug.Log("Entered PlayerMovementState");

            Initialize();

            _cameraTransform = Camera.main?.transform ?? throw new Exception("Main Camera is null!");

            _animations = _player.PlayerView;
            _animations.StartIdling();
            
            _walkSpeed = _player.Config.WalkSpeed;
            _runSpeed = _player.Config.RunSpeed;
            _jumpHeight = _player.Config.JumpHeight;
            _rotationSpeed = _player.Config.RotationSpeed;

            _inputController.OnMoveInput += OnMoveHandler;
            _inputController.OnMoveStopped += OnMoveStoppedHandler; 
            _inputController.OnSprintInput += OnSprintHandler;
            _inputController.OnSprintStopped += OnSprintStoppedHandler;
        }

        public override void Exit(State nextState, params object[] args)
        {
            _inputController.OnMoveInput -= OnMoveHandler;
            _inputController.OnMoveStopped -= OnMoveStoppedHandler;
            _inputController.OnSprintInput -= OnSprintHandler;
            _inputController.OnSprintStopped -= OnSprintStoppedHandler;
        }

        public override void Service()
        {
            ApplyGravity();
            Move();
        }
        
        private void Move()
        {
            if (_moveInput.sqrMagnitude > 0)
            {
                var moveSpeed = _isRunning ? _runSpeed : _walkSpeed;

                // Определяем направление движения
                Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
                moveDirection = _cameraTransform.TransformDirection(moveDirection);
                moveDirection.y = 0f; // Отключаем вертикальную ось

                _lastMoveDirection = moveDirection.normalized; // Запоминаем последнее направление

                // Применяем скорость
                Vector3 finalMove = _lastMoveDirection * moveSpeed;

                // Гравитация обрабатывается в BaseCharacterState
                finalMove += _velocity;

                // Двигаем персонажа
                Player.CharacterController.Move(finalMove * Time.deltaTime);

                RotatePlayer(_lastMoveDirection);
            }
        }


        private void RotatePlayer(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }

        private void OnMoveHandler(Vector2 input)
        {
            _moveInput = input;

            if (input.sqrMagnitude > 0)
            {
                Vector3 cameraForward = _cameraTransform.forward;
                Vector3 cameraRight = _cameraTransform.right;
                cameraForward.y = 0f;
                cameraRight.y = 0f;
                cameraForward.Normalize();
                cameraRight.Normalize();
        
                _lastMoveDirection = cameraForward * input.y + cameraRight * input.x;
                _lastMoveDirection.y = 0f;
                _lastMoveDirection.Normalize();

                _animations.StartWalking();
            }
        }
        
        private void OnMoveStoppedHandler()
        {
            _moveInput = Vector2.zero;
            _isMoving = false;
            _isRunning = false;
            _animations.StopRunning();
            _animations.StopWalking();
            _animations.StartIdling();
        }
        
        private void OnSprintHandler(bool isSprinting)
        {
            _isRunning = isSprinting;

            if (_isRunning)
            {
                _animations.StartRunning();
                _animations.StopWalking();
            }
        }
        
        private void OnSprintStoppedHandler()
        {
            _isRunning = false;
            _animations.StopRunning();

            if (_isMoving)
                _animations.StartWalking();
        }
    }
}