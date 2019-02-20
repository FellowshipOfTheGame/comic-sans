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

	public float volume = 1.0f;
	public float pitch = 1.0f;

	public int maxSimultaneousSources = 3;

	// Sources are instantiated in Awake() and stored in here.
	[HideInInspector] public List<AudioSource> sources;

	private int currentSource = 0;

	public void Play()
	{

		if(type == Type.Music)
		{
			if(sources[0].isPlaying)
			{
				Debug.Log("Sound.Play: Restarting music (" + clips[0].name + ")...");
				sources[currentSource].Stop();
			}
			sources[0].Play();
			return;
		}

		sources[currentSource].clip = clips[Random.Range(0, clips.Length)];
		sources[currentSource].Play();

		currentSource++;
		if(currentSource == maxSimultaneousSources)
			currentSource = 0;
	}

	public void Stop()
	{

		for(int i = 0; i < sources.Count; i++)
			sources[i].Stop();

	}

}