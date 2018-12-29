using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound 
{

	// Sounds of the music type will not be paused and will be played automatically at start.
	public enum Type { FX, Music };
	public Type type = Type.FX;

	// If sound is of the music type only clips[0] will be used. 
	// For the FX type a random clip from the array will be played each time.
	public AudioClip[] clips = new AudioClip[]{};

	public int volume = 1;
	public int pitch = 1;

	public int maxSimultaneousSources = 3;

	// Sources are instantiated in Awake() and stored in here.
	private List<AudioSource> sources;

	private int currentSource = 0;

	public void CreateAudioSources() 
	{
		
		sources = new List<AudioSource>();

		for(int i = 0; i < maxSimultaneousSources; i++)
		{

			GameObject newAudioSourceGameObject = new GameObject();
			newAudioSourceGameObject.name = clips[0].name + "_AudioSource_" + i;
			newAudioSourceGameObject.transform.parent = AudioControlCenter.instance.transform;

			AudioSource newAudioSource = newAudioSourceGameObject.AddComponent(typeof(AudioSource)) as AudioSource;

			// Configure the audio source.
			newAudioSource.volume = volume;
			newAudioSource.pitch = pitch;

			if(type == Sound.Type.FX)
				newAudioSource.loop = false;
			else
			{
				newAudioSource.loop = true;
				newAudioSource.clip = clips[0];
				newAudioSource.Play(); // Music starts playing automatically at the start.

				sources.Add(newAudioSource);
				break; // Music will always use only one source.
			}

			sources.Add(newAudioSource);
			
		}
		
	}

	public void Play()
	{

		if(type == Type.Music)
		{
			Debug.Log("(Sound) Restarting music (" + clips[0].name + ")...");
			sources[currentSource].Play();
			return;
		}

		sources[currentSource].clip = clips[Random.Range(0, clips.Length)];
		sources[currentSource].Play();

		currentSource++;
		if(currentSource == maxSimultaneousSources)
			currentSource = 0;
	}

}