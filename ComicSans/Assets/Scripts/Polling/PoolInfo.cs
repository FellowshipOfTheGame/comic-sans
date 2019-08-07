using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.PoolingSystem
{

	// Contains data about an object that can be pooled.
	[CreateAssetMenu(fileName = "newPoolInfo", menuName = "Pool Info", order = 2)]
	public class PoolInfo : ScriptableObject
	{

		[Tooltip("Unique id for this Pool.")]
		public string id;

		[Tooltip("Used to mark a group of Pools.")]
		public string tag;

		// Identifies a type of pooled objects. Polled objects with the type "Boss", will be despawned when a Boss advances phases or dies.
		public enum Type { Player, Boss, Other };

		[Tooltip("Used to mark the type of origin for this Pool.")]
		public Type type = Type.Other;

		[Tooltip("The prefab to be used by the pool.")]
		public GameObject baseObject;

		[Tooltip("Number of object instances to be initialy spawned.")]
		public int initialObjectInstances = 8;

		[Tooltip("Max number of object instances to be spawned.")]
		public int maxObjectInstances = 8;

	}

}