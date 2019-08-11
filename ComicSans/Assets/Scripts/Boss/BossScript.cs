using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.UI;
using ComicSans.Player;
using ComicSans.PoolingSystem;
using ComicSans.DataContainers;
using ComicSans.Boss.ActionSystem;

namespace ComicSans.Boss
{

	// The base script for the Bosses.
	// IMPORTANT: Remember to add a BossStateMachineHandler on the last Boss phase AnimationController in the 'Win' and 'Die' states.
	[AddComponentMenu("Scripts/Entity/Boss")]
	public class BossScript : EntityScript 
	{

		public static BossScript instance;

		[Tooltip("Used to uniquile identify this Boss and to tag it's Audios and Projectiles.")]
		public string id;

		[Tooltip("Object containing all the colliders used by the Boss.")]
		[SerializeField] private GameObject _colliders = null;

		private Rigidbody2D _rigidbody = null;

		[Tooltip("List of phases used by the current Boss.")]
		[SerializeField] private List<BossPhase> phases = null;
		private int currentPhase = 0;

		private BossPattern currentPattern;

		private int currentAction = 0;

		private Animator _animator;

		[Tooltip("Delay after the Boss end animation.")]
		public float endAnimationDelay = 1.25f;

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

			// Starts Boss movimentation.
			Initialize();		

		}

