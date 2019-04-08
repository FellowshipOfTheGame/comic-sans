using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Appear Projectile")]
public class BossAppearAttack : PooledObject {

	[SerializeField] private float appearDelay;
	[SerializeField] private float disappearDelay;
	[SerializeField] private Collider2D _collider;

	public AudioInfo projectileAudio;

	private float timer;

	void OnEnable()
	{

		_collider.enabled = false;
		timer = 0;

	}

	void Update ()
	{
		
		if(!_collider.enabled)
		{
			timer += Time.deltaTime;
			if(timer >= appearDelay)
			{
				_collider.enabled = true;
				timer = 0;
				if(projectileAudio != null)
					AudioController.instance.Play(projectileAudio);
			}
		}
		else
		{
			timer += Time.deltaTime;
			if(timer >= disappearDelay)
				if(origin != null)
					Despawn();
				else
					Destroy(this.gameObject);
			
		}
	}
}
