using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;


namespace ComicSans.PoolingSystem
{

	// Controls an object that can be pooled from the System.
	[AddComponentMenu("Scripts/Pooling/Pooled Object")]
	public class PooledObject : MonoBehaviour {

		public ObjectPool origin;

		protected void Awake()
		{

			SceneManager.sceneLoaded += OnSceneLoaded;	

		}

		public virtual void Despawn () {

			if(this == null || !gameObject.activeSelf) return;

			if(origin != null)
			{
				gameObject.SetActive(false);
				origin.Pool.Add(gameObject);
			} 
			else
			{
				Debug.Log("PooledObject.Despawn: \"" + transform.name + "\" has no origin and is being destroyed!");
				Destroy(gameObject);
			}

		}

		public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{

			Despawn();

		}

		protected void OnDestroy()
		{

			if(origin != null)
				origin.DespawnPoolObjects -= Despawn;

			SceneManager.sceneLoaded -= OnSceneLoaded;	

		}
	}

}
