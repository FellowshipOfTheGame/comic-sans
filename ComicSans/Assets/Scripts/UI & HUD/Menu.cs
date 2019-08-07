using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.UIandHUD
{

    // Controls the game's main menu.
    [AddComponentMenu("Scripts/Menu")]
    public class Menu : MonoBehaviour {

        [System.Serializable]
        private class Options
        {
            [Tooltip("Master volume for the game.")]
            public Slider gameVolume = null;

            [Tooltip("Music volume multiplier.")]
            public Slider musicVolume = null;
        }
        [SerializeField] private Options options = null;

        [System.Serializable]
        private class Loading
        {

            [Tooltip("Final loading screen GameObject.")]
            public GameObject loadingScreen = null;

            [Tooltip("Fade effect to be used.")]
            public GameObject loadingFadeEffect = null;

            [Tooltip("Scene wall used on regular scenes to be enabled, during the transition.")]
            public GameObject sceneWall = null;

        }
        [SerializeField] private Loading loading = null;
        

        void Start()
        {

            GetVolume();
            SceneSettings.instance.OnReady();
            
        }

        public void Play()
		{

            // Gets the next scene on build order.
            int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
			Debug.Log("MainMenu.Play: Loading scene with index " + sceneIndex + "...");

            // Sets the time scale to default.
			Time.timeScale = 1;

			StartCoroutine(SceneLoading(sceneIndex));

		}

		private IEnumerator SceneLoading(int sceneIndex)
		{

			// Fades out the scene.
			RectTransform rect = loading.loadingFadeEffect.GetComponent<RectTransform>();
            while(rect.localScale.x > 0.02f)
			{
				
                // Enables the scene wall so the backgorund can't be seen on some aspect ratios.
                if(rect.localScale.magnitude < 0.5f && !loading.sceneWall.activeSelf)
                    loading.sceneWall.SetActive(true);

                // Reduces the size for the laading screen transition, to make the fade out effect.
				Vector3 newScale = rect.localScale - Vector3.one * 4.0f * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			// Displays the loading screen.
			loading.loadingScreen.SetActive(true);
            yield return new WaitForEndOfFrame();

			// Start loading the scene.
			SceneManager.LoadSceneAsync(sceneIndex);

		}

        public void GetVolume()
        {

            // If the PlayerPrefs keys doesn't exist sets then to default.
            if(!PlayerPrefs.HasKey("game_volume"))
                PlayerPrefs.SetFloat("game_volume", 1.0f);

            if(!PlayerPrefs.HasKey("music_volume"))
                PlayerPrefs.SetFloat("music_volume", 1.0f);

            // Assign the saved values to the UI sliders.
            options.gameVolume.value = PlayerPrefs.GetFloat("game_volume");
            options.musicVolume.value = PlayerPrefs.GetFloat("music_volume");

        }

        public void SetVolume()
        {

            // Saves the values on the sliders to the PlayerPrefs.
            PlayerPrefs.SetFloat("game_volume", options.gameVolume.value);
            PlayerPrefs.SetFloat("music_volume", options.musicVolume.value);

            // Sets the volume of the game.
            AudioListener.volume = options.gameVolume.value;
            
            // Updates the music volume on the AudioController.
            if(AudioController.instance != null)
                AudioController.instance.UpdateMusicVolume();

        }

        public void ResetOptions()
        {

            // Saves the default values to the PlayerPrefs.
            PlayerPrefs.SetFloat("game_volume", 1.0f);
            PlayerPrefs.SetFloat("music_volume", 1.0f);

            // Assign the default values to the sliders.
            options.gameVolume.value = PlayerPrefs.GetFloat("game_volume");
            options.musicVolume.value = PlayerPrefs.GetFloat("music_volume");

        }

        public void Quit()
        {
            Debug.Log("MainMenu.Quit: Application.Quit()");
            Application.Quit();
        }
    }

}