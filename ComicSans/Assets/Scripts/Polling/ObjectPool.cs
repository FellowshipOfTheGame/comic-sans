using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Pooling/Object Pool")]
public class ObjectPool : MonoBehaviour {

	[SerializeField] private GameObject baseObject;

	public int initialObjectInstances = 8;
	public int maxObjectInstances = 8;
	public int currentObjectInstances = 0;

	public List<GameObject> Pool;

	void Start() 
	{
		// Creates the initial pool of objects.
		for(int i = 0; i < initialObjectInstances; i++) 
		{

			GameObject new_poll_obj = CreateNew(transform.position, transform.rotation);

			if(new_poll_obj != null)
				Pool.Add(new_poll_obj);
			else
			{
				Debug.Log("(ObjectPool) Failed to instantiate a new instance of " + baseObject.name + "!");
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
			if(currentObjectInstances < maxObjectInstances)
			{
				GameObject new_poll_obj = CreateNew(spawnPosition, spawnRotation);
				
				if(new_poll_obj != null)
				{
					new_poll_obj.SetActive(true);
					return new_poll_obj;
				}

				Debug.Log("(ObjectPool) Failure on instantiating " + baseObject.name + "!");

			}
			else
				Debug.Log("(ObjectPool) Can't create a new instance of " + baseObject.name + " because the pool is already full!");

			return null;
		}
	}

	// Spawns a object from the pool.
	// Version 2: Used as an wraper. Calls Version 1 with the right parameters.
	public GameObject Spawn (Transform spawnPosition) { return Spawn(spawnPosition.position, spawnPosition.rotation); }

	// Creates a new GameObject to be used by the pooling system.
	private GameObject CreateNew(Vector3 position, Quaternion rotation)
	{

		GameObject new_poll_obj = Instantiate(baseObject, position, rotation);
		PooledObject script = new_poll_obj.GetComponent<PooledObject>();
			
		if(script != null)
		{

			currentObjectInstances++;

			script.origin = this;
			new_poll_obj.SetActive(false);

			return new_poll_obj;

		}

		Debug.Log("(ObjectPool) Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
		Destroy(new_poll_obj);
		return null;

	}
}