using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.PoolingSystem;

namespace ComicSans.Projectiles
{

	[AddComponentMenu("Scripts/Projectiles/Base")]
	public class ProjectileBase : PooledObject {

		[SerializeField] protected bool destroyOnContact = true;

		public AudioInfo projectileAudio;

		protected virtual void OnEnable () 
		{

			// Plays the audio at start.
			if(projectileAudio != null)
				AudioController.instance.Play(projectileAudio);

		}

		// Destroy the projectile if it gets out of Scene bounds.
		protected virtual void FixedUpdate () 
		{

			if(Mathf.Abs(transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(transform.position.y) > SceneSettings.instance.positionConstraints.y)
				Despawn();
			
		}

		// Destroy the projectile on collision if enabled.
		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{

			if(destroyOnContact)
				Despawn();

		}
	}

}
