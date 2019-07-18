﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Base")]
public class ProjectileBase : PooledObject {

	[SerializeField] protected bool destroyOnContact = true;

	public AudioInfo projectileAudio;

	protected virtual void OnEnable () 
	{

		if(projectileAudio != null)
			AudioController.instance.Play(projectileAudio);

	}

	protected virtual void FixedUpdate () 
	{

		if(Mathf.Abs(transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(transform.position.y) > SceneSettings.instance.positionConstraints.y)
			Despawn();
        
    }

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{

		if(destroyOnContact)
			Despawn();

	}
}