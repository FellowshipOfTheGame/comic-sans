using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Boss;
using ComicSans.Player;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Repulsor Attack")]
	public class BossRepulsorAttack : ProjectileBase {

		[Tooltip("Delay before starting repulsing.")]
		[SerializeField] private float delay = 1f;

		[Tooltip("Strenght of the repulsion.")]
		[SerializeField] private float strength = 3.5f;	

		[Tooltip("Duration of the repulsion.")]
		[SerializeField] private float duration = 9f;

		[Tooltip("GameObject that will cause damage during the repulsion.")]
		[SerializeField] private GameObject damageObject = null;

		[Tooltip("GameObject used before the repulsion start to warn where the Player will take damage.")]
		[SerializeField] private GameObject predictObject = null;

		protected override void OnEnable () {
			
			// Leaves the predictObject active at start.
			if(predictObject != null) predictObject.SetActive(true);
			if(damageObject != null) damageObject.SetActive(false);
			
			if(BossScript.instance == null)
			{
				Debug.Log("BossRepulsorAttack.OnEnable: BossScript instance not found!");
				return;
			}

			// Plays the audio after the delay.
			if(projectileAudio != null)
				AudioController.instance.Play(projectileAudio, delay);
				
			StartCoroutine(Repulse(strength));

		}

		IEnumerator Repulse(float strength) {


			// Waits for the delay.
			float time = 0;
			while(time < delay) {

				time += Time.fixedDeltaTime;

				yield return new WaitForFixedUpdate();

			}

			// Disables the predictObject and enables the damageObject.
			if(predictObject != null) predictObject.SetActive(false);
			if(damageObject != null) damageObject.SetActive(true);

			// Checks if the Player exists.
			if(PlayerScript.instance != null)
			{

				// Gets the Player transform.
				Transform playerTransform = PlayerScript.instance.transform;

				// Repulses for the duration time.
				time = 0;
				while(time < duration) 
				{

					// Calculates and applies the repulsion force.
					Vector2 vet = playerTransform.position - transform.position;
					vet = vet.normalized;

					playerTransform.transform.Translate(vet * strength * Time.deltaTime);

					time += Time.fixedDeltaTime;

					yield return new WaitForFixedUpdate();

				}
			}
			else
			{
				Debug.LogWarning("BossRepulsorAttack.Repulse: Player not found!");
			}

			
			Despawn();

		}
	}

}
