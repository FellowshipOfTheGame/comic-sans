using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Boss")]
public class BossScript : MonoBehaviour 
{

	[SerializeField] 
	private int life = 100;
	[SerializeField] 
	private float velocity = 4.0f;

	public int Life 
	{
		get { return life; }
		set 
		{
			if(value < 0) life = 0; 
			else life = value; 
		}
	}

	public float Velocity 
	{
		get { return velocity; }
		set { velocity = value; }
	}

	public List<BossPhase> phases;
	private int currentPhase = 0;

	private BossPattern currentPattern;
	private BossAction currentAction;

	private int actionCounter = -1;

	private Vector2 previousPos;

	private Animator _animator;

	// Use this for initialization
	private void Start () 
	{
		
		// Initializes the boss movement pattern.
		currentPattern = phases[currentPhase].firstPattern;

		_animator = GetComponentInChildren<Animator>();
		if(_animator == null)
			Debug.LogWarning("(BossScript) No Animator found on " + transform.name + "!");

		currentPattern = phases[0].firstPattern;
		GetNewAction();

	}

	public void GetNewAction ()
	{

		// Gets the action to be executed.
		actionCounter++;
		// Goes to the next movement pattern.
		if(actionCounter >= currentPattern.actions.Count) 
			GetNewPattern();

		currentAction = currentPattern.actions[actionCounter];
		currentAction.caller = this;
		currentAction.DoAction();

	} 

	private void GetNewPattern () 
	{

		// Goes to a new boss phase if boss life is low enough and thre is one.
		if(Life < phases[currentPhase].lifeToNextPhase && phases.Count > (currentPhase + 1))
		{
			currentPhase++;

			Debug.Log("(BossScript) " + transform.name + " has gone to phase " + (currentPhase + 1) + ".");

			currentPattern = phases[currentPhase].firstPattern;
			if(_animator != null)
			{
				// TODO.
			}
		}

		// Goes through each possible next patterns and adds their chances together.
		int maxChance = 0;
		foreach(BossPattern pattern in currentPattern.nextPattern)
			maxChance += pattern.chance;

		int newRandomPattern = Random.Range(0, maxChance + 1);

		// Uses the random number to select between a boss patterns. 
		int patternCounter = 0;
		foreach(BossPattern pattern in currentPattern.nextPattern) 
		{

			if(newRandomPattern > patternCounter && newRandomPattern < patternCounter + pattern.chance) 
			{
				currentPattern = pattern;
				break;
			}

			patternCounter += pattern.chance;
		}

		actionCounter = 0;

	}

	public IEnumerator ActionMove(BossMovement movement)
	{

		// Sets the movement animation.
		if(_animator != null)
			SetAnimation(movement.animations);

		// Gets the 3D target position.
		Vector3 targetPosition = new Vector3(movement.positionTarget.x, movement.positionTarget.y, 0);

		// Move the boss to the target position.
		while (Vector3.Distance(targetPosition, transform.position) > 0.05f) {			

			// Moves to the target position.
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity * movement.velocityModifier * Time.deltaTime);

			yield return new WaitForEndOfFrame();

		}

		GetNewAction();

	}

	public void ActionAttack(BossAttack attack)
	{

		// Sets the animation for this attack.
		if(_animator != null)
			SetAnimation(attack.animations);

		// Spawns all projectiles.
		foreach(Vector2 spawn in attack.projectileSpawns)
			Instantiate(attack.projectile, transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);
		
		GetNewAction();

	}

	public IEnumerator ActionAttackMove(BossAttackMove attackMove)
	{
	
		Vector3 originalPos = transform.position;
		Vector3 targetPosition = new Vector3(attackMove.positionTarget.x, attackMove.positionTarget.y, 0);
		Vector3 nextStep = transform.position;

		int currentStep = 0;

		// Moves the boss to the target position doing attack on the way.
		while (Vector3.Distance(targetPosition, transform.position) > 0.05f) {

			// Goes to the next step and realizes an attack.
			if(Vector3.Distance(nextStep, transform.position) <= 0.05f)
			{

				currentStep++;
				nextStep = originalPos + ((targetPosition - originalPos) / attackMove.numberOfSteps) * currentStep;

				if(currentStep != 1)
				{

					if(attackMove.stopBeforeAttack)
					{
						// Plays the idle animations and wait for some time.
						if(_animator != null)
							SetAnimation(attackMove.idleAnimations);
						yield return new WaitForSeconds(attackMove.idleTime);
					}

					// Plays the attack animations.
					if(_animator != null)
						SetAnimation(attackMove.attackAnimations);

					// Spawns all projectiles.
					foreach(Vector2 spawn in attackMove.projectileSpawns)
						Instantiate(attackMove.projectile, transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);

					yield return new WaitForEndOfFrame();

					if(attackMove.stopAfterAttack)
					{
						// Plays the idle animations and wait for some time.
						if(_animator != null)
							SetAnimation(attackMove.idleAnimations);
						yield return new WaitForSeconds(attackMove.idleTime);
					} 
					else
					{
						// Plays the movementation animations.
						if(_animator != null)
							SetAnimation(attackMove.movementAnimations);
					}
				}

			} 
			else // Moves.
			{
				// Plays the movementation animations.
				if(_animator != null)
					SetAnimation(attackMove.movementAnimations);

				// Moves the boss.
				transform.position = Vector3.MoveTowards(transform.position, nextStep, velocity * attackMove.velocityModifier * Time.deltaTime);
			}

			yield return new WaitForEndOfFrame();

		}

		// Plays the idle animations.
		if(_animator != null)
			SetAnimation(attackMove.idleAnimations);

		if(attackMove.idleAtEnd)
			yield return new WaitForSeconds(attackMove.idleTime);				

		GetNewAction();

	}

	public void ActionIdle(BossIdle idle)
	{

		// Play the idle animation.
		if(_animator != null)
			SetAnimation(idle.animations);

		// Idles for some time.
		Invoke("GetNewAction", idle.idleTime);

	}

	public IEnumerator ActionTeleport (BossTeleport teleport)
	{
		if(teleport.teleportType == BossTeleport.TeleportType.AnimationThenTP)
		{
			// Plays the animation and idles.
			if(_animator != null)
				SetAnimation(teleport.animations);
			yield return new WaitForSeconds(teleport.delay);

			// Them teleport.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			GetNewAction();

		}
		else if(teleport.teleportType == BossTeleport.TeleportType.TPThenAnimation)
		{			
			
			// Teleport.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			// Them plays the animation and idles.
			if(_animator != null)
				SetAnimation(teleport.animations);
			yield return new WaitForSeconds(teleport.delay);

			GetNewAction();

		}
		else
		{
			// Play the animations.
			if(teleport.teleportType == BossTeleport.TeleportType.AnimatedTP)
				if(_animator != null)
					SetAnimation(teleport.animations);

			// Teleports simultaneously.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			GetNewAction();
		}
	}

	private void SetAnimation(List<AnimationSet> animations)
	{

		// Sets all animation parameters.
		foreach (AnimationSet anim in animations)
			_animator.SetInteger(anim.name, anim.value);

	}

	void OnCollisionEnter2D (Collision2D collision)
	{

		// If the boss has collided with a bullet deals damage to it.
		if(collision.transform.tag == "Bullet")
			Damage(10);

	}

	public void Damage(int amount) 
	{

		// Damages the boss.
		Life-=amount;

		// Kills the boss if the life is zero.
		if(Life <= 0)
			Die();

	}

	private void Die() 
	{

		Debug.Log("(BossScript) " + transform.name + " has been defeated!");
		Destroy(gameObject);

	}
}
