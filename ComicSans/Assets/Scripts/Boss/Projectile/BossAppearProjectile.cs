using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Appear Projectile")]
public class BossAppearProjectile : PooledObject {

	[SerializeField] private float appearDelay;
	[SerializeField] private float disappearDelay;
	[SerializeField] private Collider2D _collider;

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
