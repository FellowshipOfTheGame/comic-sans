using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;
using ComicSans.PoolingSystem;
using ComicSans.DataContainers;

namespace ComicSans.Projectiles.Boss
{

	[AddComponentMenu("Scripts/Projectiles/Boss/Spawner")]
	public class BossProjectileSpawner : ProjectileBase {

		[Tooltip("Number of projectiles to be spawned.")]
		[SerializeField] private int numberOfAttack = 8;

		[Tooltip("Delay between each projectile.")]
		[SerializeField] private float delay = 0.35f;

		[Tooltip("Should the delay be used at the first projectile.")]
		[SerializeField] private bool delayAtStart = false;

		// Current attack number.
		private int currentAttack = 0;

		// Used to control the time between delays. 
		private float timer = 0;

		[SerializeField] PoolInfo projectilePool = null;

		enum SpawnType { SinglePosition, MultiplePosition, PlayerPosition, PlayerFacing }
		[SerializeField] private SpawnType spawnType = SpawnType.PlayerPosition;

		[SerializeField] private List<ProjectileSpawn> spawnPositions = null;

		protected override void OnEnable () 
		{

			currentAttack = 0;
			
			if(!delayAtStart) 		// If there is no delay at start sets time to a high value so the the first projectile.
				timer = delay + 1;  // appears immediatly.
			else
				timer = 0;

		}

		protected void Update () {

			if(currentAttack < numberOfAttack) // Verifies if all attacks have been performed.
			{
				if(timer >= delay) // Waits for the time to be greater than the delay.
				{
					
					Attack();

					currentAttack++;
					timer = 0;

					if(currentAttack > spawnPositions.Count)
						currentAttack = 0;
				}

				timer += Time.deltaTime;

			} 
			else
				Despawn();
			
		}

		private void Attack() {
			
			if(spawnType == SpawnType.SinglePosition) // Spawns a single projectile.
			{
				PoolingController.instance.Spawn(projectilePool, spawnPositions[0].position, Quaternion.Euler(0, 0, spawnPositions[0].rotation));
			} 
			else if(spawnType == SpawnType.MultiplePosition) // Spawns multiples projectiles.
			{
				PoolingController.instance.Spawn(projectilePool, spawnPositions[currentAttack].position, Quaternion.Euler(0, 0, spawnPositions[currentAttack].rotation));
			}
			else if (spawnType == SpawnType.PlayerPosition) // Spawns a projectile on the Player's position.
			{
					
				if(PlayerScript.instance != null)
					PoolingController.instance.Spawn(projectilePool, PlayerScript.instance.transform.position, new Quaternion());
				else
					Debug.LogWarning("ProjectileSpawner.Attack: Player not found!");
			}
			else if (spawnType == SpawnType.PlayerFacing) // Spawns a projectile facing the Player.
			{
					
				if(PlayerScript.instance != null)
				{

					// Note that for this case the relative position to this objec will be used for the spawn.
					Vector3 spawnPos = transform.position + new Vector3(spawnPositions[currentAttack].position.x, spawnPositions[currentAttack].position.y, 0);
					Vector3 diff = PlayerScript.instance.transform.position - spawnPos;

					float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
					Quaternion rotation = Quaternion.Euler(0f, 0f, 90 + rot_z + spawnPositions[currentAttack].rotation);

					PoolingController.instance.Spawn(projectilePool, spawnPos, rotation);

				}
				else
					Debug.LogWarning("ProjectileSpawner.Attack: Player not found!");
			}

		}
	}

}