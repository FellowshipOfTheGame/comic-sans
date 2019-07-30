using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.DataContainers;

namespace ComicSans.Boss.ActionSystem
{

	// Contains the data and execution code for an teleport in the Boss ActionSystem.
	[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Teleport", order = 5)]
	public class BossTeleport : BossAction {

		
		public enum TeleportType {AnimationThenTP, TPThenAnimation, AnimatedTP, JustTP};
		[Tooltip("How the Boss should teleport to.")]
		public TeleportType teleportType;

		[Tooltip("Where the Boss should")]
		public Vector2 destination;

		[Tooltip("Time to be used when the Boss needs to wait.")]
		public float delay;

		[Tooltip("List of parameters (int) to be set on the Boss animator.")]
		public List<AnimationSet> animations;

		public override void DoAction()
		{
			caller.StartCoroutine(Teleport());
		}

		public IEnumerator Teleport ()
		{
			float timer = 0;
			if(teleportType == TeleportType.AnimationThenTP)
			{
				// Plays the animation and idles.
				caller.SetAnimation(animations);

				while(timer < delay) 
				{
						timer += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}

				// Them teleport.
				caller.transform.position = new Vector3(destination.x, destination.y, 0);

				caller.NextAction();

			}
			else if(teleportType == TeleportType.TPThenAnimation)
			{			
				
				// Teleport.
				caller.transform.position = new Vector3(destination.x, destination.y, 0);

				// Them plays the animation and idles.
				caller.SetAnimation(animations);

				while(timer < delay) 
				{
						timer += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}

				caller.NextAction();

			}
			else
			{
				// Play the animations.
				if(teleportType == TeleportType.AnimatedTP)
				
				caller.SetAnimation(animations);

				// Teleports simultaneously.
				caller.transform.position = new Vector3(destination.x, destination.y, 0);

				caller.NextAction();
			}
		}
	}

}