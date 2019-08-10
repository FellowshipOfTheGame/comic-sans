using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using ComicSans.Boss; // Used by the WORKAROUND, see below.

namespace ComicSans
{

	// Used to hold entries on the sound dictionary.
	// IMPORTANT: A workaround was used in StopWithTag to stop Boss sounds that use PlaySoundDelayed. Currently only
	// the Boss sounds use this feature. If this is no longer the case this code will need to be re-written. 
	public class AudioDictEntry {

		// Info about the base audio.
		public AudioInfo audio;

		// Source to use when playing the audio next.
		public int currentSource = 0;

		// List of sources instantiated for this audio.
		public AudioSource[] sources;

	}

	[AddComponentMenu("Scripts/Controller/Audio")]
	public class AudioController : MonoBehaviour 
	{

		public static AudioController instance;	

		private Dictionary<string, AudioDictEntry> AudioDictionary = null;

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

			// Sets a function to be called on scene loading.
			SceneManager.sceneLoaded += OnSceneLoaded;
			
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{

			// Stops sounds if necessary.
			if(SceneSettings.instance.stopSounds)
				StopAllSounds();

		}
		
		// Add an Audio to the dictionary.
		public void Add(AudioInfo sound) 
		{

			// If the AudioDictionary is empty creates it.
			if(AudioDictionary == null)
				AudioDictionary = new Dictionary<string, AudioDictEntry>();

			if(!AudioDictionary.ContainsKey(sound.id))
			{

				// Adds the audio entry to the dictionary.
				AudioDictEntry entry = new AudioDictEntry();

				entry.audio = sound;
				entry.currentSource = 0;
				entry.sources = CreateAudioSources(sound);

				AudioDictionary.Add(sound.id, entry);

			} 
			else 
			{
				Debug.Log("AudioController.Add: Audio with id \"" + sound.id + "\" is already on the dictionary!");
			}

		}

		// Creates and setups AudioSources for an Audio.
		public AudioSource[] CreateAudioSources(AudioInfo sound) 
		{
			
			AudioSource[] sources;
			if(sound.type == AudioInfo.Type.Music)
				sources = new AudioSource[1]; // Musics only need one AudioSource.
			else
				sources = new AudioSource[sound.maxSimultaneousSources];

			// Creates the AudioSources.
			for(int i = 0; i < sources.Length; i++)
			{

				// Creates and configures the AudioSource GameObject.
				GameObject newAudioSourceGameObject = new GameObject();

				newAudioSourceGameObject.name = sound.clips[0].name + "_AudioSource_" + i;
				newAudioSourceGameObject.transform.SetParent(AudioController.instance.transform);

				// Adds the AudioSource component.
				AudioSource newAudioSource = newAudioSourceGameObject.AddComponent(typeof(AudioSource)) as AudioSource;

				// Configure the AudioSource.
				if(sound.type == AudioInfo.Type.FX)
				{
					newAudioSource.volume = sound.volume;
					newAudioSource.pitch = sound.pitch;

					newAudioSource.loop = false;
				}
				else
				{

					float volMultiplier = 1; // Modifies the volume modifier by the music volume setting.
					if(PlayerPrefs.HasKey("music_volume"))
						volMultiplier = PlayerPrefs.GetInt("music_volume") / 10.0f;
					newAudioSource.volume = sound.volume * volMultiplier;

					newAudioSource.pitch = sound.pitch;
					
					newAudioSource.loop = true; // Sets musics to loop.

					newAudioSource.clip = sound.clips[0]; // Musics only have one clip.
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
				Debug.LogWarning("AudioController.Play: Dictinary is empty!");
				return;
			}

			if(AudioDictionary.ContainsKey(id)) 
			{

				AudioDictEntry entry = AudioDictionary[id]; // Gets the audio entry on the dictionary.

				 
				if(entry.audio.type == AudioInfo.Type.Music)
				{

					// If the audio is a music that's already playing, returns.
					if(entry.sources[entry.currentSource].isPlaying) return;
					StopAllMusic(); // Else, stops other music.

				} 
				
				// Assign a random clip from the sound FX entry.
				if(entry.audio.type == AudioInfo.Type.FX)
				{
					entry.sources[entry.currentSource].Stop();
					entry.sources[entry.currentSource].clip = entry.audio.clips[Random.Range(0, entry.audio.clips.Length)];
				}

				entry.sources[entry.currentSource].Play();

				// Gets the source to use for the next time.
				if(entry.audio.type == AudioInfo.Type.FX)
					entry.currentSource = (entry.currentSource + 1) % entry.audio.maxSimultaneousSources;
				
					
			} else
				Debug.LogWarning("AudioController.Play: Dictinary doesn't contain the key: " + id + "!");

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

			// Waits for the delay.
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
				Debug.LogWarning("AudioController.Stop: Dictinary is empty!");
				return;
			}

			if(AudioDictionary.ContainsKey(id)) 
			{

				AudioDictEntry entry = AudioDictionary[id]; // Gets the audio entry on the dictionary.

				foreach(AudioSource source in AudioDictionary[id].sources) // Stops sounds with matching ids.
					source.Stop();
					
			} else
				Debug.LogWarning("AudioController.Play: Dictinary doesn't contain the key: " + id + "!");

		}

