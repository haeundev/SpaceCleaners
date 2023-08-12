using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Pathfinding {
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationNAttack : VersionedMonoBehaviour {
        public Transform target;
        IAstarAI ai;
        public float chaseRadius = 10f;
        public float moveAroundRadius = 20f;
        public float attackRadius = 3f;
        public float wanderRate = 1f; // Changed from 0.5f to 1f for better readability
        public float wanderRadius = 20f;
        private Vector3 wanderTarget;
        private Animator animator;

        void OnEnable() {
            ai = GetComponent<IAstarAI>();
            animator = GetComponent<Animator>();
        }

        void Update() {
            if (target != null && ai != null) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= chaseRadius) {
                    if (distanceToTarget <= attackRadius) {
                        ai.isStopped = true;
                        this.animator.SetTrigger("Attack");
                    } else {
                        ai.isStopped = false;
                        ai.destination = target.position;
                        this.animator.SetTrigger("Walk");
                    }
                } else {
                    if (Time.time > wanderRate) {
                        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                        wanderTarget = transform.position + randomDirection;
                        wanderRate = Time.time + 1f; // Changed from 1f to wanderRate + 1f
                        ai.destination = wanderTarget;
                        this.animator.SetTrigger("Walk");
                    }
                }
            }
        }
    }
}
