using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Pathfinding {
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour {
        IAstarAI ai;
        public float wanderRate = 1f;
        public float wanderRadius = 20f;
        private Vector3 wanderTarget;
		public Transform target;

        void OnEnable() {
            ai = GetComponent<IAstarAI>();
        }

        void Update() {
            if (ai != null) {
                if (Time.time > wanderRate) {
                    Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                    wanderTarget = transform.position + randomDirection;
                    wanderRate = Time.time + 1f;
                    ai.destination = wanderTarget;
                }
            }
        }
    }
}

