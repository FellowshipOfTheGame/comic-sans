using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.UIandHUD
{

    [AddComponentMenu("Scripts/Menu")]
    public class Menu : MonoBehaviour {

        [System.Serializable]
        private class Options
        {
            public Slider gameVolume = null;
            public Slider musicVolume = null;
        }
        [SerializeField] private Options options = null;

        [System.Serializable]
        private class Loading
        {
            public GameObject loadingScreen = null;
            public GameObject loadingFadeEffect = null;
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

            int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
			Debug.Log("MainMenu.Play: Loading scene with index " + sceneIndex + "...");

			Time.timeScale = 1;
			StartCoroutine(SceneLoading(sceneIndex));

		}

		private IEnumerator SceneLoading(int sceneIndex)
		{

			// Fades out the scene.
			RectTransform rect = loading.loadingFadeEffect.GetComponent<RectTransform>();
			while(rect.localScale.magnitude > 0.01f)
			{
				
                if(rect.localScale.magnitude < 0.5f && !loading.sceneWall.activeSelf)
                    loading.sceneWall.SetActive(true);

				Vector3 newScale = rect.localScale - Vector3.one * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.005f, 1.0f);
				newScale.y = Mathf.Clamp(newScale.y, 0.005f, 1.0f);
				newScale.z = Mathf.Clamp(newScale.z, 0.005f, 1.0f);

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

            if(!PlayerPrefs.HasKey("game_volume"))
                PlayerPrefs.SetFloat("game_volume", 1.0f);

            if(!PlayerPrefs.HasKey("music_volume"))
                PlayerPrefs.SetFloat("music_volume", 1.0f);

            options.gameVolume.value = PlayerPrefs.GetFloat("game_volume");
            options.musicVolume.value = PlayerPrefs.GetFloat("music_volume");

        }

        public void SetVolume()
        {

            PlayerPrefs.SetFloat("game_volume", options.gameVolume.value);
            PlayerPrefs.SetFloat("music_volume", options.musicVolume.value);

            AudioListener.volume = options.gameVolume.value;
            
            if(AudioController.instance != null)
                AudioController.instance.UpdateMusicVolume();

        }

        public void ResetOptions()
        {

            PlayerPrefs.SetFloat("game_volume", 1.0f);
            PlayerPrefs.SetFloat("music_volume", 1.0f);

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