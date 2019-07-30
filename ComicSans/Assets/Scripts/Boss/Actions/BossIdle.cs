using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.DataContainers;

namespace ComicSans.Boss.ActionSystem
{

	// Contains the data and execution code for an idle in the Boss ActionSystem.
	[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Idle", order = 6)]
	public class BossIdle : BossAction {

		[Tooltip("The amount of time the Boss will idle.")]
		public float idleTime;

		[Tooltip("List of parameters (int) to be set on the Boss animator.")]
		public List<AnimationSet> animations;

		public override void DoAction()
		{
			caller.StartCoroutine(Idle());
		}

		public IEnumerator Idle()
		{

			// Play the idle animation.
			caller.SetAnimation(animations);

			// Idles for some time.
			float timer = 0;
			while(timer < idleTime) 
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			caller.NextAction();

		}
	}

}