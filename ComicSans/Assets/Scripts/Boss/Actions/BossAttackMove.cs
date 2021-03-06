﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.PoolingSystem;
using ComicSans.DataContainers;

namespace ComicSans.Boss.ActionSystem
{

	// Contains the data and execution code for an attack-move in the Boss ActionSystem.
	[CreateAssetMenu(fileName = "newAttackMove", menuName = "Boss/Attack Move", order = 3)]
	public class BossAttackMove : BossAction {

		[Tooltip("Number of times the Boss will stop and attack.")]
		public int numberOfSteps;

		[Tooltip("Where the Boss should move to.")]
		public Vector2 positionTarget;
		[Tooltip("Modifier to be applied to the Boss velocity.")]
		public float velocityModifier;

		[Tooltip("If the Boss should idle before attacking.")]
		public bool stopBeforeAttack;
		[Tooltip("If the Boss should idle after attacking.")]
		public bool stopAfterAttack;
		[Tooltip("The amount of time the Boss will wait when idleing.")]
		public float idleTime;

		[Tooltip("The projectile pool to spawn the projectile from.")]
		public PoolInfo projectilePool;
		[Tooltip("A projectile will be spawned in each projectileSpawn position relative to the Boss.")]
		public List<ProjectileSpawn> projectileSpawns;

		[Tooltip("List of parameters (int) to be set on the Boss animator during movimentation.")]
		public List<AnimationSet> movementAnimations;
		[Tooltip("List of parameters (int) to be set on the Boss animator during idle time.")]
		public List<AnimationSet> idleAnimations;
		[Tooltip("List of parameters (int) to be set on the Boss animator attack.")]
		public List<AnimationSet> attackAnimations;

		[Tooltip("If the Boss should idle at the end of the attack.")]
		public bool idleAtEnd;

		public override void DoAction()
		{
			caller.StartCoroutine(AttackMove());
		}

		public IEnumerator AttackMove()
		{

			float timer = 0;

			Vector3 originalPos = caller.transform.position;
			Vector3 targetPosition = new Vector3(positionTarget.x, positionTarget.y, 0);
			Vector3 nextStep = caller.transform.position;

			int currentStep = 0;

			// Moves the boss to the target position doing attack on the way.
			while (Vector3.Distance(targetPosition, caller.transform.position) > 0.05f) {

				// Moves the Boss to the current step.
				if(Vector3.Distance(nextStep, caller.transform.position) > 0.05f)
				{
					// Plays the movementation animations.
					caller.SetAnimation(movementAnimations);

					// Moves the boss.
					caller.transform.position = Vector3.MoveTowards(caller.transform.position, nextStep, caller.velocity * velocityModifier * Time.deltaTime);

				} 
				else // Goes to the next step and realizes an attack.
				{
					
					currentStep++;
					nextStep = originalPos + ((targetPosition - originalPos) / numberOfSteps) * currentStep;

					if(currentStep != 1)
					{
						if(stopBeforeAttack)
						{
							// Plays the idle animations and wait for some time.
							caller.SetAnimation(idleAnimations);
							
							timer = 0;
							while(timer < idleTime) 
							{
								timer += Time.fixedDeltaTime;
								yield return new WaitForFixedUpdate();
							}
						}

						// Plays the attack animations.
						caller.SetAnimation(attackAnimations);

						// Spawns all projectiles.
						foreach(ProjectileSpawn spawn in projectileSpawns)
							PoolingController.instance.Spawn(projectilePool, caller.transform.position + new Vector3( spawn.position.x, spawn.position.y, 0), Quaternion.Euler(0, 0, spawn.rotation));

						yield return new WaitForEndOfFrame();

						if(stopAfterAttack)
						{
							// Plays the idle animations and wait for some time.
							caller.SetAnimation(idleAnimations);
							timer = 0;
							while(timer < idleTime) 
							{
								timer += Time.fixedDeltaTime;
								yield return new WaitForFixedUpdate();
							}
							
						}
					}
				}

				yield return new WaitForFixedUpdate();

			}

			// Does the last attack.
			if(stopBeforeAttack)
			{
				// Plays the idle animations and wait for some time.
				caller.SetAnimation(idleAnimations);

				timer = 0;
				while(timer < idleTime) 
				{
					timer += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}
			}

			
			caller.SetAnimation(attackAnimations);

			// Spawns all projectiles.
			foreach(ProjectileSpawn spawn in projectileSpawns)
				PoolingController.instance.Spawn(projectilePool, caller.transform.position + new Vector3( spawn.position.x, spawn.position.y, 0), Quaternion.Euler(0, 0, spawn.rotation));

			yield return new WaitForEndOfFrame();

			if(stopAfterAttack)
			{
				// Plays the idle animations and wait for some time.
				caller.SetAnimation(idleAnimations);

				timer = 0;
				while(timer < idleTime) 
				{
					timer += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}
				
			}

			// Idles at the end of the action.
			if(idleAtEnd) 
			{
				caller.SetAnimation(idleAnimations);

				timer = 0;
				while(timer < idleTime) 
				{
					timer += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
			}		

			caller.NextAction();

		}
	}

}