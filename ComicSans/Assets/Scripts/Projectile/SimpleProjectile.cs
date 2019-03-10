using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Simple")]
public class SimpleProjectile : ProjectileBase {

	[SerializeField] private float velocity = 5f;
	[SerializeField] private float delayToAppear = 0f;

	private Rigidbody2D _rigidbody;
	private SpriteRenderer _renderer;

	protected override void OnEnable () 
	{

		base.OnEnable();

		_rigidbody = gameObject.GetComponentInChildren<Rigidbody2D>();
		_renderer = gameObject.GetComponentInChildren<SpriteRenderer>();

		if(_rigidbody == null)
		{
			Debug.Log("SimpleProjectile: " + gameObject.name + " needs to have a Ridibody2D!");

			if(origin != null)
					Despawn();
				else
					Destroy(this.gameObject);
		}

		if(_renderer == null)
		{
			Debug.Log("SimpleProjectile: " + gameObject.name + " needs to have a SpriteRenderer!");

			if(origin != null)
					Despawn();
				else
					Destroy(this.gameObject);
		}

		_renderer.enabled = false;
		if(origin != null)
			transform.SetParent(origin.transform);

		StartCoroutine(Shot());

	}

	IEnumerator Shot()
	{

		float time = 0;

		while(time < delayToAppear)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
		}

		_renderer.enabled = true;
		_rigidbody.velocity = Vector3.up * velocity;

		if(origin != null)
			transform.SetParent(null);

	}

}