		public void Initialize()
		{

			// Checks if there is a phase.
			if(phases.Count < 1) {
				Debug.LogError("BossScript.Initialize " + transform.name + " has no phases!");
				return;
			}

			// Initializes the Boss health bar.
			if(GameController.instance.uiController != null)
				GameController.instance.uiController.InitializeBossHUD(phases[currentPhase].bossPhaseName, health.Hp);
			else
				Debug.LogWarning("BossScript.Initialize: No UIController found!");

			// Start the Boss invincibility.
			StartCoroutine(Reset(phases[currentPhase].invincibilityMultiplier));

			// Sets the Boss AnimationController.
			if(_animator != null)
				_animator.runtimeAnimatorController = phases[currentPhase].animationController;

			// Initializes the Boss movementation.
			if(phases[currentPhase].firstPattern == null)
			{
				Debug.LogWarning("BossScript.Initialize: " + transform.name + "'s current phase has no first pattern.");
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

			currentPattern.actions[currentAction].caller = this; // Sets the caller of the action to this script.
			currentPattern.actions[currentAction].DoAction();

		} 

		private void NextPattern () 
		{

			if(currentPattern.nextPattern.Count == 0)
			{
				Debug.LogWarning("BossScript.NextPattern: Current pattern has no nextPattern on " + transform.name + "!");
				return;	
			}

			transform.localScale = Vector3.one; // Ensures the Boss scale can't be left wrong by a previous pattern.

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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");

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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");
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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");
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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");

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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");
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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");

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
								Debug.LogWarning("BossScript.NextPattern: Player not found!");
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

			Debug.LogError("BossScript.NextPattern: No pattern randomly selected on " + transform.name + "! Either dark magic happened, or (more probable) someone did something wrong with the selection code.");

		}

		private void NextPhase()
		{
			currentPhase++;

			Debug.Log("BossScript.NextPhase: " + transform.name + " has gone to phase " + (currentPhase + 1) + ".");

			// Sets the Boss phase name.
			if(GameController.instance.uiController != null)
				GameController.instance.uiController.UpdateBossName(phases[currentPhase].bossPhaseName);
			else
				Debug.LogWarning("BossScript.NextPhase: No UIController found!");

			// Sets the boss to the initial conditions.
			StopAllCoroutines();

			// Despawns all previous phase projectiles.
			PoolingController.instance.DespawnBossObjects();

			// Stops all previous phase sounds.
			AudioController.instance.StopWithTag(id);
			
			StartCoroutine(Reset(phases[currentPhase].invincibilityMultiplier));

			// Resets the Boss position.
			transform.position = new Vector3(phases[currentPhase].initialPosition.x, phases[currentPhase].initialPosition.y, 0);

			// Sets the first pattern.
			currentPattern = phases[currentPhase].firstPattern;
			
			// Assigns the new AnimationController.
			_animator.runtimeAnimatorController = phases[currentPhase].animationController;

			// Gets the action to be executed.
			currentAction = 0;

			// Initiates new phase's first pattern actions.
			currentPattern.actions[currentAction].caller = this;
			currentPattern.actions[currentAction].DoAction();

		}

		protected override void Damage() 
		{

			// Guaratees the Boss can't take damage when he shouldn't.
			if(GameController.instance.currentGameState != GameController.GameState.Play)
				return;

			// Damages the Boss.
			health.Hp-=10;

			// Verifies if the Boss should pass to the next phase.
			if(health.Hp < phases[currentPhase].healthToNextPhase)
			{
				
				if(phases.Count > currentPhase + 1)
					NextPhase();
				else
					Debug.LogWarning("BossScript.Damage: " + transform.name + " is trying to go to the next phase but it doesn't exist! If the current phase is the final remember to set lifeToNextPhase to a negative number on it's file.");
			
				return;
				
			}

			// Verifies if the boss has died.
			if(health.Hp <= 0)
			{
				Die();
				return;
			}

			

		}

		// Updates the Boss health bar.
		protected override void UpdateLifeHUD()
		{
			
			if(GameController.instance.uiController != null)
				GameController.instance.uiController.UpdateBossHealthBar(health.Hp);
			else
				Debug.LogWarning("PlayerScript.UpdateLifeHUD: No UIController found!");

		}

		// Resets the Boss position and aplies invincibility in scene transitions.
		protected override IEnumerator Reset(float invincibilityMultiplier)
		{

			_colliders.SetActive(false); // Disable Boss colliders so it doesn't take damage.

			// Idles for some time.
			float timer = 0;
			while(timer < health.invincibilityTime * invincibilityMultiplier) 
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			_colliders.SetActive(true); // Re-enables Boss colliders.

		}

		protected override void Die() 
		{

			// WORKAROUND ===================================================================================
			// Workaround used to stop Boss sounds played using PlayAudioDelayed, may not be necessary if a better fix is implemented.
			// Stops all Boss sounds.
			AudioController.instance.StopWithTag(BossScript.instance.id);
			// WORKAROUND ===================================================================================

			// Adds the Boss to the lsit of defeated Bosses and sets the game state.
			GameController.instance.defeatedBosses.Add(id);
			GameController.instance.currentGameState = GameController.GameState.Win;

			Debug.Log("BossScript.Die: " + transform.name + " has been defeated! (" + GameController.instance.defeatedBosses.Count + " different Bosses defeated)");

			// Stops coroutines and despawns projectiles.
			StopAllCoroutines();
			PoolingController.instance.DespawnBossObjects();
			
			// Sets the animation.
			if(_animator != null)
				_animator.Play("Die", 0);

			// Disables the HUD.
			if(GameController.instance.uiController != null)
				GameController.instance.uiController.DisableHUD();
			else
				Debug.LogWarning("BossScript.Die: No UIController found!");

		}

		// Called by the player when it's defeated by the Boss.
		public void PlayerDefeated()
		{

			// WORKAROUND ===================================================================================
			// Workaround used to stop Boss sounds played using PlayAudioDelayed, may not be necessary if a better fix is implemented.
			// Stops all Boss sounds.
			AudioController.instance.StopWithTag(BossScript.instance.id);
			// WORKAROUND ===================================================================================

			// Sets the game state.
			GameController.instance.currentGameState = GameController.GameState.Lose;

			// Stops coroutines and despawns projectiles.
			StopAllCoroutines();
			PoolingController.instance.DespawnBossObjects();
			
			// Sets the animation.
			if(_animator != null)
				_animator.Play("Win", 0);

			// Disables the HUD.
			if(GameController.instance.uiController != null)
				GameController.instance.uiController.DisableHUD();
			else
				Debug.LogWarning("BossScript.Die: No UIController found!");

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
			if(AudioController.instance != null)
				AudioController.instance.StopWithTag(id);

			// Destroys the pools generated by this Boss (must be marked with this script Boss "id").
			if(PoolingController.instance != null)	
				PoolingController.instance.DestroyWithTag(id);

		}
	}

}
