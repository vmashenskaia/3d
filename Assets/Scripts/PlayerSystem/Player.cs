using System;
using PlayerSystem;
using States;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject]
    private PlayerMovementState _movementState;
    [Inject]
    private PlayerClimbingState _climbingState;
    [Inject]
    private PlayerShootingState _shootingState;
    public const float waitClimbTimer = 0.15f;
    [SerializeField]
    private PlayerConfig _config;
    
    private StateMachine _stateMachine;

    public CharacterController CharacterController { get; private set; }
    public PlayerView PlayerView { get; private set; }
    public float SlopeLimitAngle => _config.SlopeLimitAngle;
    public PlayerConfig Config => _config;
    
    public event Action<ControllerColliderHit> OnHitObstacleCollider;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerView = GetComponent<PlayerView>();
        PlayerView.Initialize();
    }

    private void Start()
    {

        _stateMachine = new StateMachine();

        _stateMachine
            .SetStrict(true)
            .Add(_movementState)
            .Add(_climbingState)
            .Add(_shootingState);

        _stateMachine.SwitchState<PlayerMovementState>();
        Debug.Log("Current state: " + _stateMachine.CurrentState);
        CharacterController.Move(Vector3.down * 0.1f);
    }

    private void Update()
    {
         // Update current state
         _stateMachine.Service();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnHitObstacleCollider?.Invoke(hit);
    }
    
    public bool CheckForCollisions()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask layerMask = ~(1 << groundLayer | 1 << playerLayer); 
            
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CharacterController.radius*2, layerMask);
            
        // foreach (var hitCollider in hitColliders)
        //     Debug.Log("Обнаружен коллайдер: " + hitCollider.name);
            
        return hitColliders.Length > 0;
    }
}