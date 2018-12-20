using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Base")]
public class ProjectileBase : PooledObject {

	public Vector2 positionConstraints = new Vector2( 8, 8);

	protected virtual void OnEnable () {
		
		StartCoroutine(ConstraintBullet());

	}

	protected IEnumerator ConstraintBullet () {

        while(true) {

            if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
                if(origin != null)
					Despawn();
				else
					Destroy(this.gameObject);

            yield return new WaitForSeconds(0.25f);
        }
    }

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{

		if(origin != null)
			Despawn();
		else
			Destroy(this.gameObject);

	}
}
