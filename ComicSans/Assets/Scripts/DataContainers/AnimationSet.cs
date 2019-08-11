using UnityEngine;

namespace ComicSans.DataContainers {

	// Struct to be used in order of facilitating the setting of animations.
	[System.Serializable]
	public struct AnimationSet {

		[Tooltip("Name of the parameter to be set.")]
		public string name;

		[Tooltip("Value to be assigned to the parameter.")]
		public int value;

	}

}