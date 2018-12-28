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
			GameObject new_poll_obj = Instantiate(baseObject, transform.position, transform.rotation);
			PooledObject script = new_poll_obj.GetComponent<PooledObject>();
			if(script != null)
			{
				currentObjectInstances++;
				script.origin = this;
				new_poll_obj.SetActive(false);
				Pool.Add(new_poll_obj);
			}
			else
			{
				Debug.Log("(ObjectPool) Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
				Destroy(new_poll_obj);
			}			
		}
	}

	// Spawns a object from the pool.
	// Version 1:
	public GameObject Spawn (Vector3 spawnPosition, Quaternion spawnRotation) {
		
		if(Pool.Count > 0) 
		{
			GameObject pooled = Pool[0];
			Pool.RemoveAt(0);

			pooled.transform.position = spawnPosition;
			pooled.transform.rotation = spawnRotation;

			pooled.SetActive(true);
			return pooled;

		} 
		else
		{
			if(currentObjectInstances < maxObjectInstances)
			{
				GameObject new_poll_obj = Instantiate(baseObject, spawnPosition, spawnRotation);
				PooledObject script = new_poll_obj.GetComponent<PooledObject>();
				if(script != null)
				{
					currentObjectInstances++;
					script.origin = this;
					new_poll_obj.SetActive(true);
					return new_poll_obj;
				}
				else
				{
					Debug.Log("(ObjectPool) Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
					Destroy(new_poll_obj);
				}
			}
			else
				Debug.Log("(ObjectPool) Can't create a new instance of " + baseObject.name + " because the pool is already full!");

			return null;
		}
	}

	// Spawns a object from the pool.
	// Version 2:
	public GameObject Spawn (Transform spawnPosition) {
		
		if(Pool.Count > 0) 
		{
			GameObject pooled = Pool[0];
			Pool.RemoveAt(0);

			pooled.transform.position = spawnPosition.position;
			pooled.transform.rotation = spawnPosition.rotation;

			pooled.SetActive(true);
			return pooled;

		} 
		else
		{
			if(currentObjectInstances < maxObjectInstances)
			{
				GameObject new_poll_obj = Instantiate(baseObject, spawnPosition.position, spawnPosition.rotation);
				PooledObject script = new_poll_obj.GetComponent<PooledObject>();
				if(script != null)
				{
					currentObjectInstances++;
					script.origin = this;
					new_poll_obj.SetActive(true);
					return new_poll_obj;
				}
				else
				{
					Debug.Log("(ObjectPool) Object " + new_poll_obj.transform.name + " can't be pooled because it doesn't have a PooledObject component!");
					Destroy(new_poll_obj);
				}
			}
			else
				Debug.Log("(ObjectPool) Can't create a new instance of an object because the pool is already full!");

			return null;
		}
	}
}