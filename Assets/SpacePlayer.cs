using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    [SerializeField] private float rotateSpeed;
    public float RotateSpeed
    {
        get => rotateSpeed;
        set => rotateSpeed = value;
    }
}
