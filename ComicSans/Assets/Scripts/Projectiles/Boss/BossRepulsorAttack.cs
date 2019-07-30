using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Boss;
using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Repulsor Attack")]
	public class BossRepulsorAttack : ProjectileBase {

		[SerializeField] private float delay = 1f;
		[SerializeField] private float strength = 3.5f;	
		[SerializeField] private float duration = 9f;

		[SerializeField] private GameObject damageObject = null;
		[SerializeField] private GameObject predictObject = null;

		protected override void OnEnable () {
			
			if(predictObject != null) predictObject.SetActive(true);
			if(damageObject != null) damageObject.SetActive(false);
			
			if(BossScript.instance == null)
			{
				Debug.Log("BossRepulsorAttack.OnEnable: BossScript instance not found!");
				return;
			}

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

			if(PlayerScript.instance != null)
			{

				Transform playerTransform = PlayerScript.instance.transform;

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

}
