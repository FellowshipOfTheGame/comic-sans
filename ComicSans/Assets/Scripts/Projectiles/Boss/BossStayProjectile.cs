﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Stay")]
public class BossStayProjectile : ProjectileBase {

	public Rigidbody2D _rigidbody;

	public Animator _animator;

	public float delay = 1.0f;

	public float stay = 5.0f;

	public float aceleration = 10.0f;

	private bool reachedTarget = false;

	private Vector3 targetPosition;

	protected override void OnEnable ()
	{

		base.OnEnable();

		GameObject target = null;
		if(PlayerScript.instance != null)
			target = PlayerScript.instance.gameObject;
			
		if(target == null)
			Debug.LogWarning("BossFollowProjectile.OnEnable: Player not found!");

		targetPosition = new Vector3(99, 99, 99);
		StartCoroutine(Follow(target));

	}

	private void Update()
	{

		if(Vector3.Distance(transform.position, targetPosition) < 1f)
			reachedTarget = true;
	}

	protected IEnumerator Follow(GameObject target)
	{
		
		if(_animator != null) _animator.SetInteger("Stay", 0);
		
		float timer = 0;
		while(timer < delay)
		{
			timer += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		targetPosition = target.transform.position;		
		Vector3 dirVet = (targetPosition - transform.position).normalized;

		while(!reachedTarget)
		{

			if(GameController.instance.currentGameState != GameController.GameState.Paused)
				_rigidbody.AddForce( _rigidbody.mass * aceleration * dirVet, ForceMode2D.Force);

			yield return new WaitForFixedUpdate();

		}

		_rigidbody.velocity = Vector3.zero;

		if(_animator != null) _animator.SetInteger("Stay", 1);

		timer = 0;
		while(timer < stay)
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