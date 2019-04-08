using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Base")]
public class ProjectileBase : PooledObject {

	public Vector2 positionConstraints = new Vector2( 8, 8);

	public AudioInfo projectileAudio;

	protected virtual void OnEnable () 
	{

		if(projectileAudio != null)
			AudioController.instance.Play(projectileAudio);

	}

	protected virtual void FixedUpdate () 
	{

		if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
			Despawn();
        
    }

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{

		Despawn();

	}
}
