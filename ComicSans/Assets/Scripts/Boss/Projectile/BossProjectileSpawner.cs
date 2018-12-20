using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Boss/Projectile Spawner")]
public class BossProjectileSpawner : PooledObject {

	[SerializeField] private int numberOfAttack;
	[SerializeField] private float delay;

	private int currentAttack;
	private float timer;

	[SerializeField] GameObject projectile;

	enum SpawnType { SinglePosition, MultiplePosition, PlayerPosition }
	[SerializeField] private SpawnType spawnType;
	[SerializeField] private Vector3[] spawnPositions;

	private void Awake () 
	{

		currentAttack = 0;
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
			}

			timer += Time.deltaTime;

		} 
		else
		{
			Destroy(gameObject);
		}
	}

	private void Attack() {
		
		if(spawnType == SpawnType.SinglePosition) 
		{
			Instantiate(projectile, spawnPositions[0], new Quaternion());
		} 
		else if(spawnType == SpawnType.MultiplePosition) 
		{
			Instantiate(projectile, spawnPositions[currentAttack], new Quaternion());
		}
		else
		{
			GameObject _player = GameObject.FindGameObjectWithTag("Player");
			if(_player != null)
				Instantiate(projectile, _player.transform.position, new Quaternion());
			else
				Debug.LogWarning("(ProjectileSpawner) Player not found!");
		}

	}
}
