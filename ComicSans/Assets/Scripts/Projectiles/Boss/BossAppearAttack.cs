using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Appear")]
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

		if(_collider != null)
			_collider.enabled = false;
		timer = 0;

	}

	protected override void FixedUpdate ()
	{
		
		base.FixedUpdate();

		if(_collider != null && !_collider.enabled)
		{
			timer += Time.fixedDeltaTime;
			if(timer >= appearDelay)
			{
				_collider.enabled = true;
				timer = 0;
			}
		}
		else
		{
			if(timer == 0 && projectileAudio != null)
				AudioController.instance.Play(projectileAudio);

			timer += Time.fixedDeltaTime;
			if(timer >= disappearDelay)
					Despawn();			
		}
	}
}
