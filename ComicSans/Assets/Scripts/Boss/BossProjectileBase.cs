using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Base")]
public class BossProjectileBase : PooledObject
{

    public Vector2 positionConstraints = new Vector2( 8, 8);

	public AudioInfo projectileAudio;

    protected virtual void OnEnable () 
	{

        if(BossScript.instance == null)
        {
            Debug.Log("BossProjectileBase.OnEnable: BossScript instance not found!");
            return;
        }

        BossScript.instance.DespawnBossProjectiles += Despawn;

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

    public override void Despawn()
    {

        BossScript.instance.DespawnBossProjectiles -= Despawn;
        base.Despawn();

    }

}
