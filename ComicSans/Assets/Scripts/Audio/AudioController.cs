using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioDictEntry {

	public AudioInfo audio;

	public int currentSource = 0;
	public AudioSource[] sources;

}

[AddComponentMenu("Scripts/Controller/Audio")]
public class AudioController : MonoBehaviour 
{

	public static AudioController instance;	

	private Dictionary<string, AudioDictEntry> AudioDictionary = null;

	// Use this for initialization
	void Awake () 
	{
		
		// Destroy this object if a previous instance already exists.
		if(AudioController.instance != null) 
		{
			Destroy(gameObject);
			return;
		}

		// Creates a singleton of this script.
		AudioController.instance = this;
		DontDestroyOnLoad(gameObject);

		SceneManager.sceneLoaded += OnSceneLoaded;
		
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{

		StopAllSounds();

	}
	
	// Add an Audio to the dictionary.
	public void Add(AudioInfo sound) 
	{

		if(AudioDictionary == null)
			AudioDictionary = new Dictionary<string, AudioDictEntry>();

		AudioDictEntry entry = new AudioDictEntry();

		entry.audio = sound;
		entry.currentSource = 0;
		entry.sources = CreateAudioSources(sound);

		AudioDictionary.Add(sound.id, entry);

	}

	// Creates and setups AudioSources for an Audio.
	public AudioSource[] CreateAudioSources(AudioInfo sound) 
	{
		
		AudioSource[] sources;
		if(sound.type == AudioInfo.Type.Music)
			sources = new AudioSource[1];
		else
			sources = new AudioSource[sound.maxSimultaneousSources];

		for(int i = 0; i < sound.maxSimultaneousSources; i++)
		{

			GameObject newAudioSourceGameObject = new GameObject();

			newAudioSourceGameObject.name = sound.clips[0].name + "_AudioSource_" + i;
			newAudioSourceGameObject.transform.SetParent(AudioController.instance.transform);

			AudioSource newAudioSource = newAudioSourceGameObject.AddComponent(typeof(AudioSource)) as AudioSource;

			// Configure the audio source.
			if(sound.type == AudioInfo.Type.FX)
			{
				newAudioSource.volume = sound.volume;
				newAudioSource.pitch = sound.pitch;

				newAudioSource.loop = false;
			}
			else
			{

				float volMultiplier = 1;
				if(PlayerPrefs.HasKey("music_volume"))
					volMultiplier = PlayerPrefs.GetFloat("music_volume");

				newAudioSource.volume = sound.volume * volMultiplier;
				newAudioSource.pitch = sound.pitch;
				
				newAudioSource.loop = true;

				newAudioSource.clip = sound.clips[0];
				sources[i] = newAudioSource;
				break; // Music will always use only one source.
			}

			sources[i] = newAudioSource;
			
		}
		
		return sources;

	}

	// Plays an audio by id.
	public void Play(string id) 
	{

		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
			return;
		}

		if(AudioDictionary.ContainsKey(id)) 
		{

			AudioDictEntry entry = AudioDictionary[id];

			entry.sources[entry.currentSource].clip = entry.audio.clips[Random.Range(0, entry.audio.clips.Length)];
			entry.sources[entry.currentSource].Play();
			entry.currentSource = (entry.currentSource) % entry.audio.maxSimultaneousSources;
				
		} else
			Debug.Log("AudioController.Play: Dictinary doesn't contain the key: " + id + "!");

	}

	// Plays an audio, if it doesn't exist creates it.
	public void Play(AudioInfo audio) 
	{

		// Add the audio to the dictionary if it doesn't exist.
		if(AudioDictionary == null || !AudioDictionary.ContainsKey(audio.id))
			Add(audio);

		// Plays the sound.
		Play(audio.id);

	}

	// Plays an audio after a delay, if it doesn't exist creates it.
	public void Play(AudioInfo audio, float delay) 
	{

		// Add the audio to the dictionary if it doesn't exist.
		if(AudioDictionary == null || !AudioDictionary.ContainsKey(audio.id))
			Add(audio);

		if(delay <= 0) // Plays the audio immediately.
			Play(audio.id);
		else // Plays the audio after a delay.
			StartCoroutine(PlayAudioDelayed(audio, delay));

	}

	IEnumerator PlayAudioDelayed(AudioInfo audio, float delay)
	{

		float time = 0;

		while(time < delay)
		{

			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();			

		}

		// Plays the sound.
		Play(audio.id);

	}

	// Stops a sound by id.
	public void Stop(string id) 
	{

		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
			return;
		}

		if(AudioDictionary.ContainsKey(id)) 
		{

			AudioDictEntry entry = AudioDictionary[id];

			foreach(AudioSource source in AudioDictionary[id].sources)
				source.Stop();
				
		} else
			Debug.Log("AudioController.Play: Dictinary doesn't contain the key: " + id + "!");

	}

	// Stops all sounds with an specific tag.
	public void StopWithTag(string tag)
	{
		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopWithTag: Dictinary is empty!");
			return;
		}

		foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
		{
			if(entry.Value.audio.tag == tag)
				foreach(AudioSource source in entry.Value.sources)
					source.Stop();
		}

	}

	// Stops all sounds.
	public void StopAllSounds()
	{
		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
			return;
		}

		foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
				foreach(AudioSource source in entry.Value.sources)
					source.Stop();

	}

	// Pauses all sounds.
	public void PauseSounds()
	{
		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
			return;
		}

		foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
			if(entry.Value.audio.type != AudioInfo.Type.Music)
				foreach(AudioSource source in entry.Value.sources)
					source.Pause();

	}

	// Unpauses all sounds.
	public void UnPauseSounds()
	{
		if(AudioDictionary == null || AudioDictionary.Count == 0) {
			Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
			return;
		}

		foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
			if(entry.Value.audio.type != AudioInfo.Type.Music)
				foreach(AudioSource source in entry.Value.sources)
					source.UnPause();

	}
}
