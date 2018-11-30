﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Follow Projectile")]
public class BossFollowProjectile : BossProjectile {

	public Rigidbody2D _rigidbody;
	public float delay = 1.0f;

	public float aceleration = 10.0f;

	void Start ()
	{

		GameObject target = GameObject.FindGameObjectWithTag("Player");
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

		while (true)
		{

			Vector3 diff = target.position - transform.position;
         	diff.Normalize();
 
         	float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
         	transform.rotation = Quaternion.Euler(0f, 0f, 90 + rot_z);

			yield return new WaitForEndOfFrame();

		}

	}

}
