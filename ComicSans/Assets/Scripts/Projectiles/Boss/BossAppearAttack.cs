using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Boss;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Appear")]
	public class BossAppearAttack : ProjectileBase {

		[Tooltip("Delay for the projectile to appear.")]
		[SerializeField] private float appearDelay = 0.4f;

		[Tooltip("Delay for the projectile to disappear.")]
		[SerializeField] private float disappearDelay = 0.6f;

		[Tooltip("Collider for the projectile.")]
		[SerializeField] private Collider2D _collider = null;

		// Used to count the time.
		private float timer;

		protected override void OnEnable()
		{

			if(BossScript.instance == null)
			{
				Debug.Log("BossAppearAttack.OnEnable: BossScript instance not found!");
				return;
			}

			if(_collider != null)
				_collider.enabled = false;
			timer = 0;

		}

		protected override void FixedUpdate ()
		{
			
			base.FixedUpdate();

			if(_collider != null && !_collider.enabled)
			{
				
				// Counts the delay for the projectile to appear.
				timer += Time.fixedDeltaTime;
				if(timer >= appearDelay)
				{
					_collider.enabled = true;
					timer = 0;
				}
				
			}
			else
			{

				// Counts the delay for the projectile to disappear.
				if(timer == 0 && projectileAudio != null)
					AudioController.instance.Play(projectileAudio);

				timer += Time.fixedDeltaTime;
				if(timer >= disappearDelay)
						Despawn();	

			}
		}
	}

}
