using UnityEngine;

namespace States
{
    public abstract class BaseCharacterState : State
    {
        protected PlayerConfig _playerConfig;
        protected const float gravity = -9.81f;
        protected Vector3 _velocity = Vector3.zero;
        private bool _isGrounded;
        
        //slide
        private Vector3 _slideVelocity = Vector3.zero; 
        private Vector3 _groundNormal;
        private Vector3 _slideVelocityRef = Vector3.zero;
        protected  float _slideSpeed = 5f;
        
        private readonly Player _player;
        protected static readonly RaycastHit[] _raycastHits = new RaycastHit[16];//buffer
        protected Player Player { get; }
        protected bool IsGrounded => _isGrounded;
        //protected Animator Animator => Player.Animator;

        protected BaseCharacterState(Player player)
        {
            Player = player;
        }

        protected void Initialize()
        {
            _playerConfig = Player.Config;
        }
        
        protected virtual void ApplyGravity()
        {
            _isGrounded = CheckGround();
            if (!_isGrounded) 
                _velocity.y += gravity * Time.deltaTime; 
            else 
                _velocity.y = -2f; 

            HandleSlopeMovement(); 
            Player.CharacterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleSlopeMovement()
        {
            _groundNormal = Physics.Raycast(Player.transform.position, Vector3.down, out RaycastHit hit, 1.5f, LayerMask.GetMask("Default")) 
                ? hit.normal 
                : Vector3.up;

            float slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);

            if (slopeAngle > Player.SlopeLimitAngle)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, _groundNormal).normalized;
                _slideVelocity = Vector3.SmoothDamp(_slideVelocity, slideDirection * _slideSpeed, ref _slideVelocityRef, 0.3f);
            }
            else
            {
                _slideVelocity = Vector3.SmoothDamp(_slideVelocity, Vector3.zero, ref _slideVelocityRef, 0.5f);
            }
        }

        private bool CheckGround()
        {
            var center = Player.CharacterController.center;
            var halfHeight = Player.CharacterController.height / 2;
            var radius = Player.CharacterController.radius;
            var p = Player.transform.position + center + Vector3.down * (halfHeight - radius);
            int count = Physics.SphereCastNonAlloc(p, radius, Vector3.down, _raycastHits, 0.1f);
            for (int i = 0; i < count; ++i)
            {
                if (_raycastHits[i].transform == Player.CharacterController.transform) continue;
                var v1 = _raycastHits[i].point - p;
                var v2 = new Vector3(v1.x, 0, v1.z);
                float angle = Vector3.Angle(v1, v2);
                
                if (angle <45) continue;
                return true;
            }
            return count > 1;
        }
    }
}