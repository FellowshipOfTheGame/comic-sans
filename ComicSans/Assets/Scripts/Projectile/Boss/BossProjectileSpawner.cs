using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Projectile Spawner")]
public class BossProjectileSpawner : ProjectileBase {

	[SerializeField] private int numberOfAttack = 8;
	[SerializeField] private float delay = 0.35f;
	[SerializeField] private bool delayAtStart = false;

	private int currentAttack = 0;
	private float timer = 0;

	[SerializeField] PoolInfo projectilePool = null;

	enum SpawnType { SinglePosition, MultiplePosition, PlayerPosition }
	[SerializeField] private SpawnType spawnType = SpawnType.PlayerPosition;

	[SerializeField] private List<ProjectileSpawn> spawnPositions = null;

	protected override void OnEnable () 
	{

		currentAttack = 0;
		
		if(!delayAtStart)
			timer = delay + 1;

	}

	private void Update () {
		
		if(currentAttack < numberOfAttack) 
		{
			if(timer >= delay) 
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
		{
			if(origin != null)
				Despawn();
			else
				Destroy(this.gameObject);
		}
	}

	private void Attack() {
		
		if(spawnType == SpawnType.SinglePosition) 
		{
			PoolingController.instance.Spawn(projectilePool, spawnPositions[0].position, Quaternion.Euler(0, 0, spawnPositions[0].rotation));
		} 
		else if(spawnType == SpawnType.MultiplePosition) 
		{
			PoolingController.instance.Spawn(projectilePool, spawnPositions[currentAttack].position, Quaternion.Euler(0, 0, spawnPositions[currentAttack].rotation));
		}
		else
		{
				
			if(PlayerScript.instance != null)
				PoolingController.instance.Spawn(projectilePool, PlayerScript.instance.transform.position, new Quaternion());
			else
				Debug.LogWarning("ProjectileSpawner.Attack: Player not found!");
		}

	}
}
