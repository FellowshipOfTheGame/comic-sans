using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Audio/Control Center")]
public class AudioControlCenter : MonoBehaviour 
{

	public static AudioControlCenter instance;	

	// Used to improve the interface on Editor. In practice, will only 
	// be used to build the Dictionary later.
	[System.Serializable] struct SoundId { public string id; public Sound sound; } 
	[SerializeField] SoundId[] soundId;

	Dictionary<string, Sound> SoundDictionary;

	// Use this for initialization
	void Awake () {
		
		// Destroy a previous instance from this script.
		if(AudioControlCenter.instance != null) 
		{
			Destroy(gameObject);
			return;
		}

		AudioControlCenter.instance = this;

		BuildSoundDictionary();
		
	}
	
	private void BuildSoundDictionary() 
	{

		SoundDictionary = new Dictionary<string, Sound>();
		
		for(int i = 0; i < soundId.Length; i++)
		{
		
			CreateAudioSources(soundId[i].sound);
			SoundDictionary.Add(soundId[i].id, soundId[i].sound);
		}
	}

	public void CreateAudioSources(Sound sound) 
	{
		
		sound.sources = new List<AudioSource>();

		for(int i = 0; i < sound.maxSimultaneousSources; i++)
		{

			GameObject newAudioSourceGameObject = new GameObject();
			newAudioSourceGameObject.transform.SetParent(transform);

			newAudioSourceGameObject.name = sound.clips[0].name + "_AudioSource_" + i;
			newAudioSourceGameObject.transform.parent = AudioControlCenter.instance.transform;

			AudioSource newAudioSource = newAudioSourceGameObject.AddComponent(typeof(AudioSource)) as AudioSource;

			// Configure the audio source.
			newAudioSource.volume = sound.volume;
			newAudioSource.pitch = sound.pitch;

			if(sound.type == Sound.Type.FX)
				newAudioSource.loop = false;
			else
			{
				newAudioSource.loop = true;
				newAudioSource.clip = sound.clips[0];
				sound.sources.Add(newAudioSource);
				break; // Music will always use only one source.
			}

			sound.sources.Add(newAudioSource);
			
		}
		
	}

	public void Play(string id) {

		if(SoundDictionary.ContainsKey(id))
			SoundDictionary[id].Play();			
		else
			Debug.Log("AudioControlCenter.Play: Dictinary doesn't contain the key: " + id + "!");

	}

	public void StopAllSounds()
	{

		foreach (KeyValuePair<string, Sound> soundEntry in SoundDictionary)
		{

			soundEntry.Value.Stop();

		}

	}
}
