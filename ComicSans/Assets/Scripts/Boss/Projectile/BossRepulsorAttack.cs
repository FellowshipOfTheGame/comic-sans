using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Repulsor Attack")]
public class BossRepulsorAttack : ProjectileBase {

	[SerializeField] private float strenght;	
	[SerializeField] private float duration;

	protected override void OnEnable () {
		
		base.OnEnable();
		StartCoroutine(Repulse(strenght));

	}

	IEnumerator Repulse(float strenght) {

		float time = 0;

		if(Player.instance != null)
		{

			Transform playerTransform = Player.instance.transform;

			while(time < duration) 
			{
				time += Time.fixedDeltaTime;

				if(playerTransform == null)
					break;

				Vector2 vet = playerTransform.position - transform.position;
				vet = vet.normalized;

				playerTransform.transform.Translate(vet * strenght * Time.deltaTime);

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
