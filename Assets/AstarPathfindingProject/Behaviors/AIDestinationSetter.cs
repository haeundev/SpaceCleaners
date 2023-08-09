using UnityEngine;
using System.Collections;

namespace Pathfinding {
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class AIDestinationSetter : VersionedMonoBehaviour {
		/// <summary>The object that the AI should move to</summary>

		public Transform target;
		IAstarAI ai;
		public float chaseRadius = 10f;
		public float moveAroundRadius = 20f;
		public float attackRadius = 3f;

		public float wanderRate = 5f;
		public float wanderRadius = 30f;
		private Vector3 wanderTarget;

		private Animator animator;

		
		void OnEnable () {
			ai = GetComponent<IAstarAI>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;
			animator = GetComponent<Animator>();

		}

		void OnDisable () {
			if (ai != null) ai.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		 void Update () {
            if (target != null && ai != null) {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= chaseRadius) {
                    if (distanceToTarget <= attackRadius) {
                        // Stop the AI from moving
                        ai.isStopped = true;
						animator.SetBool("Walk",false);
                        animator.SetBool("Attack", true);

                        // Perform the attack logic here (e.g., deal damage)
                    }
                    else {
                        // Resume normal movement
                        ai.isStopped = false;

                        // Move towards the player
                        ai.destination = target.position;
                        animator.SetBool("Walk", true);
                        animator.SetBool("Attack", false);
                    }
                


				} else{
					if(Time.time > wanderRate){
					Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
					wanderTarget = transform.position + randomDirection;
					wanderRate = Time.time + 1f;
					animator.SetBool("Walk",true);



				}
				ai.isStopped = false;
				ai.destination = wanderTarget;
			}

			}

		 }
			
		}
	}

