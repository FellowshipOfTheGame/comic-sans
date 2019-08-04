using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.UIandHUD;

namespace ComicSans
{

	// Contrains data and code for controlling a specific Scene.
	[AddComponentMenu("Scripts/Scene/Settings")]
	public class SceneSettings : MonoBehaviour {

		public static SceneSettings instance;

		public AudioInfo backgroundMusic;
		public bool stopSounds = true;
		public bool showHUD = true;	

		public Vector2 playerSpawnPoint = new Vector2(0, -3.3f);
		public bool useReturnPos = false;

		public Vector2 positionConstraints = new Vector2( 6.25f, 5f);

		[System.Serializable]
		public class BossSettings
		{
			public bool bossScene = true;
			public GameObject boss;
			public Vector2 spawn = new Vector2( 0, 2.4f);
		}
		public BossSettings bossSettings;

		public string returnSceneName = "Lobby";
		public GameObject winScreenObject = null;
		public float winScreenTime = 3.0f;

		void Awake () 
		{
			
			if(instance != null)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;			

		}

		// Starts the scene when ready.
		public void OnReady()
		{

			if(HUDController.instance != null)
			{
				if(!showHUD)
					HUDController.instance.DisableHUD();
				else
					HUDController.instance.EnableHUD();
			}

			if(backgroundMusic != null)
				AudioController.instance.Play(backgroundMusic);

			if(bossSettings.bossScene)
				SpawnBoss();
		}

		public void SpawnBoss()
		{
			Instantiate(bossSettings.boss, bossSettings.spawn, new Quaternion());
		}

	}

}