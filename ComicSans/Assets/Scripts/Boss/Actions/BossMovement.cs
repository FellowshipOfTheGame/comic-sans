using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.DataContainers;

namespace ComicSans.Boss.ActionSystem
{

	// Contains the data and execution code for an movement in the Boss ActionSystem.
	[CreateAssetMenu(fileName = "newMovement", menuName = "Boss/Movement", order = 1)]
	public class BossMovement : BossAction {

		[Tooltip("Where the Boss should move to.")]
		public Vector2 positionTarget;
		[Tooltip("Modifier to be applied to the Boss velocity.")]
		public float velocityModifier;

		[Tooltip("List of parameters (int) to be set on the Boss animator.")]
		public List<AnimationSet> animations;

		public override void DoAction()
		{
			caller.StartCoroutine(Move());
		}

		public IEnumerator Move()
		{

			// Sets the movement animation.
			caller.SetAnimation(animations);

			// Gets the 3D target position.
			Vector3 targetPosition = new Vector3(positionTarget.x, positionTarget.y, 0);

			// Move the boss to the target position.
			while (Vector3.Distance(targetPosition, caller.transform.position) > 0.05f) {			

				// Moves to the target position.
				caller.transform.position = Vector3.MoveTowards(caller.transform.position, targetPosition, caller.velocity * velocityModifier * Time.deltaTime);

				yield return new WaitForEndOfFrame();

			}

			caller.NextAction();

		}
	}

	}