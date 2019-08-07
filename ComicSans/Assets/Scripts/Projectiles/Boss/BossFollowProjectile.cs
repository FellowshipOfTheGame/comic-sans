using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Follow")]
	public class BossFollowProjectile : ProjectileBase {

		[Tooltip("RigidBody for the projecile.")]
		public Rigidbody2D _rigidbody;

		[Tooltip("Delay before the projectile starts moving.")]
		public float delay = 1.0f;

		[Tooltip("Aceleration for the projectile.")]
		public float aceleration = 10.0f;

		protected override void OnEnable ()
		{

			base.OnEnable();

			GameObject target = null; // Checks for the player as a target.
			if(PlayerScript.instance != null)
				target = PlayerScript.instance.gameObject;
				
			if(target == null)
				Debug.LogWarning("BossFollowProjectile.OnEnable: Player not found!");

			StartCoroutine(Follow(target)); // Starts targeting the Player.

		}


		IEnumerator Follow(GameObject target)
		{

			
			Coroutine lookAtCoroutine = null;
			
			if(target != null) // Looks in the direction of the target.
				lookAtCoroutine = StartCoroutine(LookAt(target));
			
			// Waits for the delay.
			float timer = 0;
			while(timer < delay)
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			// Stops the LookAt coroutine.
			if(lookAtCoroutine != null)
				StopCoroutine(lookAtCoroutine);
	
			while(true)
			{

				// Starts acelerating.
				if(GameController.instance.currentGameState != GameController.GameState.Paused)
					_rigidbody.AddForce( _rigidbody.mass * aceleration * -transform.up, ForceMode2D.Force);

				yield return new WaitForFixedUpdate();
			}

		}

		IEnumerator LookAt(GameObject target)
		{

			while (true)
			{

				if(target != null)
				{

					// Calculates and sets the correct angle to face the target.
					Vector3 diff = target.transform.position - transform.position;
					diff.Normalize();
		
					float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.Euler(0f, 0f, 90 + rot_z);

					yield return new WaitForEndOfFrame();

				} 
				else
				{
					Debug.LogWarning("BossFollowProjectile.LookAt: Player not found!");
					break;
				}

			}
		}

		protected override void OnCollisionEnter2D(Collision2D collision)
		{

			StopAllCoroutines();
			base.OnCollisionEnter2D(collision);

		}
	}

}
