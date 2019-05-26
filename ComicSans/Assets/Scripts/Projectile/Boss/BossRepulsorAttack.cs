using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Repulsor Attack")]
public class BossRepulsorAttack : BossProjectileBase {

	[SerializeField] private float delay = 1f;
	[SerializeField] private float strength = 3.5f;	
	[SerializeField] private float duration = 9f;

	[SerializeField] private GameObject damageObject;
	[SerializeField] private GameObject predictObject;

	protected override void OnEnable () {
		
		if(predictObject != null) predictObject.SetActive(true);
		if(damageObject != null) damageObject.SetActive(false);
		
		if(BossScript.instance == null)
        {
            Debug.Log("BossRepulsorAttack.OnEnable: BossScript instance not found!");
            return;
        }

        BossScript.instance.DespawnBossProjectiles += Despawn;

		if(projectileAudio != null)
			AudioController.instance.Play(projectileAudio, delay);
			
		StartCoroutine(Repulse(strength));

	}

	IEnumerator Repulse(float strength) {

		float time = 0;

		while(time < delay) {

			time += Time.fixedDeltaTime;

			yield return new WaitForFixedUpdate();

		}

		if(predictObject != null) predictObject.SetActive(false);

		time = 0;

		if(damageObject != null) damageObject.SetActive(true);

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
