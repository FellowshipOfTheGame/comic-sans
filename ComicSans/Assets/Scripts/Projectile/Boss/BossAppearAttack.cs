using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Appear Projectile")]
public class BossAppearAttack : ProjectileBase {

	[SerializeField] private float appearDelay = 0.4f;
	[SerializeField] private float disappearDelay = 0.6f;
	[SerializeField] private Collider2D _collider = null;

	private float timer;

	protected override void OnEnable()
	{

		if(BossScript.instance == null)
        {
            Debug.Log("BossAppearAttack.OnEnable: BossScript instance not found!");
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
