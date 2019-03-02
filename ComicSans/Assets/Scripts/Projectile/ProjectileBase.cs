using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Base")]
public class ProjectileBase : PooledObject {

	public Vector2 positionConstraints = new Vector2( 8, 8);

	public string audioName;

	protected virtual void OnEnable () 
	{

		if(audioName != null && audioName != "")
			AudioControlCenter.instance.Play(audioName);

	}

	protected virtual void FixedUpdate () 
	{

		if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
			if(origin != null)
				Despawn();
			else
				Destroy(this.gameObject);
        
    }

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{

		if(origin != null)
			Despawn();
		else
			Destroy(this.gameObject);

	}
}
