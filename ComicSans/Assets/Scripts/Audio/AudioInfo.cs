using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComicSans
{

	[CreateAssetMenu(fileName = "newAudioInfo", menuName = "Audio Info", order = 2)]
	public class AudioInfo : ScriptableObject
	{

		// Used to refer to a specific sound.
		public string id;

		// Used to mark a group of sounds.
		public string tag;

		// Sounds of the music type will not be paused and will be played automatically at start.
		public enum Type { FX, Music };
		public Type type = Type.FX;

		// If sound is of the music type only clips[0] will be used. 
		// For the FX type a random clip from the array will be played each time.
		public AudioClip[] clips = new AudioClip[]{};

		public float volume = 1.0f;
		public float pitch = 1.0f;

		public int maxSimultaneousSources = 3;

	}
}