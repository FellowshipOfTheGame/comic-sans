using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Repulsor Attack")]
public class BossRepulsorAttack : BossProjectileBase {

	[SerializeField] private float strength;	
	[SerializeField] private float duration;

	protected override void OnEnable () {
		
		base.OnEnable();
		StartCoroutine(Repulse(strength));

	}

	IEnumerator Repulse(float strength) {

		float time = 0;

		if(Player.instance != null)
		{

			Transform playerTransform = Player.instance.transform;

			while(time < duration) 
			{

				if(BossScript.instance == null)
					break;

				time += Time.fixedDeltaTime;

				if(playerTransform == null)
					break;

				Vector2 vet = playerTransform.position - transform.position;
				vet = vet.normalized;

				playerTransform.transform.Translate(vet * strength * Time.deltaTime);

				yield return new WaitForFixedUpdate();

			}
		}
		else
		{
			Debug.Log("BossRepulsorAttack.Repulse: Player not found!");
		}

		if(origin != null)
			Despawn();
		else
			Destroy(this.gameObject);

	}
}
