using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour {

	public Vector2 positionConstraints = new Vector2( 8, 8);

	protected virtual void Awake () {
		
		StartCoroutine(ConstraintBullet());

	}

	IEnumerator ConstraintBullet () {

        while(true) {

            if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
                Destroy(gameObject);

            yield return new WaitForSeconds(0.25f);
        }
    }

	public virtual void OnCollisionEnter2D(Collision2D collision)
	{

		Destroy(this.gameObject);

	}
}
