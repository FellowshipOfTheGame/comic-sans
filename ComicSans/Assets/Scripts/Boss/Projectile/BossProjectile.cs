using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour {

	public Vector2 positionConstraints = new Vector2( 8, 8);

	protected virtual void Update () {
		
		if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
			Destroy(this.gameObject);

	}

	public virtual void OnCollisionEnter2D(Collision2D collision)
	{

		if(collision.gameObject.tag == "Player")
			Destroy(this.gameObject);

	}
}
