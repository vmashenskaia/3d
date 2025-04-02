using UnityEngine;

namespace PlayerSystem
{
    [RequireComponent(typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {
        private static readonly int IsIdling = Animator.StringToHash("isIdle");
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

        private Animator _animator;

        public void Initialize() => _animator = GetComponent<Animator>();

        public void StartIdling()
        {
            ResetAll();
            _animator.SetBool(IsIdling, true);
        }

        public void StartWalking()
        {
            if (_animator.GetBool(IsRunning)) return;
            _animator.SetBool(IsIdling, false);
            _animator.SetBool(IsWalking, true);
            _animator.SetBool(IsGrounded, true);
        }

        public void StopWalking()
        {
            if (_animator.GetBool(IsRunning)) return; 
            _animator.SetBool(IsWalking, false);
            StartIdling();
        }

        public void StartRunning()
        {
            _animator.SetBool(IsIdling, false);
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsRunning, true);
            _animator.SetBool(IsGrounded, true);
        }

        public void StopRunning()
        {
            _animator.SetBool(IsRunning, false);
            if (_animator.GetBool(IsWalking)) return;
            StartIdling();
        }

        private void ResetAll()
        {
            _animator.SetBool(IsIdling, false);
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsRunning, false);
        }
    }
}