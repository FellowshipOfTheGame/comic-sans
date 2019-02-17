using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Follow Projectile")]
public class BossFollowProjectile : ProjectileBase {

	public Rigidbody2D _rigidbody;
	public float delay = 1.0f;

	public float aceleration = 10.0f;

	protected override void OnEnable ()
	{

		base.OnEnable();

		GameObject target = null;
		if(Player.instance != null)
			target = Player.instance.gameObject;
			
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
			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		if(lookAtCoroutine != null)
			StopCoroutine(lookAtCoroutine);
 
		while(true)
		{

			_rigidbody.AddForce( _rigidbody.mass * aceleration * -transform.up, ForceMode2D.Force);

			yield return new WaitForEndOfFrame();
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
