using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.PoolingSystem
{

	// Controls an Object Pool from which base Objects can be pooled.
	[AddComponentMenu("Scripts/Pooling/Object Pool")]
	public class ObjectPool : MonoBehaviour {

		// PoolInfo used by this pool, assigned by the PoolingController.
		[HideInInspector] public PoolInfo poolInfo;

		[Tooltip("Objects on the pool.")]
		public List<GameObject> Pool;

		[Tooltip("Number of objects instantiated by the pool.")]
		public int currentObjectInstances = 0;

		public delegate void DespawnPool();

		// Used to despawn the objects from the pool when the pool is destroyed..
		public DespawnPool DespawnPoolObjects;

		public void Initialize() 
		{

			// Initializes an empty list of objects.
			Pool = new List<GameObject>();
			currentObjectInstances = 0;

			// Creates the initial pool of objects.
			for(int i = 0; i < poolInfo.initialObjectInstances; i++) 
			{

				GameObject new_poll_obj = CreateNew(transform.position, transform.rotation); // Creates the object.

				if(new_poll_obj != null) // Adds it to the list.
					Pool.Add(new_poll_obj);
				else
				{
					Debug.LogError("ObjectPool.Initialize: Failed to instantiate a new instance of " + poolInfo.baseObject.name + "!");
					break;
				}

			}
		}

		// Spawns a object from the pool.
		// Version 1:
		public GameObject Spawn (Vector3 spawnPosition, Quaternion spawnRotation) 
		{
			
			if(Pool.Count > 0) 
			{

				GameObject pooled = Pool[0]; // Gets the first object on the Pool.

				// Assigs the object to the spawn position.
				pooled.transform.position = spawnPosition;
				pooled.transform.rotation = spawnRotation;

				// Activets the object.
				pooled.SetActive(true);

				// Removes the object from the Pool.
				Pool.RemoveAt(0);

				return pooled; // Returns the object that has been pooled.

			} 
			else
			{
				if(currentObjectInstances < poolInfo.maxObjectInstances)
				{

					// If there is no objects on the Pool and the max number of instances hasn't been reached, spawns a
					// new instance of the object on the spawn position.
					GameObject new_poll_obj = CreateNew(spawnPosition, spawnRotation);
					
					if(new_poll_obj != null)
					{
						new_poll_obj.SetActive(true); // Enables the object.
						return new_poll_obj;
					}

					Debug.LogError("ObjectPool.Spawn: Failure on instantiating " + poolInfo.baseObject.name + "!");

				}
				else
					Debug.LogWarning("ObjectPool.Spawn: Can't create a new instance of " + poolInfo.baseObject.name + " because the pool is already full!");

				return null;
			}
		}

		// Spawns a object from the pool.
		// Version 2: Used as a wraper. Calls Version 1 with the right parameters.
		public GameObject Spawn (Transform spawnPosition) { return Spawn(spawnPosition.position, spawnPosition.rotation); }

		// Creates a new GameObject to be used by the pooling system.
		private GameObject CreateNew(Vector3 position, Quaternion rotation)
		{

			// Instatiates the new object.
			GameObject new_poll_obj = Instantiate(poolInfo.baseObject, position, rotation);

			// Tries to get the PooledObject script from the new GameObject.
			PooledObject script = new_poll_obj.GetComponent<PooledObject>();

			if(script != null)
			{

				DontDestroyOnLoad(new_poll_obj); // Sets the object to no be destroyed when loading a new scene.

				DespawnPoolObjects += script.Despawn; // Stores the new object's Despawn function to be used when
													  // destroying the Pool.

				currentObjectInstances++;  // Increases the instance count.

				script.origin = this; // Marks this script as the origin of this projectile.
				new_poll_obj.SetActive(false);  // Disables the object.

				return new_poll_obj; // Returns the new created object.

			}

			// If the object doesn't have a PooledObject, destroys the object.
			Debug.LogWarning("ObjectPool.CreateNew: Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
			Destroy(new_poll_obj);
			return null;

		}

		protected void OnDestroy()
		{
			
			if(poolInfo != null) // Removes this Pool from the PoolingController.
				PoolingController.instance.Remove(poolInfo.id);

			// Despawns all objects instantiated by this Pool.
			if(DespawnPoolObjects != null)
				DespawnPoolObjects();

			// Destroys the objects on the Pool.
			while(Pool.Count > 0)
			{
				Destroy(Pool[0]);
				Pool.RemoveAt(0);
			}
		}

	}

}