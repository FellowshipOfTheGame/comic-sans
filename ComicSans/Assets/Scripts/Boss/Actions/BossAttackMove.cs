using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Tooltip("The id of the projectile on the Boss projectilePools.")]
    public string projectileId;
    [Tooltip("A projectile will be spawned in each projectileSpawn position relative to the Boss.")]
    public List<Vector2> projectileSpawns;

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
					if(caller.projectileDictionary.ContainsKey(projectileId))
						foreach(Vector2 spawn in projectileSpawns)
							caller.projectileDictionary[projectileId].Spawn(caller.transform.position + new Vector3( spawn.x, spawn.y, 0), caller.transform.rotation);
					else
						Debug.Log("BossAttackMove.AttackMove: Could not spawn projectile " + projectileId + " because there is no ObjectPool with that id!");

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
		if(caller.projectileDictionary.ContainsKey(projectileId))
			foreach(Vector2 spawn in projectileSpawns)
				caller.projectileDictionary[projectileId].Spawn(caller.transform.position + new Vector3( spawn.x, spawn.y, 0), caller.transform.rotation);
		else
			Debug.Log("BossAttackMove.AttackMove: Could not spawn projectile " + projectileId + " because there is no ObjectPool with that id!");

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