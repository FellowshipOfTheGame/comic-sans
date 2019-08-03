using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Player;
using ComicSans.UIandHUD;

namespace ComicSans
{

	
	// Manages the main flow of the game.
	[AddComponentMenu("Scripts/Controller/Game")]
	public class GameController : MonoBehaviour {

		public static GameController instance;

		[Space(10)]
		public GameObject playerPrefab;

		private bool allowPlayerControl = true;
		public bool AllowPlayerControl {
			get
			{
				return allowPlayerControl;
			}
		}

		public enum GameState { Play, Paused, Win, Lose, WinScreen, LoseScreen, LoadingScreen }
		public GameState currentGameState = GameState.Play;

		[Space(10)]
		[SerializeField] private GameObject pauseMenu = null;
		[SerializeField] private GameObject pauseMenuInitialSelection = null;

		[Space(10)]
		[SerializeField] private GameObject deathMenu = null;
		[SerializeField] private GameObject deathMenuInitialSelection = null;

		[Space(10)]
		[SerializeField] private GameObject victoryMenu = null;
		[SerializeField] private GameObject victoryMenuInitialSelection = null;

		[Space(10)]
		[SerializeField] private GameObject loadingScreen = null;
		[SerializeField] private GameObject loadingFadeEffect = null;

		[Space(10)]
		[SerializeField] private EventSystem eventSystem = null;

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
				Destroy(PlayerScript.instance.gameObject);
				Destroy(gameObject);

				return;

			}

			SpawnPlayer();
			currentGameState = GameState.Play;
			allowPlayerControl = true;

			StartCoroutine(FadeIn());

			SceneSettings.instance.OnReady();

		}

		private void SpawnPlayer()
		{

			if(PlayerScript.instance == null)
			{
				
				GameObject _player = Instantiate(playerPrefab, SceneSettings.instance.playerSpawnPoint, new Quaternion());
				DontDestroyOnLoad(_player);

			} else {

				if(PlayerScript.instance.gameObject.activeSelf)
					PlayerScript.instance.Initialize();
				else
					PlayerScript.instance.gameObject.SetActive(true);

				PlayerScript.instance.transform.position = SceneSettings.instance.playerSpawnPoint;

			}
		}

		public void SetPause(bool state) {

			if(currentGameState == GameState.Play)
			{
				currentGameState = GameState.Paused;
				allowPlayerControl = false;
			}
			else if(currentGameState == GameState.Paused)
			{
				currentGameState = GameState.Play;
				allowPlayerControl = true;
			}
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
				eventSystem.SetSelectedGameObject(pauseMenuInitialSelection);
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(false);
				pauseMenu.SetActive(true);
			} 
			else if(currentGameState == GameState.Play)
			{
				Time.timeScale = 1;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				AudioController.instance.UnPauseSounds();
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(true);
				pauseMenu.SetActive(false);
			}

		}

		public IEnumerator EndGame (float animDelay, bool playerWin) {
			
			float time = 0;

			while(time < animDelay)
			{
				time += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();

			}

			if(playerWin)
			{

				if(SceneSettings.instance.winScreenObject != null)
					SceneSettings.instance.winScreenObject.SetActive(true);

				PlayerScript.instance.gameObject.SetActive(false);

				time = 0;
				while(time < SceneSettings.instance.winScreenTime)
				{
					time += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}

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
				PlayerScript.instance.StopMovimentation();
				eventSystem.SetSelectedGameObject(deathMenuInitialSelection);
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(false);
				deathMenu.SetActive(true);				
			} 
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				allowPlayerControl = true;
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(true);
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
				PlayerScript.instance.StopMovimentation();
				eventSystem.SetSelectedGameObject(victoryMenuInitialSelection);
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(false);
				victoryMenu.SetActive(true);
			} 
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				allowPlayerControl = true;
				if(InputController.instance.isTouchDevice)
					InputController.instance.SetTouchUI(true);
				victoryMenu.SetActive(false);
			}

		}

		public void QuitToLobby(bool win)
		{

			string sceneName;

			if(win)
				sceneName = SceneSettings.instance.winLobbySceneName;
			else
				sceneName = SceneSettings.instance.loseLobbySceneName;

			LoadScene(sceneName);

		}

		public void QuitToMainMenu(bool win)
		{

			PlayerScript.instance.StopMovimentation();
			LoadScene("Menu");

		}

		public void RestartScene()
		{

			Debug.Log("GameController.RestartScene: Restarting...");

			PlayerScript.instance.transform.position = SceneSettings.instance.playerSpawnPoint;
			PlayerScript.instance.gameObject.SetActive(true);

			allowPlayerControl = true;

			HUDController.instance.EnableHUD();

			SceneSettings.instance.SpawnBoss();

			currentGameState = GameState.Play;

		}

		public void LoadScene(string sceneName)
		{

			Debug.Log("GameController.LoadScene: Loading " + sceneName + "...");
			currentGameState = GameState.LoadingScreen;

			pauseMenu.SetActive(false);
			deathMenu.SetActive(false);
			victoryMenu.SetActive(false);

			Time.timeScale = 1;
			allowPlayerControl = false;
			StartCoroutine(SceneLoading(sceneName));

		}

		private IEnumerator SceneLoading(string sceneName)
		{

			// Fades out the scene.
			RectTransform rect = loadingFadeEffect.GetComponent<RectTransform>();
			while(rect.localScale.x > 0.02f)
			{
				
				Vector3 newScale = rect.localScale - Vector3.one * 4.0f * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			// Displays the loading screen.
			loadingScreen.SetActive(true);
			yield return new WaitForEndOfFrame();

			// Start loading the scene.
			SceneManager.LoadSceneAsync(sceneName);

		}

		private IEnumerator FadeIn()
		{

			// Hides the loading screen.
			loadingScreen.SetActive(false);

			// Fades out the scene.
			RectTransform rect = loadingFadeEffect.GetComponent<RectTransform>();
			while(rect.localScale.x < 2.5f)
			{

				if(currentGameState == GameState.LoadingScreen) break;

				Vector3 newScale = rect.localScale + Vector3.one * 4.0f * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			if(currentGameState != GameState.LoadingScreen) rect.localScale = 2.5f * Vector3.one;

		}
		
	}

}
