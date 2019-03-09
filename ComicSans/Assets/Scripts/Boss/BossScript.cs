using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Boss")]
public class BossScript : MonoBehaviour 
{	

	public static BossScript instance;

	public string bossName;

	[SerializeField] private int life = 4000;
	private int Life 
	{
		get { return life; }
		set 
		{
			if(value < 0) life = 0; 
			else life = value; 
		}
	}

	[SerializeField] private float velocity = 4.0f;

	[SerializeField] private GameObject _colliders;
	private Rigidbody2D _rigidbody;

	[SerializeField] private List<BossPhase> phases;
	private int currentPhase = 0;

	private BossPattern currentPattern;

	private int currentAction = 0;

	private Vector2 previousPos;

	private Vector3 defaultScale;

	private Animator _animator;

	[System.Serializable]private struct ProjectilePool { public string id; public ObjectPool pool; }
	[SerializeField] private ProjectilePool[] projectilePools;

	private Dictionary<string, ObjectPool> projectileDictionary; 

	public List<GameObject> pooledObjects;

	// Use this for initialization
	private void Awake () 
	{

		if(instance != null)
		{
			Debug.LogWarning("BossScript.Awake: More than one BossScript found! Deleting " + transform.name + "...");
			Destroy(gameObject);
		}

		instance = this;

		// Finds the boss Animator.
		_animator = GetComponentInChildren<Animator>();
		if(_animator == null)
			Debug.LogWarning("BossScript.Awake: No Animator found on " + transform.name + "!");

		// Gives an error if no collider GameObject is assigned the boss Collider.
		if(_colliders == null)
			Debug.LogWarning("BossScript.Awake: You need to assign the Colliders GameObject on " + transform.name + "!");

		// Finds the boss Rigidbody.
		_rigidbody = GetComponentInChildren<Rigidbody2D>();
		if(_rigidbody == null)
			Debug.LogWarning("BossScript.Awake: No Rigidbody found on " + transform.name + "!");

		// Gets the default object scale.
		defaultScale = transform.localScale;

		// Initializes the boss health bar.
		if(HUDController.instance != null)
			HUDController.instance.InitializeBossHUD(bossName, Life);
		else
			Debug.LogWarning("BossScript.Awake: No HUDController found!");

		// Creates a dictionary of projectile types and its respective pools.
		BuildProjectileDictionary();

		// Starts Boss movimentation.
		StartMovimentation();		

	}

	private void BuildProjectileDictionary()
	{

		projectileDictionary = new Dictionary<string, ObjectPool>();
		for(int i = 0; i < projectilePools.Length; i++)
			projectileDictionary.Add(projectilePools[i].id, projectilePools[i].pool);

	}

	private void OnCollisionEnter2D (Collision2D collision)
	{

		// If the boss has collided with a bullet deals damage to it.
		if(collision.transform.tag == "Damage")
			Damage(10);

	}

	private void Damage(int amount) 
	{

		// Used to guarantee the Boss can't be killed by a remaining shot after the Player's death. 
		if(!Player.instance.gameObject.activeSelf)
			return;

		Life-=amount;

		if(HUDController.instance != null)
			HUDController.instance.UpdateBossHealthBar(Life);
		else
			Debug.LogWarning("BossScript.Damage: No HUDController found!");

		if(Life <= 0)
		{
			Die();
			return;
		}

		if(Life < phases[currentPhase].lifeToNextPhase)
		{
			if(phases.Count > currentPhase + 1)
				NextPhase();
			else
				Debug.Log("BossScript.Damage: " + transform.name + " is trying to go to the next phase but it doesn't exist! If the current phase is the final remember to set lifeToNextPhase to a negative number on it's file.");
		}

	}

	private void Die() 
	{

		Debug.Log("BossScript.Die: " + transform.name + " has been defeated!");

		transform.localScale = defaultScale;

		StopAllCoroutines();
		
		if(_animator != null)
			_animator.Play("Die", 0);

		if(HUDController.instance != null)
			HUDController.instance.DisableHUD();
		else
			Debug.LogWarning("BossScript.Die: No HUDController found!");

		GameController.instance.StartCoroutine(GameController.instance.EndGame(3.5f, true));

	}

	public void PlayerDie()
	{

		StopAllCoroutines();
		
		if(_animator != null)
			_animator.Play("Win", 0);

		GameController.instance.StartCoroutine(GameController.instance.EndGame(3.5f, false));

	}

	public void Exit()
	{
		// Destroy objects pooled by the Boss.
		foreach(GameObject obj in pooledObjects)
			Destroy(obj);

		Destroy(gameObject);
	}

