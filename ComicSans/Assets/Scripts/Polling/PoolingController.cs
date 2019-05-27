using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Controller/Pooling")]
public class PoolingController : MonoBehaviour 
{

	public static PoolingController instance;	

	private Dictionary<string, ObjectPool> PoolDictionary = null;

	// Use this for initialization
	void Awake () 
	{
		
		// Destroy this object if a previous instance already exists.
		if(PoolingController.instance != null) 
		{
			Destroy(gameObject);
			return;
		}

		// Creates a singleton of this script.
		PoolingController.instance = this;
		DontDestroyOnLoad(gameObject);
		
	}
	
	// Addw an ObjectPool to the dictionary.
	public void Add(PoolInfo pool) 
	{

		if(PoolDictionary == null)
			PoolDictionary = new Dictionary<string, ObjectPool>();

		if(!PoolDictionary.ContainsKey(pool.id))
		{

			ObjectPool entry = CreateObjectPool(pool);
			PoolDictionary.Add(pool.id, entry);

		} 
		else 
		{
			Debug.Log("PoolingController.Add: ObjectPool with id \"" + pool.id + "\" is already on the dictionary!");
		}
	}

	// Removes an ObjectPool from the dictionary.
	public void Remove(string id) 
	{

		if(PoolDictionary == null)
			Debug.Log("PoolDictionary.Remove: Dictinary is empty!");

		if(PoolDictionary.ContainsKey(id))
		{

			PoolDictionary.Remove(id);

		} 
		else 
		{
			Debug.Log("PoolingController.Remove: ObjectPool with id \"" + id + "\" isn't on the dictionary!");
		}
	}

	// Creates and setups an ObjectPool.
	public ObjectPool CreateObjectPool(PoolInfo pool) 
	{

		// Creates the object of the new pool.		
		GameObject newPoolGameObject = new GameObject();

		newPoolGameObject.name = pool.id + "_ObjectPool";
		newPoolGameObject.transform.SetParent(PoolingController.instance.transform);

		// Adds the ObjectPool script.
		ObjectPool newObjectPool = newPoolGameObject.AddComponent(typeof(ObjectPool)) as ObjectPool;

		// Initializes the new pool values.
		newObjectPool.id = pool.id;

		newObjectPool._tag = pool.tag;

		newObjectPool.type = pool.type;

		newObjectPool.baseObject = pool.baseObject;

		newObjectPool.initialObjectInstances = pool.initialObjectInstances;
		newObjectPool.maxObjectInstances = pool.maxObjectInstances;

		// Calls the function to create the initial objects of the pool.
		newObjectPool.Initialize();

		return newObjectPool;

	}

	// Spawns a object from the pool with id.
	// Version 1:
	public void Spawn(string id, Vector3 spawnPosition, Quaternion spawnRotation) 
	{

		if(PoolDictionary == null || PoolDictionary.Count == 0) {
			Debug.Log("PoolDictionary.Spawn: Dictinary is empty!");
			return;
		}

		if(PoolDictionary.ContainsKey(id)) 
		{

			PoolDictionary[id].Spawn(spawnPosition, spawnRotation);
				
		} else
			Debug.Log("PoolDictionary.Spawn: Dictinary doesn't contain the key: " + id + "!");

	}

	// Spawns a object from the pool with id.
	// Version 2: Used as a wraper. Calls Version 1 with the right parameters.
	public void Spawn (string id, Transform spawnPosition) { Spawn(id, spawnPosition.position, spawnPosition.rotation); }

	// Spawns an object from a pool, if it doesn't exist creates it.
	// Version 1:
	public void Spawn(PoolInfo pool, Vector3 spawnPosition, Quaternion spawnRotation) 
	{

		// Adds the ObjectPool to the dictionary if it doesn't exist.
		if(PoolDictionary == null || !PoolDictionary.ContainsKey(pool.id))
			Add(pool);

		// Spawns the object.
		Spawn(pool.id, spawnPosition, spawnRotation);

	}

	// Spawns an object from a pool, if it doesn't exist creates it.
	// Version 2: Used as a wraper. Calls Version 1 with the right parameters.
	public void Spawn (PoolInfo pool, Transform spawnPosition) { Spawn(pool, spawnPosition.position, spawnPosition.rotation); }

	// Destroy all pools of the Boss type.
	public void DespawnBossObjects()
	{

		if(PoolDictionary == null || PoolDictionary.Count == 0) {
			Debug.Log("PoolController.DespawnBossObjects: Dictinary is empty!");
			return;
		} 
		else
		{
			foreach (KeyValuePair<string, ObjectPool> entry in PoolDictionary)
				if(entry.Value.type == PoolInfo.Type.Boss)
					if(entry.Value.DespawnPoolObjects != null)
						entry.Value.DespawnPoolObjects();
		}	

	}

	// Destroy all pools with a specific tag.
	public void DestroyWithTag(string tag)
	{

		if(PoolDictionary == null || PoolDictionary.Count == 0) {
			Debug.Log("PoolController.DestroyBossObjects: Dictinary is empty!");
			return;
		} 
		else
		{
			foreach (KeyValuePair<string, ObjectPool> entry in PoolDictionary)
				if(entry.Value._tag == tag)
					Destroy(entry.Value.gameObject);
		}	

	}

	protected void OnDestroy()
	{

		// Destroy all pools when the controller is destroyed.
		if(PoolDictionary == null || PoolDictionary.Count == 0) {
			Debug.Log("PoolController.OnDestroy: Dictinary is empty! [This message may be caused by the destruction of a second instance of this script when entering a scene]");
			return;
		} 
		else
		{
			foreach (KeyValuePair<string, ObjectPool> entry in PoolDictionary)
				if(entry.Value != null)
					Destroy(entry.Value.gameObject);
		}	

	}
}
