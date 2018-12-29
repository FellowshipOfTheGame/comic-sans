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
		if(Player_Manager.manager != null)
			target = Player_Manager.manager.gameObject;
			
		if(target != null)
			StartCoroutine(Follow(target.transform));
		else
			Debug.LogWarning("(BossFollowProjectile) Player not found!");

	}


	IEnumerator Follow(Transform target)
	{

		Coroutine lookAtCoroutine = StartCoroutine(LookAt(target));
		yield return new WaitForSeconds(delay);
		StopCoroutine(lookAtCoroutine);
 
		while(true)
		{

			_rigidbody.AddForce( _rigidbody.mass * aceleration * -transform.up, ForceMode2D.Force);

			yield return new WaitForEndOfFrame();
		}

	}

	IEnumerator LookAt(Transform target)
	{

		while (target != null)
		{

			Vector3 diff = target.position - transform.position;
         	diff.Normalize();
 
         	float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
         	transform.rotation = Quaternion.Euler(0f, 0f, 90 + rot_z);

			yield return new WaitForEndOfFrame();

		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{

		StopAllCoroutines();
		base.OnCollisionEnter2D(collision);

	}
}
