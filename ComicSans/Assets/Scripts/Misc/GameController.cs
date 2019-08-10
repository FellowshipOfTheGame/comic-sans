using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

using ComicSans.UI;
using ComicSans.Boss;
using ComicSans.Player;

namespace ComicSans
{

	
	// Manages the main flow of the game.
	[AddComponentMenu("Scripts/Controller/Game")]
	public class GameController : MonoBehaviour {

		public static GameController instance;

		[Space(10)]
		[Tooltip("Prefab fot he Player.")]
		public GameObject playerPrefab;

		// If the user is able to control the Player.
		private bool allowPlayerControl = true;
		public bool AllowPlayerControl {
			get
			{
				return allowPlayerControl;
			}
		}

		public enum GameState { Play, Paused, Win, Lose, WinScreen, LoseScreen, LoadingScreen }
		[Tooltip("Current state of the game.")]
		public GameState currentGameState = GameState.Play;

		// Spawn position to be used when the Player returns from a scene.
		[HideInInspector] public Vector2 returnSpawnPos = Vector2.positiveInfinity;

		// List of unique defeated Bosses.
		public HashSet<string> defeatedBosses = new HashSet<string>();

		[Space(10)]
		[Tooltip("Number of unique Bosses to be defeated before the game ends.")]
		[SerializeField] private int bossesToEndGame = 4;
		[Tooltip("Scene used to transition to the end game.")]
		[SerializeField] private string endGameTransition = "EndGameTransition";
		[Tooltip("End game scene.")]
		[SerializeField] private string endGameScene = "EndGame";
		// If the Player has won the game on this run before.
		private bool gameEnded = false;

		[Space(10)]
		[Tooltip("Name of the main menu scene.")]
		[SerializeField] private string menuScene = "Menu";

		[Space(10)]
		[Tooltip("UIController to be used.")]
		public UIController uiController;

		void Awake()
		{

			// Destroy this object if a previous instance already exists.
			if(instance != null)
			{
				Destroy(gameObject);
				return;
			}

			// Resets the time.
			Time.timeScale = 1;

			// Creates a singleton of this script.
			instance = this;
			DontDestroyOnLoad(gameObject);

			// Adds a function to be called when a new scene loads.
			SceneManager.sceneLoaded += OnSceneLoaded;

			// Sets the returnPosition to an invalid value to indicate it has never been set.
			returnSpawnPos = Vector2.positiveInfinity;

			// Resets the end game conditions.
			defeatedBosses = new HashSet<string>();
			gameEnded = false;

		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{

			StopAllCoroutines();

			// Removes the GameController if exiting gameplay portions.
			if(scene.name == menuScene || scene.name == endGameScene)
			{

				// Removes the OnSceneLoaded event.
				SceneManager.sceneLoaded -= OnSceneLoaded;

				// Guarantees that time is reset.
				Time.timeScale = 1;

				// Destroys the Player and GameController.
				Destroy(PlayerScript.instance.gameObject);
				Destroy(gameObject);

				return;

			}

			// Otherwise, spawns the Player and starts a new gameplay scene.
			SpawnPlayer();
			currentGameState = GameState.Play;
			allowPlayerControl = true;

			// Fades in a scene.
			uiController.FadeInScene();

			// Initializes necessary scene settings.
			SceneSettings.instance.OnReady();

		}

		// Spawn the Player and initializes it.
		private void SpawnPlayer()
		{

			if(PlayerScript.instance == null) // If no Player exists instantiates one.
			{
				
				// Spawns the Player on the correct position.
				GameObject _player;
				if(SceneSettings.instance.useReturnPos && returnSpawnPos != Vector2.positiveInfinity)
					_player = Instantiate(playerPrefab, returnSpawnPos, new Quaternion());
				else
					_player = Instantiate(playerPrefab, SceneSettings.instance.playerSpawnPoint, new Quaternion());

				DontDestroyOnLoad(_player);

			} else { // If a Player already exists, uses it instead.


				// Spawns the Player on the correct position.
				if(SceneSettings.instance.useReturnPos && returnSpawnPos != Vector2.positiveInfinity)
					PlayerScript.instance.transform.position = returnSpawnPos;
				else
					PlayerScript.instance.transform.position = SceneSettings.instance.playerSpawnPoint;

				// Initializes the Player().
				if(PlayerScript.instance.gameObject.activeSelf)
					PlayerScript.instance.Initialize();
				else
					PlayerScript.instance.gameObject.SetActive(true);

			}
		}

		// Pauses or unpauses the game.
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

				// Pauses the time and sounds.
				Time.timeScale = 0;
				AudioController.instance.PauseSounds();

				// Sets the proper UI.
				uiController.ResetPauseMenu();
				uiController.SetPauseMenu(true);
				uiController.UpdateInGameMenuSelection();

				// Hides touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);

			} 
			else if(currentGameState == GameState.Play)
			{

				// Inpauses the time and sounds.
				Time.timeScale = 1;
				AudioController.instance.UnPauseSounds();

				// Hides the UI.
				uiController.SetPauseMenu(false);

				// Shows touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(true);

			}

		}