		// Stops all sounds with an specific tag.
		public void StopWithTag(string tag)
		{

			// WORKAROUND ===================================================================================
			// Workaround to stop delayed sounds played by Bosses.
			// There is no problem with this method as long as the only delayed audios are played by Bosses.
			// Otherwhise a better solution is needed.
			if(BossScript.instance != null && tag == BossScript.instance.id)
			{
				StopAllCoroutines(); // Used to stop possible PlayAudioDelayed Coroutines.
			}
			// WORKAROUND ===================================================================================

			if(AudioDictionary == null || AudioDictionary.Count == 0) {
				Debug.LogWarning("AudioController.StopWithTag: Dictinary is empty!");
				return;
			}

			foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary) // Looks through the sound tags.
			{
				if(entry.Value.audio.tag == tag) // Stop sounds with matching tags.
					foreach(AudioSource source in entry.Value.sources)
						if(source != null)
							source.Stop();
			}

		}

		// Stops all sounds.
		public void StopAllSounds()
		{

			// Used to stop possible PlayAudioDelayed Coroutines.
			StopAllCoroutines();

			if(AudioDictionary == null || AudioDictionary.Count == 0) {
				Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
				return;
			}

			foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
					foreach(AudioSource source in entry.Value.sources)
						source.Stop();

		}

		// Stops all sounds of the music type.
		public void StopAllMusic()
		{

			if(AudioDictionary == null || AudioDictionary.Count == 0) {
				Debug.Log("AudioController.StopAllSounds: Dictinary is empty!");
				return;
			}

			foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
				if(entry.Value.audio.type == AudioInfo.Type.Music)
					foreach(AudioSource source in entry.Value.sources)
						source.Stop();

		}

		// Pauses all sounds.
		public void PauseSounds()
		{
			if(AudioDictionary == null || AudioDictionary.Count == 0) {
				Debug.Log("AudioController.PauseSounds: Dictinary is empty!");
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
				Debug.Log("AudioController.UnPauseSounds: Dictinary is empty!");
				return;
			}

			foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
				if(entry.Value.audio.type != AudioInfo.Type.Music)
					foreach(AudioSource source in entry.Value.sources)
						source.UnPause();

		}

		// Updates the music volume with the modifier set in options.
		public void UpdateMusicVolume(float musicVolumeMultiplier)
		{

			if(AudioDictionary == null) return;

			foreach (KeyValuePair<string, AudioDictEntry> entry in AudioDictionary)
				if(entry.Value.audio.type == AudioInfo.Type.Music) // Detects if the audio is a music.
				{

					foreach(AudioSource source in entry.Value.sources)
						source.volume = entry.Value.audio.volume * musicVolumeMultiplier;

				}

		}

	}

}
