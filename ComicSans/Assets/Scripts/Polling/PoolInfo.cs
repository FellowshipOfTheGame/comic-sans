using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPoolInfo", menuName = "Pool Info", order = 2)]
public class PoolInfo : ScriptableObject
{

	// Used to refer to a specific pool.
	public string id;

	// Used to mark a group of pools.
	public string tag;

	// Identifies a type of pooled objects. Polled objects with the type "Boss", will be despawned when a Boss advances phases or dies.
	public enum Type { Player, Boss, Other };
	public Type type = Type.Other;

	// The prefab to be used by the pool.
	public GameObject baseObject;

	public int initialObjectInstances = 8;
	public int maxObjectInstances = 8;

}