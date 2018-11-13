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
	[HideInInspector]
	public Coroutine currentCoroutine;

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

		// Stop the curret coroutine if it exists.
		if(currentCoroutine != null)
		{
			StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		}

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

		while (true) {

			if(_animator != null)
				SetAnimation(movement.animations);

			Vector3 targetPosition = new Vector3(movement.positionTarget.x, movement.positionTarget.y, 0);

			// Moves to the target position.
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocity * movement.velocityModifier * Time.deltaTime);

			// Cheks if the boss has arrived on the target position.
			if(Vector3.Distance(targetPosition, transform.position) <= 0.05f)
				GetNewAction();

			yield return new WaitForSeconds(Time.deltaTime);
		}
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

		while (true) {

			if(Vector3.Distance(nextStep, transform.position) <= 0.05f)
			{
				currentStep++;
				nextStep = originalPos + ((targetPosition - originalPos) / attackMove.numberOfSteps) * currentStep;

				if(currentStep != 1)
				{
					if(_animator != null)
						SetAnimation(attackMove.idleAnimations);

					yield return new WaitForSeconds(attackMove.idleTime);

					if(_animator != null)
						SetAnimation(attackMove.attackAnimations);

					// Spawns all projectiles.
					foreach(Vector2 spawn in attackMove.projectileSpawns)
						Instantiate(attackMove.projectile, transform.position + new Vector3( spawn.x, spawn.y, 0), transform.rotation);

					yield return new WaitForSeconds(attackMove.idleTime);
				}

			} 
			else
			{
				if(_animator != null)
					SetAnimation(attackMove.movementAnimations);

				transform.position = Vector3.MoveTowards(transform.position, nextStep, velocity * attackMove.velocityModifier * Time.deltaTime);
			}

			yield return new WaitForSeconds(Time.deltaTime);

			if(Vector3.Distance(targetPosition, transform.position) <= 0.05f)
				GetNewAction();
		}
	}

	public void ActionIdle(BossIdle idle)
	{

		if(_animator != null)
			SetAnimation(idle.animations);

		Invoke("GetNewAction", idle.idleTime);
	}

	private void SetAnimation(List<AnimationSet> animations)
	{

		foreach (AnimationSet anim in animations)
			_animator.SetInteger(anim.name, anim.value);

	}

	public void Damage(int amount) 
	{

		Life-=amount;
		if(Life <= 0)
			Die();

	}

	private void Die() 
	{

		Destroy(gameObject);

	}
}
