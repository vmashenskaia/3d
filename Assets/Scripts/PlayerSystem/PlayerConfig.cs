using UnityEngine;

[CreateAssetMenu(menuName = "Config/Player Config")]
public class PlayerConfig: ScriptableObject
{
    public PlayerStats PlayerStats;
    public float WalkSpeed;
    public float RunSpeed;
    public float JumpHeight;
    public float RotationSpeed;
    public float WaitClimbTimer = 0.15f;
    public  float SlopeLimitAngle = 30f; // min angle for sliding
    public float PlayerGravity;
}