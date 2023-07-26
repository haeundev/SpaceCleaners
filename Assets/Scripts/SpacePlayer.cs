using UnityEngine;

public class SpacePlayer : MonoBehaviour
{
    [SerializeField] private float idleMoveMoveSpeed = 5f;
    public float IdleMoveSpeed
    {
        get => idleMoveMoveSpeed;
        set => idleMoveMoveSpeed = value;
    }

    [SerializeField] private float fastFastMoveSpeed = 10f;
    public float FastMoveSpeed
    {
        get => fastFastMoveSpeed;
        set => fastFastMoveSpeed = value;
    }

    [SerializeField] private float rotateSpeed = 10f;
    public float RotateSpeed
    {
        get => rotateSpeed;
        set => rotateSpeed = value;
    }
}