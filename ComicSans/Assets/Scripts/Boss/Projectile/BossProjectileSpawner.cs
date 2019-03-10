﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Projectile Spawner")]
public class BossProjectileSpawner : PooledObject {

	[SerializeField] private int numberOfAttack;
	[SerializeField] private float delay;
	[SerializeField] private bool delayAtStart;

	private int currentAttack;
	private float timer = 0;

	[SerializeField] ObjectPool projectilePool;

	enum SpawnType { SinglePosition, MultiplePosition, PlayerPosition }
	[SerializeField] private SpawnType spawnType;
	[SerializeField] private Vector3[] spawnPositions;

	void OnEnable () 
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
			 projectilePool.Spawn(spawnPositions[0], new Quaternion());
		} 
		else if(spawnType == SpawnType.MultiplePosition) 
		{
			projectilePool.Spawn(spawnPositions[currentAttack], new Quaternion());
		}
		else
		{
				
			if(Player.instance != null)
				projectilePool.Spawn(Player.instance.transform.position, new Quaternion());
			else
				Debug.LogWarning("ProjectileSpawner.Attack: Player not found!");
		}

	}
}
