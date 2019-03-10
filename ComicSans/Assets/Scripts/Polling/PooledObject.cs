using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Pooling/Pooled Object")]
public class PooledObject : MonoBehaviour {

	public ObjectPool origin;

	public virtual void Despawn () {

		if(origin != null)
		{
			gameObject.SetActive(false);
			origin.Pool.Add(gameObject);
		} else
			Destroy(gameObject);

	}

	public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(scene.name == "Menu")
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;	
			Destroy(gameObject);
			return;
		}

		Despawn();

	}
}
