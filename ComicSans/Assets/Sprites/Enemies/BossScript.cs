using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Boss")]
public class BossScript : MonoBehaviour {

	public int life = 100;

	public float velocity = 4.0f;

	public BossPhase[] phases;
	private int currentPhase = 0;

	// Used to store where inside the current pattern the boss is.
	private int currentPhasePattern = 0;
	private int currentMovement = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	//To do: Make boos go to the movement target position in a straight line (using sin and cos to give the correct velocity).

	// Update is called once per frame
	void Update () {
		
		Vector2 position_2D = new Vector2(transform.position.x, transform.position.y);

		if(phases[currentPhase].actionPatterns[currentPhasePattern].patternType == BossPattern.Type.Movement) {

			// Cheks if the boss has arrived on the target position.
			if(Vector2.Distance(position_2D, phases[currentPhase].actionPatterns[currentPhasePattern].movement[currentMovement].positionTarget) == 0)
				currentMovement++;

			// Goes to the next movement pattern.
			if(currentMovement >= phases[currentPhase].actionPatterns[currentPhasePattern].movement.Length) {
				currentMovement = 0;
				currentPhasePattern++;
			}

			// Restart this phase movement patterns if there are no more patterns avaliable.
			if(currentPhasePattern >= phases[currentPhase].actionPatterns.Length)
				currentPhasePattern = 0;

			// Moves to the target position.
			Movement(phases[currentPhase].actionPatterns[currentPhasePattern].movement[currentMovement]);

		} else if(phases[currentPhase].actionPatterns[currentPhasePattern].patternType == BossPattern.Type.Attack) {

			// Realizes an attack.
			Attack(phases[currentPhase].actionPatterns[currentPhasePattern].attack);
			currentPhasePattern++;

			// Restart this phase movement patterns if there are no more patterns avaliable.
			if(currentPhasePattern >= phases[currentPhase].actionPatterns.Length)
				currentPhasePattern = 0;

		} else if(phases[currentPhase].actionPatterns[currentPhasePattern].patternType == BossPattern.Type.Mix) {

			// Cheks if the boss has arrived on the target position.
			if(Vector2.Distance(position_2D, phases[currentPhase].actionPatterns[currentPhasePattern].movement[currentMovement].positionTarget) <= 0.05f)
				currentMovement++;

			// Goes to the next movement pattern.
			if(currentMovement == phases[currentPhase].actionPatterns[currentPhasePattern].movement.Length) {

				currentMovement = 0;
				// Realizes an attack.
				Attack(phases[currentPhase].actionPatterns[currentPhasePattern].attack);
				currentPhasePattern++;

			} else {

				// Moves to the target position.
				Movement(phases[currentPhase].actionPatterns[currentPhasePattern].movement[currentMovement]);

			}

			// Restart this phase movement patterns if there are no more patterns avaliable.
			if(currentPhasePattern >= phases[currentPhase].actionPatterns.Length)
				currentPhasePattern = 0;

		} else
			Debug.LogError("(BossScript) Invalid action pattern type on " + gameObject.name + "!");

	}

	public void Movement(Movement movement) {

		if(transform.position.x > movement.positionTarget.x)
			transform.Translate( -velocity * movement.velocityModifier * Time.deltaTime, 0, 0);
		else if(transform.position.x < movement.positionTarget.x)
			transform.Translate( velocity * movement.velocityModifier * Time.deltaTime, 0, 0);

		// Clamps the x position if it's close enough.
		if(Mathf.Abs(transform.position.x - movement.positionTarget.x) < 0.05f)
			transform.position = new Vector2(movement.positionTarget.x, transform.position.y);

		if(transform.position.y > movement.positionTarget.y)
			transform.Translate( 0, -velocity * movement.velocityModifier * Time.deltaTime, 0);
		else if(transform.position.y < movement.positionTarget.y)
			transform.Translate( 0, velocity * movement.velocityModifier * Time.deltaTime, 0);

		// Clamps the y position if it's close enough.
		if(Mathf.Abs(transform.position.y - movement.positionTarget.y) < 0.05f)
			transform.position = new Vector2(transform.position.x,movement.positionTarget.y);

	}

	public void Attack (BossAttack attack) {

		for(int i = 0; i < attack.projectileSpawns.Length; i++) {

			Vector2 spawnPos = new Vector3(transform.position.x + attack.projectileSpawns[i].x, transform.position.y + attack.projectileSpawns[i].y, 0);
			Instantiate(attack.projectile, spawnPos, transform.rotation);

		}

	}

	public void Damage(int amount) {

		life-=amount;
		if(life <= 0)
			Die();

	}

	void Die() {

		Destroy(gameObject);

	}
}
