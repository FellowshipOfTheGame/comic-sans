using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.PoolingSystem
{

	// Controls an Object Pool from which base Objects can be pooled.
	[AddComponentMenu("Scripts/Pooling/Object Pool")]
	public class ObjectPool : MonoBehaviour {

		[HideInInspector] public PoolInfo poolInfo;

		public List<GameObject> Pool;

		public int currentObjectInstances = 0;

		public delegate void DespawnPool();
		public DespawnPool DespawnPoolObjects;

		public void Initialize() 
		{

			// Initializes an empty list of objects.
			Pool = new List<GameObject>();
			currentObjectInstances = 0;

			// Creates the initial pool of objects.
			for(int i = 0; i < poolInfo.initialObjectInstances; i++) 
			{

				GameObject new_poll_obj = CreateNew(transform.position, transform.rotation);

				if(new_poll_obj != null)
					Pool.Add(new_poll_obj);
				else
				{
					Debug.Log("ObjectPool.Initialize: Failed to instantiate a new instance of " + poolInfo.baseObject.name + "!");
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
				GameObject pooled = Pool[0];

				pooled.transform.position = spawnPosition;
				pooled.transform.rotation = spawnRotation;

				pooled.SetActive(true);

				Pool.RemoveAt(0);
				return pooled;

			} 
			else
			{
				if(currentObjectInstances < poolInfo.maxObjectInstances)
				{
					GameObject new_poll_obj = CreateNew(spawnPosition, spawnRotation);
					
					if(new_poll_obj != null)
					{
						new_poll_obj.SetActive(true);
						return new_poll_obj;
					}

					Debug.Log("ObjectPool.Spawn: Failure on instantiating " + poolInfo.baseObject.name + "!");

				}
				else
					Debug.Log("ObjectPool.Spawn: Can't create a new instance of " + poolInfo.baseObject.name + " because the pool is already full!");

				return null;
			}
		}

		// Spawns a object from the pool.
		// Version 2: Used as a wraper. Calls Version 1 with the right parameters.
		public GameObject Spawn (Transform spawnPosition) { return Spawn(spawnPosition.position, spawnPosition.rotation); }

		// Creates a new GameObject to be used by the pooling system.
		private GameObject CreateNew(Vector3 position, Quaternion rotation)
		{

			GameObject new_poll_obj = Instantiate(poolInfo.baseObject, position, rotation);
			PooledObject script = new_poll_obj.GetComponent<PooledObject>();

			if(script != null)
			{

				DontDestroyOnLoad(new_poll_obj);

				DespawnPoolObjects += script.Despawn;

				currentObjectInstances++;

				script.origin = this;
				new_poll_obj.SetActive(false);

				return new_poll_obj;

			}

			Debug.Log("ObjectPool.CreateNew: Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
			Destroy(new_poll_obj);
			return null;

		}

		protected void OnDestroy()
		{
			
			PoolingController.instance.Remove(poolInfo.id);

			if(DespawnPoolObjects != null)
				DespawnPoolObjects();

			while(Pool.Count > 0)
			{
				Destroy(Pool[0]);
				Pool.RemoveAt(0);
			}
		}

	}

}