using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Boss")]
public class BossScript : MonoBehaviour 
{	

	[SerializeField] protected int life = 100;
	public int Life 
	{
		get { return life; }
		set 
		{
			if(value < 0) life = 0; 
			else life = value; 
		}
	}

	protected BossHealthBar healthBar;

	[SerializeField] protected float velocity = 4.0f;
	public float Velocity 
	{
		get { return velocity; }
		set { velocity = value; }
	}

	protected Collider2D _collider;

	public List<BossPhase> phases;
	protected int currentPhase = 0;

	protected BossPattern currentPattern;

	protected int currentAction = 0;

	protected Vector2 previousPos;

	protected Animator _animator;

	[System.Serializable]protected struct ProjectilePool { public string id; public ObjectPool pool; }
	[SerializeField] protected ProjectilePool[] projectilePools;

	protected Dictionary<string, ObjectPool> projectileDictionary; 

	// Use this for initialization
	protected void Awake () 
	{
		// Finds the boss Animator.
		_animator = GetComponentInChildren<Animator>();
		if(_animator == null)
			Debug.LogWarning("(BossScript) No Animator found on " + transform.name + "!");

		// Finds the boss Collider.
		_collider = GetComponentInChildren<Collider2D>();
		if(_collider == null)
			Debug.LogWarning("(BossScript) No Collider found on " + transform.name + "!");

		StartMovimentation();

		// Creates a dictionary of projectile types and its respective pools.
		BuildProjectileDictionary();

		// Finds the boss health bar.
		healthBar = FindObjectOfType(typeof(BossHealthBar)) as BossHealthBar;
		if(healthBar != null)
			healthBar.SetIntitialLife(Life);
		else
			Debug.Log("(BossScript) No health bar found for the boss!");

	}

	protected void BuildProjectileDictionary()
	{

		projectileDictionary = new Dictionary<string, ObjectPool>();
		for(int i = 0; i < projectilePools.Length; i++)
			projectileDictionary.Add(projectilePools[i].id, projectilePools[i].pool);

	}

	protected void OnCollisionEnter2D (Collision2D collision)
	{

		// If the boss has collided with a bullet deals damage to it.
		if(collision.transform.tag == "Damage")
			Damage(10);

	}

	protected void Damage(int amount) 
	{

		Life-=amount;

		if(healthBar != null)
			healthBar.UpdateHealthBar(Life);

		if(Life <= 0)
		{
			Die();
			return;
		}

		if(Life < phases[currentPhase].lifeToNextPhase && phases.Count > (currentPhase + 1))
			NextPhase();

	}

	protected void Die() 
	{

		Debug.Log("(BossScript) " + transform.name + " has been defeated!");
		Destroy(gameObject);

	}

	protected void StartMovimentation()
	{

		StartCoroutine(Invincible(phases[currentPhase].invincibilityDuration));

		_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Initializes the boss movement pattern.
		currentPattern = phases[currentPhase].firstPattern;

		// Gets the action to be executed.
		currentAction = 0;
		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

	protected void NextAction ()
	{

		// Gets the action to be executed.
		currentAction++;
		// Goes to the next movement pattern.
		if(currentAction >= currentPattern.actions.Count) 
			NextPattern();

		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	} 

	protected void NextPattern () 
	{

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

		currentAction = 0;

	}

	protected void NextPhase()
	{
		currentPhase++;

		Debug.Log("(BossScript) " + transform.name + " has gone to phase " + (currentPhase + 1) + ".");

		// Sets the boss to the initial conditions.
		StopAllCoroutines();

		StartCoroutine(Invincible(phases[currentPhase].invincibilityDuration));

		transform.position = new Vector3(phases[currentPhase].initialPosition.x, phases[currentPhase].initialPosition.y, 0);

		currentPattern = phases[currentPhase].firstPattern;
		
		_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Gets the action to be executed.
		currentAction = 0;

		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

	public IEnumerator Invincible(float duration)
	{

		_collider.enabled = false;

		// Idles for some time.
		float timer = 0;
		while(timer < duration) 
		{
			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		_collider.enabled = true;

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

		NextAction();

	}

	public void ActionAttack(BossAttack attack)
	{

		// Sets the animation for this attack.
		if(_animator != null)
			SetAnimation(attack.animations);

		// Spawns all projectiles.
		if(projectileDictionary.ContainsKey(attack.projectileId))
			foreach(Vector2 spawn in attack.projectileSpawns)
				projectileDictionary[attack.projectileId].Spawn(transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);
		else
			Debug.Log("(BossScript) Could not spawn projectile " + attack.projectileId + " because there is no ObjectPool with that id!");

		NextAction();

	}

	public IEnumerator ActionAttackMove(BossAttackMove attackMove)
	{

		float timer = 0;

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
						timer = 0;
						while(timer < attackMove.idleTime) 
						{
							timer += Time.deltaTime;
							yield return new WaitForEndOfFrame();
						}
					}

					// Plays the attack animations.
					if(_animator != null)
						SetAnimation(attackMove.attackAnimations);

					// Spawns all projectiles.
					if(projectileDictionary.ContainsKey(attackMove.projectileId))
						foreach(Vector2 spawn in attackMove.projectileSpawns)
							projectileDictionary[attackMove.projectileId].Spawn(transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);
					else
						Debug.Log("(BossScript) Could not spawn projectile " + attackMove.projectileId + " because there is no ObjectPool with that id!");

					yield return new WaitForEndOfFrame();

					if(attackMove.stopAfterAttack)
					{
						// Plays the idle animations and wait for some time.
						if(_animator != null)
							SetAnimation(attackMove.idleAnimations);
						timer = 0;
						while(timer < attackMove.idleTime) 
						{
							timer += Time.deltaTime;
							yield return new WaitForEndOfFrame();
						}
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

		// Idles at the end of the action.
		if(attackMove.idleAtEnd) 
		{
			timer = 0;
			while(timer < attackMove.idleTime) 
			{
				timer += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}		

		NextAction();

	}

	public IEnumerator ActionIdle(BossIdle idle)
	{

		// Play the idle animation.
		if(_animator != null)
			SetAnimation(idle.animations);

		// Idles for some time.
		float timer = 0;
		while(timer < idle.idleTime) 
		{
			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		NextAction();

	}

	public IEnumerator ActionTeleport (BossTeleport teleport)
	{
		float timer = 0;
		if(teleport.teleportType == BossTeleport.TeleportType.AnimationThenTP)
		{
			// Plays the animation and idles.
			if(_animator != null)
				SetAnimation(teleport.animations);
			while(timer < teleport.delay) 
			{
					timer += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			// Them teleport.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			NextAction();

		}
		else if(teleport.teleportType == BossTeleport.TeleportType.TPThenAnimation)
		{			
			
			// Teleport.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			// Them plays the animation and idles.
			if(_animator != null)
				SetAnimation(teleport.animations);
			while(timer < teleport.delay) 
			{
					timer += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			NextAction();

		}
		else
		{
			// Play the animations.
			if(teleport.teleportType == BossTeleport.TeleportType.AnimatedTP)
				if(_animator != null)
					SetAnimation(teleport.animations);

			// Teleports simultaneously.
			transform.position = new Vector3(teleport.destination.x, teleport.destination.y, 0);

			NextAction();
		}
	}

	protected void SetAnimation(List<AnimationSet> animations)
	{

		// Sets all animation parameters.
		foreach (AnimationSet anim in animations)
			_animator.SetInteger(anim.name, anim.value);

	}
}
