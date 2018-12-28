using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Audio/Control Center")]
public class AudioControlCenter : MonoBehaviour 
{

	public static AudioControlCenter instance;

	enum SoundType { FX, Music };

	[System.Serializable]class Sound 
	{

		public SoundType type = SoundType.FX;
		public AudioClip[] clips = new AudioClip[]{};
		public int maxSources = 3;

		// Sources are instantiated in Awake() and stored in here.
		[HideInInspector] public List<AudioSource> sources;

		// Stores the AudioSources in which the sound will be played next. If the sound is played more times than there are AudioSources
		// the AudioSources that were used first will restart the sound.
		int _curSource;
		[HideInInspector] public int currentSource {
			get 
			{
				return _curSource;
			}
			set
			{
				if(value >= maxSources)
					_curSource = value - maxSources;
				else
					_curSource = value;
			}
		}

	}

	// Used for improving the interface on Editor. In practice, will only be used to build the Dictionary later.
	[System.Serializable]struct AvailableSounds 
	{

		public string id;
		public Sound config;

	} 
	[SerializeField]AvailableSounds[] sounds;

	// Dictionary build from the avaliable sounds.
	Dictionary<string, Sound> soundDict;

	// Use this for initialization
	void Awake () {
		
		if(AudioControlCenter.instance != null) 
			Destroy(AudioControlCenter.instance);
			
		AudioControlCenter.instance = this;

		// Creates the sound dictionary and the AudioSources to be used.
		soundDict = new Dictionary<string, Sound>();
		for(int i = 0; i < sounds.Length; i++)
		{

			// Creates AudioSources for the sound.
			for(int j = 0; j < sounds[i].config.maxSources; j++)
			{
				GameObject newSourceObject = new GameObject();
				newSourceObject.name = sounds[i].id + "_Source_" + j;
				newSourceObject.transform.parent = transform;
				AudioSource newSource = newSourceObject.AddComponent(typeof(AudioSource)) as AudioSource;
				if(sounds[i].config.type == SoundType.FX)
					newSource.loop = false;
				else
				{
					newSource.loop = true;
					newSource.clip = sounds[i].config.clips[0];
					newSource.Play();
					sounds[i].config.sources.Add(newSource);
					break; // Music needs only one source.
				}

				sounds[i].config.sources.Add(newSource);
				
			}

			// Adds to sound dictionary.
			soundDict.Add(sounds[i].id, sounds[i].config);
			
		}

		
	}
	
	public void Play(string id) {

		if(soundDict.ContainsKey(id))
		{
			soundDict[id].sources[soundDict[id].currentSource].clip = soundDict[id].clips[Random.Range(0, soundDict[id].clips.Length)];
			soundDict[id].sources[soundDict[id].currentSource].Play();
			soundDict[id].currentSource++;
		} 
		else
			Debug.Log("(AudioControlCenter) Dictinary doesn't contain the key: " + id + "!");	
	}
}
