using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Pooling/Pooled Object")]
public class PooledObject : MonoBehaviour {

	public ObjectPool origin;

	public void Despawn () {

		gameObject.SetActive(false);
		origin.Pool.Add(gameObject); 

	}
}
