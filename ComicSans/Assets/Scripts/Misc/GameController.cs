using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Controller/Game")]
public class GameController : MonoBehaviour {

	public static GameController instance;

	public GameObject playerPrefab;

	private bool allowPlayerControl = true;
	public bool AllowPlayerControl {
		get
		{
			return allowPlayerControl;
		}
	}

	public enum GameState { Play, Paused, Win, Lose, WinScreen, LoseScreen }
	public GameState currentGameState;

	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject deathMenu;
	[SerializeField] private GameObject victoryMenu;

	void Awake()
	{

		// Destroy this object if a previous instance already exists.
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Time.timeScale = 1;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Creates a singleton of this script.
		instance = this;
		DontDestroyOnLoad(gameObject);

		SceneManager.sceneLoaded += OnSceneLoaded;

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

		SpawnPlayer();
		currentGameState = GameState.Play;

		SceneSettings.instance.OnReady();

	}

	private void SpawnPlayer()
	{

		if(Player.instance == null)
		{
			
			GameObject _player = Instantiate(playerPrefab, SceneSettings.instance.playerSpawnPoint, new Quaternion());
			DontDestroyOnLoad(_player);

		} else {

			if(Player.instance.gameObject.activeSelf)
				Player.instance.OnEnable();
			else
				Player.instance.gameObject.SetActive(true);

			Player.instance.transform.position = SceneSettings.instance.playerSpawnPoint;

		}
	}

	public void SetPause(bool state) {

		if(currentGameState == GameState.Play)
			currentGameState = GameState.Paused;
		else if(currentGameState == GameState.Paused)
			currentGameState = GameState.Play;
		else
		{
			Debug.LogWarning("GameController.SetPause: Attempt to pause the game when the Player has already won or lost!");
			return;
		}

		if(currentGameState == GameState.Paused)
		{
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			AudioController.instance.PauseSounds();
			pauseMenu.SetActive(true);
		} 
		else if(currentGameState == GameState.Play)
		{
			Time.timeScale = 1;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			AudioController.instance.UnPauseSounds();
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
			currentGameState = GameState.WinScreen;
			SetVictoryMenu(true);
		}
		else
		{
			currentGameState = GameState.LoseScreen;
			SetDeathMenu(true);
		}

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

		Player.instance.transform.position = SceneSettings.instance.playerSpawnPoint;
		Player.instance.gameObject.SetActive(true);

		allowPlayerControl = true;

		HUDController.instance.EnableHUD();

		SceneSettings.instance.SpawnBoss();

		currentGameState = GameState.Play;

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
