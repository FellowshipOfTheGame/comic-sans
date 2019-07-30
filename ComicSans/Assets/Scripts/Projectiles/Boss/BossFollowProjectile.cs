using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Follow")]
	public class BossFollowProjectile : ProjectileBase {

		public Rigidbody2D _rigidbody;
		public float delay = 1.0f;

		public float aceleration = 10.0f;

		protected override void OnEnable ()
		{

			base.OnEnable();

			GameObject target = null;
			if(PlayerScript.instance != null)
				target = PlayerScript.instance.gameObject;
				
			if(target == null)
				Debug.LogWarning("BossFollowProjectile.OnEnable: Player not found!");

			StartCoroutine(Follow(target));

		}


		IEnumerator Follow(GameObject target)
		{

			
			Coroutine lookAtCoroutine = null;
			
			if(target != null)
				lookAtCoroutine = StartCoroutine(LookAt(target));
			
			float timer = 0;
			while(timer < delay)
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			if(lookAtCoroutine != null)
				StopCoroutine(lookAtCoroutine);
	
			while(true)
			{

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
