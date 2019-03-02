using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Audio/Control Center")]
public class AudioControlCenter : MonoBehaviour 
{

	public static AudioControlCenter instance;	

	// Used to improve the interface on Editor. In practice, will only 
	// be used to build the Dictionary later.
	[System.Serializable]struct SoundId { public string id; public Sound sound; } 
	[SerializeField]SoundId[] soundId;

	Dictionary<string, Sound> SoundDictionary;

	// Use this for initialization
	void Awake () {
		
		// Destroy a previous instance from this script.
		if(AudioControlCenter.instance != null) 
			Destroy(AudioControlCenter.instance);

		AudioControlCenter.instance = this;

		BuildSoundDictionary();
		
	}
	
	private void BuildSoundDictionary() 
	{

		SoundDictionary = new Dictionary<string, Sound>();
		
		for(int i = 0; i < soundId.Length; i++)
		{
		
			soundId[i].sound.CreateAudioSources();
			SoundDictionary.Add(soundId[i].id, soundId[i].sound);
		}
	}

	public void Play(string id) {

		if(SoundDictionary.ContainsKey(id))
			SoundDictionary[id].Play();			
		else
			Debug.Log("(AudioControlCenter) Dictinary doesn't contain the key: " + id + "!");

	}
}
