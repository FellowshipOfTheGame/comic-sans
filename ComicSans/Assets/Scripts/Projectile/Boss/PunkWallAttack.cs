using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Punk/Wall Attack")]
public class PunkWallAttack : BossProjectileBase {

	[SerializeField] private float duration = 8.0f;
	[SerializeField] private AudioInfo attackAudio;

	void OnEnable()
	{

		if(AudioController.instance != null)
			AudioController.instance.Play(attackAudio);

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
