﻿using UnityEngine;

using System.Collections;

namespace ComicSans.Projectiles
{

	[AddComponentMenu("Scripts/Projectiles/Simple")]
	public class SimpleProjectile : ProjectileBase {

		[Tooltip("Base velocity of the projectile.")]
		[SerializeField] protected float velocity = 5f;
		[Tooltip("Delay before the projectile starts moving.")]
		[SerializeField] protected float delayToAppear = 0f;

		protected Rigidbody2D _rigidbody;
		protected SpriteRenderer _renderer;

		protected override void OnEnable () 
		{

			base.OnEnable();

			// Gets the Rigidbody2D of the projectile.
			_rigidbody = gameObject.GetComponentInChildren<Rigidbody2D>();
			if(_rigidbody == null)
			{
				Debug.Log("SimpleProjectile: " + gameObject.name + " needs to have a Ridibody2D!");
				Despawn();
				return;
			}

			// Gets the SpriteRenderer of the projectile.
			_renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
			if(_renderer == null)
			{
				Debug.Log("SimpleProjectile: " + gameObject.name + " needs to have a SpriteRenderer!");
				Despawn();
				return;
			}

			// Disables the projectile renderer at start.
			_renderer.enabled = false;

			StartCoroutine(Shot());

		}

		// Moves the projectile.
		IEnumerator Shot()
		{	

			// Waits for the delay.
			float time = 0;
			while(time < delayToAppear)
			{
				yield return new WaitForFixedUpdate();
				time += Time.fixedDeltaTime;
			}

			// Enables the projectile renderer.
			_renderer.enabled = true;

			// Moves the projectile.
			_rigidbody.velocity = transform.up * velocity;

		}
	}

}