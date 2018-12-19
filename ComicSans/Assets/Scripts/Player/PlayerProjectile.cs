using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : ProjectileBase {

	[SerializeField] private float velocity;

	// Use this for initialization
	protected override void Awake () {
		
		base.Awake();
	
		Rigidbody2D _rigidbody = GetComponent<Rigidbody2D>();
		_rigidbody.velocity = Vector3.up * velocity;

	}
}