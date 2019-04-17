using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Appear Projectile")]
public class BossAppearAttack : BossProjectileBase {

	[SerializeField] private float appearDelay;
	[SerializeField] private float disappearDelay;
	[SerializeField] private Collider2D _collider;

	private float timer;

	protected override void OnEnable()
	{

		if(BossScript.instance == null)
        {
            Debug.Log("BossProjectileBase.OnEnable: BossScript instance not found!");
            return;
        }

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
