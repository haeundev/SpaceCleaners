using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInACircle : MonoBehaviour
{
    // Start is called before the first frame update
    public float angularSpeed = 1f;
    public float circleRad = 1f;

    private Vector2 fixedPoint;
    private float currentAngle;

    void Start()
    {
        fixedPoint = transform.position;
    }

    void Update()
    {
        currentAngle += angularSpeed * Time.deltaTime;
        Vector2 offset = new Vector2(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * circleRad;
        transform.position = new Vector3((fixedPoint + offset).x,0, (fixedPoint + offset).y);
    }
}
