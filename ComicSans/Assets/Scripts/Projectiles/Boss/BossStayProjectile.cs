using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Stay")]
	public class BossStayProjectile : ProjectileBase {

		[SerializeField] private Rigidbody2D _rigidbody = null;

		[SerializeField] private Animator _animator = null;

		[SerializeField] private float spawnDelay = 1.0f;

		[SerializeField] private float stay = 5.0f;
		
		[SerializeField] private float despawnDelay = 1.0f;

		[SerializeField] private float aceleration = 10.0f;

		[SerializeField] private AudioInfo hitAudio = null;


		private bool reachedTarget = false;

		private Vector3 targetPosition;

		protected override void OnEnable ()
		{

			base.OnEnable();

			targetPosition = new Vector3(99, 99, 99);
			reachedTarget = false;

			GameObject target = null;
			if(PlayerScript.instance != null)
				target = PlayerScript.instance.gameObject;
				
			if(target == null)
				Debug.LogWarning("BossFollowProjectile.OnEnable: Player not found!");

			StartCoroutine(Follow(target));

		}

		protected override void FixedUpdate () 
		{

			if(Mathf.Abs(transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(transform.position.y) > SceneSettings.instance.positionConstraints.y)
				reachedTarget = true;
			
		}

		protected IEnumerator Follow(GameObject target)
		{
			
			if(_animator != null) _animator.SetInteger("Stay", 0);
			if(_animator != null) _animator.SetInteger("Despawn", 0);
			
			float timer = 0;
			while(timer < spawnDelay)
			{
				timer += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			targetPosition = target.transform.position;		
			Vector3 dirVet = (targetPosition - transform.position).normalized;

			while(!reachedTarget)
			{

				if(Vector3.Distance(transform.position, targetPosition) < 1f)
					reachedTarget = true;

				if(GameController.instance.currentGameState != GameController.GameState.Paused)
					_rigidbody.AddForce( _rigidbody.mass * aceleration * dirVet, ForceMode2D.Force);

				yield return new WaitForFixedUpdate();

			}

			_rigidbody.velocity = Vector3.zero;

			if(_animator != null) _animator.SetInteger("Stay", 1);
			if(hitAudio != null) AudioController.instance.Play(hitAudio);

			timer = 0;
			while(timer < stay)
			{

				timer += Time.fixedDeltaTime;

				yield return new WaitForFixedUpdate();
			}

			if(_animator != null) _animator.SetInteger("Despawn", 1);

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
