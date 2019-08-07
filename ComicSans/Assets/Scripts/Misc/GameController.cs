using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Boss;
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

		[HideInInspector] public Vector2 returnSpawnPos = Vector2.positiveInfinity;

		public HashSet<string> defeatedBosses = new HashSet<string>();
		[SerializeField] private int bossesToEndGame = 4;
		[SerializeField] private string endGameScene = "EndGame";
		private bool gameEnded = false;

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

			returnSpawnPos = Vector2.positiveInfinity;

			defeatedBosses = new HashSet<string>();

			gameEnded = false;

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
				
				GameObject _player;
				if(SceneSettings.instance.useReturnPos && returnSpawnPos != Vector2.positiveInfinity)
					_player = Instantiate(playerPrefab, returnSpawnPos, new Quaternion());
				else
					_player = Instantiate(playerPrefab, SceneSettings.instance.playerSpawnPoint, new Quaternion());

				DontDestroyOnLoad(_player);

			} else {

				if(SceneSettings.instance.useReturnPos && returnSpawnPos != Vector2.positiveInfinity)
					PlayerScript.instance.transform.position = returnSpawnPos;
				else
					PlayerScript.instance.transform.position = SceneSettings.instance.playerSpawnPoint;

				if(PlayerScript.instance.gameObject.activeSelf)
					PlayerScript.instance.Initialize();
				else
					PlayerScript.instance.gameObject.SetActive(true);

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
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);
				pauseMenu.SetActive(true);
			} 
			else if(currentGameState == GameState.Play)
			{
				Time.timeScale = 1;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				AudioController.instance.UnPauseSounds();
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(true);
				pauseMenu.SetActive(false);
			}

		}

		// Starts the EndScene Coroutine.
		public void StartEndScene(bool playerWin) 
		{ 

			StartCoroutine(EndScene (playerWin, BossScript.instance.endAnimationDelay)); 

		}

		// Ends the scene when either the Player or the Boss is defeated.
		private IEnumerator EndScene (bool playerWin, float endAnimationDelay) {

			// Destroys the boss.
			GameObject boss = BossScript.instance.gameObject;

			BossScript.instance = null;
			Destroy(boss);

			// Wait for the delay after the end animation.
			float time = 0;
			while(time < endAnimationDelay)
			{
				time += Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}

			// If the player wins, waits for the win screen and the displays the win menu.
			if(playerWin)
			{

				// Displays the win screen.
				if(SceneSettings.instance.winScreenObject != null)
					SceneSettings.instance.winScreenObject.SetActive(true);

				// Disables the Player.
				PlayerScript.instance.gameObject.SetActive(false);

				// Waits for the win screen time.
				time = 0;
				while(time < SceneSettings.instance.winScreenTime)
				{
					time += Time.fixedDeltaTime;
					yield return new WaitForFixedUpdate();
				}

				// Hides the win screen.
				if(SceneSettings.instance.winScreenObject != null)
					SceneSettings.instance.winScreenObject.SetActive(false);

				// Displays the win menu.
				currentGameState = GameState.WinScreen;
				SetVictoryMenu(true);
			}
			else // If the player loses, displays the death menu.
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
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);
				deathMenu.SetActive(true);				
			} 
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				allowPlayerControl = true;
				if(InputController.instance.IsMobileDevice)
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
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);
				victoryMenu.SetActive(true);
			} 
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				allowPlayerControl = true;
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(true);
				victoryMenu.SetActive(false);
			}

		}

		public void QuitToLobby()
		{

			if(defeatedBosses.Count < bossesToEndGame || gameEnded)
				LoadScene(SceneSettings.instance.returnSceneName);
			else
			{
				gameEnded = true;
				LoadScene(endGameScene);
			}

		}

		public void QuitToMainMenu()
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