	private void StartMovimentation()
	{

		if(phases.Count < 1) {
			Debug.LogError("BossScript.StartMovimentation " + transform.name + " has no phases!");
			return;
		}

		StartCoroutine(Invincible(phases[currentPhase].invincibilityDuration));

		if(_animator != null)
			_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Initializes the boss movement pattern.
		if(phases[currentPhase].firstPattern == null)
		{
			Debug.Log("BossScript.StartMovimentation: " + transform.name + "'s current phase has no first pattern.");
			return;
		}
		currentPattern = phases[currentPhase].firstPattern;

		// Gets the action to be executed.
		currentAction = 0;
		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

	private void NextAction ()
	{

		// Gets the action to be executed.
		currentAction++;
		// Goes to the next movement pattern.
		if(currentAction >= currentPattern.actions.Count) 
			NextPattern();

		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	} 

	private void NextPattern () 
	{

		if(currentPattern.nextPattern.Count == 0)
		{
			Debug.Log("BossScript.NextPattern: Current pattern has no nextPattern on " + transform.name + "!");
			return;	
		}

		// Checks the choice types for next pattern.
		foreach(BossPattern pattern in currentPattern.nextPattern)
		{
			// If the pattern choice type is Trigger check if the trigger is satisfied.
			if(pattern.choiceType != BossPattern.ChoiceType.Random)
			{
				switch (pattern.trigger)
				{
					case BossPattern.Trigger.PlayerOnRight:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.x > transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");

						break;
					case BossPattern.Trigger.PlayerOnLeft:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.x < transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");
						break;
					case BossPattern.Trigger.PlayerOnScreenRight:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.x >= 0)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");

						break;
					case BossPattern.Trigger.PlayerOnScreenLeft:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.x < 0)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");
						break;
					case BossPattern.Trigger.PlayerOnScreenTop:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y >= 0)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");

						break;
					case BossPattern.Trigger.PlayerOnScreenBottom:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y < 0)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");
						break;
					case BossPattern.Trigger.PlayerOnScreenDiagonalTop:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y >= Player.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenDiagonalBottom:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y < Player.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenAntiDiagonalTop:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y >= -Player.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenAntiDiagonalBottom:
						if(Player.instance != null)
						{
							if(Player.instance.transform.position.y < -Player.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}	
						break;
				}
			}
		}

		// If no trigger is satisfied or all patterns choiceType is random, picks a random one between those that are not OnlyTrigger.

		// Goes through each possible next patterns and adds their chances together.
		int maxChance = 0;
		foreach(BossPattern pattern in currentPattern.nextPattern)
		{
			if(pattern.choiceType != BossPattern.ChoiceType.OnlyTrigger)
				maxChance += pattern.chance;
		}

		int newRandomPattern = Random.Range(0, maxChance);

		// Uses the random number to select between a boss patterns. 
		int patternCounter = 0;
		foreach(BossPattern pattern in currentPattern.nextPattern) 
		{

			if(pattern.choiceType != BossPattern.ChoiceType.OnlyTrigger)
			{

				if(newRandomPattern >= patternCounter && newRandomPattern < patternCounter + pattern.chance) 
				{
					currentPattern = pattern;
					currentAction = 0;
					return;
				}

				patternCounter += pattern.chance;
			}
		}	

		Debug.LogError("BossScript.NextPattern: No pattern randomly selected on " + transform.name + "! Either dark magic happened, or (more probable) someone did something wrong with the selection code. Defaulting to the first pattern on the list....");
		currentPattern = currentPattern.nextPattern[0];
		currentAction = 0;

	}

	private void NextPhase()
	{
		currentPhase++;

		Debug.Log("BossScript.NextPhase: " + transform.name + " has gone to phase " + (currentPhase + 1) + ".");

		// Sets the boss to the initial conditions.
		StopAllCoroutines();

		// Despawns all previous phase projectiles.
		foreach(GameObject obj in pooledObjects)
			obj.GetComponent<PooledObject>().Despawn();

		StartCoroutine(Invincible(phases[currentPhase].invincibilityDuration));

		transform.position = new Vector3(phases[currentPhase].initialPosition.x, phases[currentPhase].initialPosition.y, 0);

		transform.localScale = defaultScale;

		currentPattern = phases[currentPhase].firstPattern;
		
		_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Gets the action to be executed.
		currentAction = 0;

		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

	public IEnumerator Invincible(float duration)
	{

		_colliders.SetActive(false);

		// Idles for some time.
		float timer = 0;
		while(timer < duration) 
		{
			timer += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		_colliders.SetActive(true);

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
			Debug.Log("BossScript.ActionAttack: Could not spawn projectile " + attack.projectileId + " because there is no ObjectPool with that id!");

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

			// Moves the Boss to the current step.
			if(Vector3.Distance(nextStep, transform.position) > 0.05f)
			{
				// Plays the movementation animations.
				if(_animator != null)
					SetAnimation(attackMove.movementAnimations);

				// Moves the boss.
				transform.position = Vector3.MoveTowards(transform.position, nextStep, velocity * attackMove.velocityModifier * Time.deltaTime);

			} 
			else // Goes to the next step and realizes an attack.
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
							timer += Time.fixedDeltaTime;
							yield return new WaitForFixedUpdate();
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
						Debug.Log("BossScript.ActionAttackMove: Could not spawn projectile " + attackMove.projectileId + " because there is no ObjectPool with that id!");

					yield return new WaitForEndOfFrame();

					if(attackMove.stopAfterAttack)
					{
						// Plays the idle animations and wait for some time.
						if(_animator != null)
							SetAnimation(attackMove.idleAnimations);
						timer = 0;
						while(timer < attackMove.idleTime) 
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
		if(attackMove.stopBeforeAttack)
		{
			// Plays the idle animations and wait for some time.
			if(_animator != null)
				SetAnimation(attackMove.idleAnimations);
			timer = 0;
			while(timer < attackMove.idleTime) 
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
		}

		
		if(_animator != null)
			SetAnimation(attackMove.attackAnimations);

		// Spawns all projectiles.
		if(projectileDictionary.ContainsKey(attackMove.projectileId))
			foreach(Vector2 spawn in attackMove.projectileSpawns)
				projectileDictionary[attackMove.projectileId].Spawn(transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);
		else
			Debug.Log("BossScript.ActionAttackMove: Could not spawn projectile " + attackMove.projectileId + " because there is no ObjectPool with that id!");

		yield return new WaitForEndOfFrame();

		if(attackMove.stopAfterAttack)
		{
			// Plays the idle animations and wait for some time.
			if(_animator != null)
				SetAnimation(attackMove.idleAnimations);
			timer = 0;
			while(timer < attackMove.idleTime) 
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
			
		}

		// Idles at the end of the action.
		if(attackMove.idleAtEnd) 
		{
			if(_animator != null)
			SetAnimation(attackMove.idleAnimations);

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
			timer += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
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
					timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
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
					timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
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

	public IEnumerator ActionDash(BossDash dash) {

		// Sets the charge animation.
		if(_animator != null)
			SetAnimation(dash.chargeAnimations);

		// Plays the audio.
		if(AudioControlCenter.instance != null)
			if(dash.audioId != null && dash.audioId != "none")
			AudioControlCenter.instance.Play(dash.audioId);

		// Waits for the charging time.
		float time = 0f;
		while(time < dash.chargeTime)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		// Gets the 3D target position.
		Vector2 dirVector;
		if(Player.instance != null) {

			// Sets the dash animation.
			if(_animator != null)
				SetAnimation(dash.dashAnimations);

			dirVector = Player.instance.transform.position - transform.position;
			dirVector.Normalize();

			float accel = 0;

			if(!dash.lookingRight && dirVector.x > 0)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			else if(dash.lookingRight && dirVector.x < 0)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			else
				transform.localScale = defaultScale;

			int bounces = 0;

			// Used to guarantee that bounces can't be accounted 2 times because of physics limitations.
			bool bounceLeft = false;
			bool bounceRight = false;
			bool bounceTop = false;
			bool bounceBottom = false;

			// Move the boss to the target position.
			while (bounces < dash.bounceAmount) {			

				// Moves to the target position.
				if(bounces == 0)
					accel += dash.aceleration * Time.deltaTime;
				transform.Translate(dirVector * accel * Time.deltaTime);

				yield return new WaitForEndOfFrame();

				if(Mathf.Abs(transform.position.x) > dash.positionConstraints.x || Mathf.Abs(transform.position.y) > dash.positionConstraints.y)
				{

					if(transform.position.x > dash.positionConstraints.x && !bounceRight)
					{
						dirVector = new Vector2(-dirVector.x, dirVector.y);
						
						bounceLeft = false;
						bounceRight = true;
					}
					else if(transform.position.x < -dash.positionConstraints.x && !bounceLeft)
					{
						dirVector = new Vector2(-dirVector.x, dirVector.y);

						bounceLeft = true;
						bounceRight = false;
					}

					if(transform.position.y > dash.positionConstraints.y && !bounceTop)
					{
						dirVector = new Vector2(dirVector.x, -dirVector.y);

						bounceTop = true;
						bounceBottom = false;
					}
					else if(transform.position.y < -dash.positionConstraints.y && !bounceBottom)
					{
						dirVector = new Vector2(dirVector.x, -dirVector.y);

						bounceTop = false;
						bounceBottom = true;
					}

					bounces++;

				}

			}
		}
		else 
		{
			Debug.Log("BossScript.ActionDash: Player not found");
		}

		transform.localScale = defaultScale;
		NextAction();

	}

	private void SetAnimation(List<AnimationSet> animations)
	{

		// Sets all animation parameters.
		foreach (AnimationSet anim in animations)
			_animator.SetInteger(anim.name, anim.value);

	}
}