		// Sets the win menu.
		public void SetWinMenu(bool state)
		{

			// Re-sets the time.
			Time.timeScale = 1;

			if(state == true)
			{

				// Disables Player.
				allowPlayerControl = false;
				PlayerScript.instance.StopMovimentation();

				// Sets the proper UI.
				uiController.UpdateInGameMenuSelection();
				uiController.SetWinMenu(true);

				// Hides touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);

			} 
			else
			{

				// Re-enables player control.
				allowPlayerControl = true;

				// Hides the UI.
				uiController.SetWinMenu(false);

				// Shows touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(true);
				
			}

		}

		// Sets the lose menu.
		public void SetLoseMenu(bool state)
		{

			// Re-sets the time.
			Time.timeScale = 1;

			if(state == true)
			{	

				// Disables Player.
				allowPlayerControl = false;
				PlayerScript.instance.StopMovimentation();

				// Sets the proper UI.
				uiController.UpdateInGameMenuSelection();
				uiController.SetLoseMenu(true);

				// Hides touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(false);	

			} 
			else
			{

				// Re-enables player control.
				allowPlayerControl = true;

				// Hides the UI.
				uiController.SetLoseMenu(false);

				// Shows touch controls.
				if(InputController.instance.IsMobileDevice)
					InputController.instance.SetTouchUI(true);

			}

		}

		// Starts the EndScene Coroutine.
		public void StartEndScene(bool playerWin) { StartCoroutine(EndScene (playerWin, BossScript.instance.endAnimationDelay)); }

		// Ends the scene when either the Player or the Boss is defeated.
		private IEnumerator EndScene (bool playerWin, float endAnimationDelay) {

			// WORKAROUND ===================================================================================
			// Workaround used to stop Boss sounds played using PlayAudioDelayed, may not be necessary if a better fix is implemented.
			// Stops all Boss sounds.
			AudioController.instance.StopWithTag(BossScript.instance.id);
			// WORKAROUND ===================================================================================
			
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
				SetWinMenu(true);
			}
			else // If the player loses, displays the death menu.
			{
				currentGameState = GameState.LoseScreen;
				SetLoseMenu(true);
			}

		}

		public void RestartScene()
		{

			Debug.Log("GameController.RestartScene: Restarting...");

			// Resets the player.
			PlayerScript.instance.transform.position = SceneSettings.instance.playerSpawnPoint;
			PlayerScript.instance.gameObject.SetActive(true);
			allowPlayerControl = true;

			uiController.EnableHUD();

			SceneSettings.instance.SpawnBoss();

			currentGameState = GameState.Play;

		}

		public void QuitToReturnScene()
		{

			// Verifies if the Player hasn't won the game.
			if(defeatedBosses.Count < bossesToEndGame || gameEnded)
				LoadScene(SceneSettings.instance.returnSceneName); // Loads the regular scene.
			else
			{
				gameEnded = true; // Loads the end game scene.
				LoadScene(endGameTransition);
			}

		}

		public void QuitToMainMenu()
		{

			PlayerScript.instance.StopMovimentation();
			LoadScene(menuScene);

		}

		public void LoadScene(string sceneName)
		{

			Debug.Log("GameController.LoadScene: Loading " + sceneName + "...");
			currentGameState = GameState.LoadingScreen;

			// Disables the in-game menus.
			uiController.SetPauseMenu(false);
			uiController.SetWinMenu(false);
			uiController.SetLoseMenu(false);

			// Sets time and disables the Player.
			Time.timeScale = 1;
			allowPlayerControl = false;

			// Calls the scene loading with the loading effect.
			uiController.LoadScene(sceneName);

		}

	}

}
