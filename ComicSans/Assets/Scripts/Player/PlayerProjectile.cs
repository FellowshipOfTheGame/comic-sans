using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : ProjectileBase {

	[SerializeField] private float velocity;

	protected override void Awake(){}

	private void OnEnable () {

		StartCoroutine(ConstraintBullet());

		Rigidbody2D _rigidbody = GetComponent<Rigidbody2D>();
		_rigidbody.velocity = Vector3.up * velocity;

	}

}