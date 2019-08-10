using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.UI
{

    // Controls the game's main menu.
    [AddComponentMenu("Scripts/Controller/UI")]
    public class UIController : MonoBehaviour {

        [SerializeField] private EventSystem eventSystem = null;

        [System.Serializable]
        private class Options
        {
            [Tooltip("Master volume for the game.")]
            public Slider gameVolume = null;

            [Tooltip("Music volume multiplier.")]
            public Slider musicVolume = null;
        }
        [Tooltip("Configuration for the options menu.")]
        [SerializeField] private Options options = null;

        [System.Serializable]
        private class InGame
        {

            [Tooltip("In-game pause menu.")]
            public GameObject pauseMenu = null;
            [Tooltip("Main portion of the pause menu.")]
            public GameObject pauseMenuMain = null;
            [Tooltip("Options portion of the pause menu.")]
            public GameObject pauseMenuOptions = null;
            [Tooltip("UI object to be initially selected on the Pause screen.")]
            public Button initialSelectionPauseMenu = null;

            [Space(10)]
            public GameObject winMenu = null;
            [Tooltip("UI object to be initially selected on the Win screen.")]
            public Button initialSelectionWinMenu = null;

            [Space(10)]
            public GameObject loseMenu = null;
            [Tooltip("UI object to be initially selected on the Lose screen.")]
            public Button initialSelectionLoseMenu = null;

        }
        [Tooltip("Configuration for the in-game menus.")]
        [SerializeField] private InGame inGame = null;

        [System.Serializable]
        private class HUD
        {
            [Tooltip("Parent GameObject for the HUD objects.")]
            [SerializeField] public GameObject container = null;

            [Tooltip("UI elements for the player lifes, element 0 will be the last to disappear.")]
            [SerializeField] public Image[] playerHealthIcons = null;

            [Tooltip("UI for the Boss health bar.")]
            [SerializeField] public Image bossHealthBar = null;

            [Tooltip("UI to display the Boss name.")]
            [SerializeField] public Text bossNameText = null;

            // Used to store the inital Boss life to calculate how much of the Boss life should be filled.
		    [HideInInspector] public int initialBossLife = 5000;

        }
        [Tooltip("Configuration for the HUD.")]
        [SerializeField] private HUD hud = null;

        [System.Serializable]
        private class Loading
        {

            [Tooltip("Final loading screen GameObject.")]
            public GameObject screen = null;

            [Tooltip("Fade effect to be used.")]
            public GameObject fadeEffect = null;

            [Tooltip("Scene wall used on regular scenes to be enabled, during the transition.")]
            public GameObject sceneWall = null;

            [Tooltip("Fade speed for the vignette effect.")]
            public float fadeSpeed = 4.0f;

        }
        [Tooltip("Configuration for the loading screens.")]
        [SerializeField] private Loading loading = null;

		private void Awake () 
        {          

            // Gets the volume configurations.
            GetVolume();

            // Checks if the HUD is properly configured.
            if(SceneSettings.instance.showHUD)
            {
                if(hud.playerHealthIcons.Length < 1)
				Debug.Log("HUDController.Awake: No Player health icons assigned!");

                if(hud.bossNameText == null)
                    Debug.Log("HUDController.Awake: No Image component assigned as Boss health bar!");

                if(hud.bossNameText == null)
                    Debug.Log("HUDController.Awake: No Text component to display the Boss name assigned!");
                    
            }

        }

        // Gets the volume configurations.
        public void GetVolume()
        {

            // If the PlayerPrefs keys doesn't exist sets then to default.
            if(!PlayerPrefs.HasKey("game_volume"))
                PlayerPrefs.SetInt("game_volume", 10);

            if(!PlayerPrefs.HasKey("music_volume"))
                PlayerPrefs.SetInt("music_volume", 10);

            // Guarantees PlayerPrefs are saved.
            PlayerPrefs.Save();

            // Assign the saved values to the UI sliders.
            options.gameVolume.value = PlayerPrefs.GetInt("game_volume");
            options.musicVolume.value = PlayerPrefs.GetInt("music_volume");

             // Sets the volume of the game.
            AudioListener.volume = (int)Mathf.Floor(options.gameVolume.value) / 10.0f;

            // Updates the music volume on the AudioController.
            if(AudioController.instance != null)
                AudioController.instance.UpdateMusicVolume(Mathf.Floor(options.musicVolume.value) / 10.0f);

        }

        public void SetGameVolume()
        {

            // Saves the values on the sliders to the PlayerPrefs.
            PlayerPrefs.SetInt("game_volume", (int)Mathf.Floor(options.gameVolume.value));

            // Guarantees PlayerPrefs are saved.
            PlayerPrefs.Save();

            // Sets the volume of the game.
            AudioListener.volume = (int)Mathf.Floor(options.gameVolume.value) / 10.0f;

        }

        public void SetMusicVolume()
        {

            PlayerPrefs.SetInt("music_volume", (int)Mathf.Floor(options.musicVolume.value));

            // Guarantees PlayerPrefs are saved.
            PlayerPrefs.Save();
            
            // Updates the music volume on the AudioController.
            if(AudioController.instance != null)
                AudioController.instance.UpdateMusicVolume(Mathf.Floor(options.musicVolume.value) / 10.0f);

        }

        public void ResetOptions()
        {

            // Saves the default values to the PlayerPrefs.
            PlayerPrefs.SetInt("game_volume", 10);
            PlayerPrefs.SetInt("music_volume", 10);

            // Guarantees PlayerPrefs are saved.
            PlayerPrefs.Save();

            // Assign the default values to the sliders.
            options.gameVolume.value = PlayerPrefs.GetInt("game_volume");
            options.musicVolume.value = PlayerPrefs.GetInt("music_volume");

        }

        // Resets the pause menu to ensure it's not on the options menu.
        public void ResetPauseMenu()
        {

            inGame.pauseMenuOptions.SetActive(false);
            inGame.pauseMenuMain.SetActive(true);            

        }

        // Update the seleted UI object on the in-game menus.
        public void UpdateInGameMenuSelection()
        {

            if(GameController.instance.currentGameState == GameController.GameState.Paused)
            {
                eventSystem.SetSelectedGameObject(inGame.initialSelectionPauseMenu.gameObject);
				inGame.initialSelectionPauseMenu.OnSelect(null);
            }
            else if(GameController.instance.currentGameState == GameController.GameState.WinScreen)
            {
                eventSystem.SetSelectedGameObject(inGame.initialSelectionWinMenu.gameObject);
				inGame.initialSelectionWinMenu.OnSelect(null);
            }
            else if(GameController.instance.currentGameState == GameController.GameState.LoseScreen)
            {
                eventSystem.SetSelectedGameObject(inGame.initialSelectionLoseMenu.gameObject);
				inGame.initialSelectionLoseMenu.OnSelect(null);
            }

        }

        // Enables or disables the pause menu.
        public void SetPauseMenu(bool status) { inGame.pauseMenu.SetActive(status); }

        // Enables or disables the win menu.
        public void SetWinMenu(bool status) { inGame.winMenu.SetActive(status); }

        // Enables or disables the lose menu.
        public void SetLoseMenu(bool status) { inGame.loseMenu.SetActive(status); }

        public void UpdatePlayerLifeIcons(int life)
		{

			if(hud.playerHealthIcons.Length < 3)
			{
				Debug.LogWarning("HUDController.UpdatePlayerLifeIcons: You need 3 playerHealthIcons, but " + hud.playerHealthIcons.Length + " were assigned!");
				return;
			}

			for(int i = 0; i < 3; i++) 
			{

				if(hud.playerHealthIcons[i] == null)
				{
					Debug.LogWarning("HUDController.UpdatePlayerLifeIcons: playerHealthIcon on position " + i + " is null!");
					return;
				}

				if(i < life)
					hud.playerHealthIcons[i].enabled = true;
				else
					hud.playerHealthIcons[i].enabled = false;
			}
		}

		public void InitializeBossHUD(string bossName, int bossLife) 
		{
			hud.initialBossLife = bossLife; // Stores the initial Boss life.

			if(hud.bossHealthBar != null) // Sets the Boss health bar to be fully filled.
			{
				hud.bossHealthBar.fillAmount = 1; 
				hud.bossHealthBar.enabled = true;
			}

			if(hud.bossNameText != null) // Set the Boss name.
			{
				hud.bossNameText.text = bossName;
				hud.bossNameText.enabled = true;
			}		
		
		}

		public void UpdateBossHealthBar (int life)
		{

			if(hud.bossHealthBar != null)	
				hud.bossHealthBar.fillAmount = (float)life / (float)hud.initialBossLife; // Calculates and assigns how much of the life
																				         // bar should be filled.

		}

		public void UpdateBossName (string name)
		{

			if(hud.bossNameText != null)
				hud.bossNameText.text = name;

		}

		public void EnableHUD() { if(hud.container != null) hud.container.SetActive(true); }

		public void DisableHUD() { if(hud.container != null) hud.container.SetActive(false); }

        // Loads a scene by name.
        public void LoadScene(string sceneName) { StartCoroutine(SceneLoading(sceneName)); } // Starts the Coroutine to load the Scene.

        // Performs the fade effect and loads the Scene.
        public IEnumerator SceneLoading(string sceneName)
		{

			// Fades out the scene.
			RectTransform rect = loading.fadeEffect.GetComponent<RectTransform>();
            while(rect.localScale.x > 0.02f)
			{
				
                // Enables the scene wall so the backgorund can't be seen on some aspect ratios.
                if(rect.localScale.magnitude < 0.5f && !loading.sceneWall.activeSelf)
                    loading.sceneWall.SetActive(true);

                // Reduces the size for the laading screen transition, to make the fade out effect.
				Vector3 newScale = rect.localScale - Vector3.one * loading.fadeSpeed * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			// Displays the loading screen.
			loading.screen.SetActive(true);
            yield return new WaitForEndOfFrame();

			// Start loading the scene.
			SceneManager.LoadSceneAsync(sceneName);

		}

        // Starts the Coroutine to fade in a scene.
        public void FadeInScene() { StartCoroutine(FadeIn()); }

        // Performs the fade in effect by removing the loading screen and vignette.
        private IEnumerator FadeIn()
		{

			// Hides the loading screen.
			loading.screen.SetActive(false);

			// Fades out the scene.
			RectTransform rect = loading.fadeEffect.GetComponent<RectTransform>();
			while(rect.localScale.x < 2.5f)
			{

				Vector3 newScale = rect.localScale + Vector3.one * loading.fadeSpeed * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

            if(GameController.instance == null || GameController.instance.currentGameState != GameController.GameState.LoadingScreen)
			    rect.localScale = 2.5f * Vector3.one;

		}

        public void Quit()
        {
            Debug.Log("MainMenu.Quit: Application.Quit()");
            Application.Quit();
        }

    }

}