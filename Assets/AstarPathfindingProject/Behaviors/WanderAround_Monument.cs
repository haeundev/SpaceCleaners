using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAround_Monument : MonoBehaviour {
    public float wanderRate = 1f;
    public float wanderRadius = 20f;
    private Vector3 wanderTarget;
    private float nextWanderTime;

    void Start() {
        CalculateNewWanderTarget();
    }

    void Update() {
        if (Time.time >= nextWanderTime) {
            CalculateNewWanderTarget();
        }

        // Move towards the wander target
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, Time.deltaTime);

        // Rotate towards the wander target (optional)
        Vector3 directionToTarget = wanderTarget - transform.position;
        if (directionToTarget != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180f);
        }
    }

    void CalculateNewWanderTarget() {
        nextWanderTime = Time.time + wanderRate;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection.y = 0f; // Ensure the wander is on the same plane as the monster
        wanderTarget = transform.position + randomDirection;
    }
}
