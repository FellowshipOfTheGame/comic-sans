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

		[Tooltip("Background music to be looped during the scene.")]
		public AudioInfo backgroundMusic;

		[Tooltip("If sounds should be stopped when entering the scene.")]
		public bool stopSounds = true;	

		[Tooltip("If the HUD should be shown on the scene.")]
		public bool showHUD = true;	

		[Tooltip("Where to spawn the Player.")]
		public Vector2 playerSpawnPoint = new Vector2(0, -3.3f);

		[Tooltip("If the player should be spawned on the return position instead.")]
		public bool useReturnPos = false;

		[Tooltip("The borders of the scene.")]
		public Vector2 positionConstraints = new Vector2( 6.25f, 5f);

		[System.Serializable]
		public class BossSettings
		{
			[Tooltip("If the scene shoud have a Boss.")]
			public bool bossScene = true;

			[Tooltip("Boss prefab.")]
			public GameObject boss;

			[Tooltip("Boss spawn position.")]
			public Vector2 spawn = new Vector2( 0, 2.4f);
		}
		[Tooltip("Configurations related to the Boss.")]
		public BossSettings bossSettings;

		[Tooltip("Scene that should be loaded when the Player wins or loses.")]
		public string returnSceneName = "Lobby";

		[Tooltip("GameObject to be activated as a win screen.")]
		public GameObject winScreenObject = null;

		[Tooltip("Time the win screen should stay.")]
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

			// Shows or hides the HUD.
			if(HUDController.instance != null)
			{
				if(!showHUD)
					HUDController.instance.DisableHUD();
				else
					HUDController.instance.EnableHUD();
			}

			// Plays the backgrund music if necessary.
			if(backgroundMusic != null)
				AudioController.instance.Play(backgroundMusic);

			// Spawns the Boss if necessary.
			if(bossSettings.bossScene)
				SpawnBoss();
		}

		public void SpawnBoss()
		{
			Instantiate(bossSettings.boss, bossSettings.spawn, new Quaternion());
		}

	}

}