using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	// Projectile that accelerates targets the Player position and then stays for a time where it hit it's destination. 
	[AddComponentMenu("Scripts/Projectiles/Boss/Stay")]
	public class BossStayProjectile : ProjectileBase {

		[Tooltip("Projectile Rigidbody2D.")]
		[SerializeField] private Rigidbody2D _rigidbody = null;

		[Tooltip("Projectile Animator.")]
		[SerializeField] private Animator _animator = null;

		[Tooltip("Delay for the projectile to appear.")]
		[SerializeField] private float spawnDelay = 1.0f;

		[Tooltip("Amount of time the projectile shoud stay on the screen after hitting it's destination.")]
		[SerializeField] private float stay = 5.0f;
		
		[Tooltip("Delay for the projectile to disappear.")]
		[SerializeField] private float despawnDelay = 1.0f;

		[Tooltip("Projectile aceleration.")]
		[SerializeField] private float aceleration = 10.0f;

		[Tooltip("Audio to be played when the projectile hits it's destination..")]
		[SerializeField] private AudioInfo hitAudio = null;

		// Stores if the projectile has hit it's destination.
		private bool reachedTarget = false;

		// Stores the target position for the projectile.
		private Vector3 targetPosition;

		protected override void OnEnable ()
		{

			base.OnEnable();

			targetPosition = new Vector3(99, 99, 99);
			reachedTarget = false;

			// Gets the player as a target.
			GameObject target = null;
			if(PlayerScript.instance != null)
				target = PlayerScript.instance.gameObject;
				
			if(target == null)
				Debug.LogWarning("BossFollowProjectile.OnEnable: Player not found!");

			StartCoroutine(Follow(target));

		}

		protected override void FixedUpdate () 
		{

			// If the projectile goes out of bounds it should remain on the place it reached the screen edge.
			if(Mathf.Abs(transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(transform.position.y) > SceneSettings.instance.positionConstraints.y)
				reachedTarget = true;
			
		}

		protected IEnumerator Follow(GameObject target)
		{
			
			// Initializes the animation parameters.
			if(_animator != null) _animator.SetInteger("Stay", 0);
			if(_animator != null) _animator.SetInteger("Despawn", 0);
			
			// Waits for the delay.
			float timer = 0;
			while(timer < spawnDelay)
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			// Gets the target position and calculates the direction to go.
			targetPosition = target.transform.position;		
			Vector3 dirVet = (targetPosition - transform.position).normalized;

			// Acelerates the projectile until it reachs it's target.
			while(!reachedTarget)
			{

				// Checks if the projectile has reached it's target.
				if(Vector3.Distance(transform.position, targetPosition) < 0.5f)
				{
					reachedTarget = true;
					break;
				}

				if(GameController.instance.currentGameState != GameController.GameState.Paused)
					_rigidbody.AddForce( _rigidbody.mass * aceleration * dirVet, ForceMode2D.Force);

				yield return new WaitForFixedUpdate();

			}

			// Stops the proejctile.
			_rigidbody.velocity = Vector3.zero;

			// Plays the stay animation and the hit audio.
			if(_animator != null) _animator.SetInteger("Stay", 1);
			if(hitAudio != null) AudioController.instance.Play(hitAudio);

			// Waits for the delay.
			timer = 0;
			while(timer < stay)
			{

				timer += Time.fixedDeltaTime;

				yield return new WaitForFixedUpdate();
			}

			// Plays the despawn animation.
			if(_animator != null) _animator.SetInteger("Despawn", 1);

			// Waits for the despawn delay.
			timer = 0;
			while(timer < despawnDelay)
			{

				timer += Time.fixedDeltaTime;

				yield return new WaitForFixedUpdate();
			}

			Despawn();

		}

		protected override void OnCollisionEnter2D(Collision2D collision)
		{

			reachedTarget = true;

		}
	}

}
