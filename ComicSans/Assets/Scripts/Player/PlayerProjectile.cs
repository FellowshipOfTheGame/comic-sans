using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Player/Projectile")]
public class PlayerProjectile : ProjectileBase {

	[SerializeField] private float velocity;
	[SerializeField] private Rigidbody2D _rigidbody;

	protected override void OnEnable () {

		base.OnEnable();
		if(_rigidbody != null)
			_rigidbody.velocity = Vector3.up * velocity;
		else
			Debug.Log("(PlayerProjectile) " + gameObject.name + " needs to have a Ridibody2D!");

	}

}