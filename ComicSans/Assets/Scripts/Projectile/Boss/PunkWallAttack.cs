using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Punk/Wall Attack")]
public class PunkWallAttack : ProjectileBase {

	[SerializeField] private float duration = 8.0f;

	protected override void OnEnable()
	{

		base.OnEnable();

		StartCoroutine(Disappear(duration));

	}

	IEnumerator Disappear(float duration)
	{

		float time = 0;
		while(time < duration)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		if(origin != null)
			Despawn();
		else
			Destroy(gameObject);

	}

}
