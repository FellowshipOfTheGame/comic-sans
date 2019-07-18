using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Entity/Boss")]
public class BossScript : EntityScript 
{

	public static BossScript instance;

	public string id;

	public string bossName;

	[SerializeField] private GameObject _colliders = null;
	private Rigidbody2D _rigidbody = null;

	[SerializeField] private List<BossPhase> phases = null;
	private int currentPhase = 0;

	private BossPattern currentPattern;

	private int currentAction = 0;

	private Vector2 previousPos;

	private Animator _animator;

	// Use this for initialization
	protected override void Awake () 
	{

		if(instance != null)
		{
			Debug.LogWarning("BossScript.Awake: More than one BossScript found! Deleting " + transform.name + "...");
			Destroy(gameObject);
		}

		instance = this;

		base.Awake();

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

		// Initializes the boss health bar.
		if(HUDController.instance != null)
			HUDController.instance.InitializeBossHUD(bossName, health.Hp);
		else
			Debug.LogWarning("BossScript.Awake: No HUDController found!");

		// Starts Boss movimentation.
		Initialize();		

	}

	public void Initialize()
	{

		if(phases.Count < 1) {
			Debug.LogError("BossScript.Initialize " + transform.name + " has no phases!");
			return;
		}

		StartCoroutine(Reset(phases[currentPhase].invincibilityMultiplier));

		if(_animator != null)
			_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Initializes the boss movement pattern.
		if(phases[currentPhase].firstPattern == null)
		{
			Debug.Log("BossScript.Initialize: " + transform.name + "'s current phase has no first pattern.");
			return;
		}
		currentPattern = phases[currentPhase].firstPattern;

		// Gets the action to be executed.
		currentAction = 0;
		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

	public void NextAction ()
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.x > transform.position.x)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.x < transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						else
							Debug.Log("BossScript.NextPattern: Player not found!");
						break;
					case BossPattern.Trigger.PlayerOnTop:
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y > transform.position.y)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.x >= 0)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.x < 0)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y >= 0)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y < 0)
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
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y >= PlayerScript.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenDiagonalBottom:
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y < PlayerScript.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenAntiDiagonalTop:
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y >= -PlayerScript.instance.transform.position.x)
							{
								currentPattern = pattern;
								currentAction = 0;
								return;
							}
						}
						break;
					case BossPattern.Trigger.PlayerOnScreenAntiDiagonalBottom:
						if(PlayerScript.instance != null)
						{
							if(PlayerScript.instance.transform.position.y < -PlayerScript.instance.transform.position.x)
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
		PoolingController.instance.DespawnBossObjects();
		
		StartCoroutine(Reset(phases[currentPhase].invincibilityMultiplier));

		transform.position = new Vector3(phases[currentPhase].initialPosition.x, phases[currentPhase].initialPosition.y, 0);

		currentPattern = phases[currentPhase].firstPattern;
		
		_animator.runtimeAnimatorController = phases[currentPhase].animationController;

		// Gets the action to be executed.
		currentAction = 0;

		currentPattern.actions[currentAction].caller = this;
		currentPattern.actions[currentAction].DoAction();

	}

		protected override void Damage() 
	{

		if(GameController.instance.currentGameState != GameController.GameState.Play)
			return;

		health.Hp-=10;

		if(health.Hp <= 0)
		{
			Die();
			return;
		}

		if(health.Hp < phases[currentPhase].healthToNextPhase)
		{
			if(phases.Count > currentPhase + 1)
				NextPhase();
			else
				Debug.Log("BossScript.Damage: " + transform.name + " is trying to go to the next phase but it doesn't exist! If the current phase is the final remember to set lifeToNextPhase to a negative number on it's file.");
		}

	}

	protected override IEnumerator Reset(float invincibilityMultiplier)
	{

		_colliders.SetActive(false);

		// Idles for some time.
		float timer = 0;
		while(timer < health.invincibilityTime * invincibilityMultiplier) 
		{
			timer += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		_colliders.SetActive(true);

		

	}

	protected override void UpdateLifeHUD()
	{
		
		if(HUDController.instance != null)
            HUDController.instance.UpdateBossHealthBar(health.Hp);
        else
            Debug.LogWarning("PlayerScript.UpdateLifeHUD: No HUDController found!");

	}

	protected override void Die() 
	{

		GameController.instance.currentGameState = GameController.GameState.Win;

		Debug.Log("BossScript.Die: " + transform.name + " has been defeated!");

		StopAllCoroutines();
		PoolingController.instance.DespawnBossObjects();
		
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

		GameController.instance.currentGameState = GameController.GameState.Lose;

		StopAllCoroutines();
		PoolingController.instance.DespawnBossObjects();
		
		if(_animator != null)
			_animator.Play("Win", 0);

		GameController.instance.StartCoroutine(GameController.instance.EndGame(3.5f, false));

	}

	public void SetAnimation(List<AnimationSet> animations)
	{

		// Sets all animation parameters.
		foreach (AnimationSet anim in animations)
			_animator.SetInteger(anim.name, anim.value);

	}

	protected void OnDestroy()
	{

		// Stops the necessary Boss sounds  (must be marked with this script Boss "id").
		AudioController.instance.StopWithTag(id);

		// Destroys the pools generated by this Boss (must be marked with this script Boss "id").	
		PoolingController.instance.DestroyWithTag(id);

	}
}
