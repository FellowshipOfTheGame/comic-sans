using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Appear Projectile")]
public class BossAppearProjectile : MonoBehaviour {

	[SerializeField] private float appearDelay;
	[SerializeField] private float disappearDelay;
	[SerializeField] private Collider2D _collider;

	private float timer;

	private void Awake()
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
				Destroy(gameObject);
			
		}
	}
}
