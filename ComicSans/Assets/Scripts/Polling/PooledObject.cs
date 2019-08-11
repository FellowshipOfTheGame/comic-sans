using UnityEngine;
using UnityEngine.SceneManagement;

namespace ComicSans.PoolingSystem
{

	// Controls an object that can be pooled from the System.
	[AddComponentMenu("Scripts/Pooling/Pooled Object")]
	public class PooledObject : MonoBehaviour {

		// Origin Pool of this object.
		[HideInInspector] public ObjectPool origin;

		protected void Awake()
		{

			SceneManager.sceneLoaded += OnSceneLoaded;	// Adds a function to be called when a scene is loaded.

		}

		public virtual void Despawn () {

			// Guarantees this object has not been destroyed or deactivated before.
			if(this == null || !gameObject.activeSelf) return;

			if(origin != null)
			{
				gameObject.SetActive(false); // Disables the object.
				origin.Pool.Add(gameObject); // Re-adds it to the origin Pool.
			} 
			else
			{
				Debug.LogWarning("PooledObject.Despawn: \"" + transform.name + "\" has no origin and is being destroyed!");
				Destroy(gameObject);
			}

		}

		public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{

			Despawn(); // Despawns this gameObject when a new scene is loaded.

		}

		protected void OnDestroy()
		{

			// When the object is destroyed, removes it's Despawn function from the origin Pool list.
			if(origin != null)
				origin.DespawnPoolObjects -= Despawn;
			// When the object is destroyed, removes it's OnSceneLoaded from the SceneManager. 
			SceneManager.sceneLoaded -= OnSceneLoaded;	

		}
	}

}
