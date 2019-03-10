using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public static GameController instance;

	[System.Serializable]
	public class PlayerSettings
	{
		
		public GameObject prefab;
		public Vector2 spawnPoint;

	}
	public PlayerSettings playerSettings;

	private bool allowPlayerControl = true;
	public bool AllowPlayerControl {
		get
		{
			return allowPlayerControl;
		}
	}

	private bool paused = false;
	public bool Paused {
		get
		{
			return paused;
		}
	}
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject deathMenu;
	[SerializeField] private GameObject victoryMenu;

	void Awake()
	{

		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Time.timeScale = 1;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		instance = this;
		DontDestroyOnLoad(gameObject);

		SceneManager.sceneLoaded += OnSceneLoaded;

		if(Player.instance == null)
		{
			GameObject _player = Instantiate(playerSettings.prefab, playerSettings.spawnPoint, new Quaternion());
			DontDestroyOnLoad(_player);
			_player.SetActive(true);
		}

	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{

		StopAllCoroutines();

		if(scene.name == "Menu")
		{

			// Removes the OnSceneLoaded event.
			SceneManager.sceneLoaded -= OnSceneLoaded;

			// Guarantees that time and cursor settings are reset.
			Time.timeScale = 1;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			// Destroys the Player and GameController.
			Destroy(Player.instance.gameObject);
			Destroy(gameObject);

			return;

		}

		AudioControlCenter.instance.StopAllSounds();

		SpawnPlayer();
		
	}

	private void SpawnPlayer()
	{

		if(Player.instance.gameObject.activeSelf)
			Player.instance.OnEnable();
		else
			Player.instance.gameObject.SetActive(true);

		Player.instance.transform.position = playerSettings.spawnPoint;

	}

	public void SetPause(bool state) {

		paused = state;

		if(paused)
		{
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			AudioControlCenter.instance.PauseSounds();
			pauseMenu.SetActive(true);
		} 
		else 
		{
			Time.timeScale = 1;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			AudioControlCenter.instance.UnPauseSounds();
			pauseMenu.SetActive(false);
		}

	}

	public IEnumerator EndGame (float delay, bool playerWin) {
		
		float time = 0;

		while(time < delay)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();

		}

		if(playerWin)
		{
			SetVictoryMenu(true);
			Player.instance.DisableCollider();
		}
		else
			SetDeathMenu(true);

	}

	public void SetDeathMenu(bool state)
	{

		Time.timeScale = 1;

		if(state == true)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;			
			allowPlayerControl = false;
			deathMenu.SetActive(true);
		} 
		else
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			allowPlayerControl = true;
			deathMenu.SetActive(false);
		}

	}

	public void SetVictoryMenu(bool state)
	{

		Time.timeScale = 1;

		if(state == true)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			allowPlayerControl = false;
			victoryMenu.SetActive(true);
		} 
		else
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			allowPlayerControl = true;
			victoryMenu.SetActive(false);
		}

	}

	public void RestartScene()
	{

		SceneSettings.instance.SpawnBoss();
		Player.instance.transform.position = playerSettings.spawnPoint;
		Player.instance.gameObject.SetActive(true);
		allowPlayerControl = true;
		HUDController.instance.EnableHUD();

	}

	public void QuitToMainMenu()
	{
		SceneManager.LoadSceneAsync("Menu");
	}

	public void LoadScene(string sceneName)
	{
		Debug.Log("GameController.LoadScene: Loading " + sceneName + "...");
		SceneManager.LoadSceneAsync(sceneName);
	}
}
