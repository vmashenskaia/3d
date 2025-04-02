using System;
using UnityEngine;

namespace States
{
    public class PlayerClimbingState : BaseCharacterState
    {
        public override Type[] Transitions => new[] { typeof(PlayerMovementState) };
         
        private Vector3 _climbEndPosition;
        private Vector3 _lastMoveDirection;
        

        public PlayerClimbingState(Player player) : base(player) {}

        public override void Enter(State prevState, params object[] args)
        {
            var climbPos = (Vector3) args[0]; 
            _lastMoveDirection = (Vector3) args[1];

            Player.CharacterController.enabled = false;
            Debug.Log("Enter Climb State");
        }

        public override void Exit(State nextState, params object[] args)
        {
            Player.CharacterController.enabled = true;
             Debug.Log("Exit Climb State");
        }

        public override void Service()
        {
        }
    }
}