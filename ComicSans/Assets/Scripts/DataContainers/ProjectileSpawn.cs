using UnityEngine;

namespace ComicSans.DataContainers {
    
    // Struct to be used in order of facilitating the setting of projectile spwans.
	[System.Serializable]
	public struct ProjectileSpawn {

		[Tooltip("Position to spawn the projectile.")]
		public Vector2 position;

		[Tooltip("Position to spawn the projectile, on the z-axis.")]
		public float rotation;

	}

}