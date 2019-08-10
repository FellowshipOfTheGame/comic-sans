using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ComicSans.UI
{

    // Controls the game's main menu.
    [AddComponentMenu("Scripts/Misc/EndScreen")]
    public class EndScreen : MonoBehaviour {

        [SerializeField] private string menuScene = "Menu";

        [SerializeField] private GameObject quitButton = null;

        [System.Serializable]
        private class Loading
        {

            [Tooltip("Final loading screen GameObject.")]
            public GameObject screen = null;

            [Tooltip("Fade effect to be used.")]
            public GameObject fadeEffect = null;

            [Tooltip("Scene wall used on regular scenes to be enabled, during the transition.")]
            public GameObject sceneWall = null;

            [Tooltip("Fade speed for the vignette effect when entering the scene.")]
            public float fadeInSpeed = 4.0f;

            [Tooltip("Fade speed for the vignette effect when exiting the scene.")]
            public float fadeOutSpeed = 4.0f;

            [Tooltip("When to freeze the vignette effect.")]
            public float freezeScale = 0.75f;

            [Tooltip("Faor how much time the vignette effect should be frozen.")]
            public float freezeDelay = 1.0f;

        }
        [SerializeField] private Loading loading = null;

        private void Awake()
        {

            Time.timeScale = 1;
            quitButton.SetActive(false);
            SceneSettings.instance.OnReady();
            StartCoroutine(FadeIn());

        }

        public void ExitToMainMenu()
		{

            // Disables the exit button.
            quitButton.SetActive(false);

            // Gets the main menu scene.
			Debug.Log("GameController.LoadScene: Loading " + menuScene + "...");

			StartCoroutine(SceneLoading(menuScene));

		}

		private IEnumerator SceneLoading(string sceneName)
		{

            // If the vignette effect has already been frozen.
            bool froze = false;

			// Fades out the scene.
			RectTransform rect = loading.fadeEffect.GetComponent<RectTransform>();
            while(rect.localScale.x > 0.02f)
			{
				
                // Enables the scene wall so the backgorund can't be seen on some aspect ratios.
                if(rect.localScale.magnitude < 0.5f && !loading.sceneWall.activeSelf)
                    loading.sceneWall.SetActive(true);

                // Reduces the size for the laading screen transition, to make the fade out effect.
				Vector3 newScale = rect.localScale - Vector3.one * loading.fadeOutSpeed * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);


                if(!froze) // Freeze the effect for a certain time.
                {

                    float time = 0;
                    if(newScale.x < loading.freezeScale)
                    {
                        while(time < loading.freezeDelay)
                        {

                            time += Time.fixedDeltaTime;
                            yield return new WaitForFixedUpdate();

                        }

                        froze = true;

                    }
                }

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			// Displays the loading screen.
			loading.screen.SetActive(true);
            yield return new WaitForEndOfFrame();

			// Start loading the scene.
			SceneManager.LoadSceneAsync(sceneName);

		}

        private IEnumerator FadeIn()
		{

			// Hides the loading screen.
			loading.screen.SetActive(false);

			// Fades out the scene.
			RectTransform rect = loading.fadeEffect.GetComponent<RectTransform>();
			while(rect.localScale.x < 2.5f)
			{

				Vector3 newScale = rect.localScale + Vector3.one * loading.fadeInSpeed * Time.deltaTime;
				newScale.x = Mathf.Clamp(newScale.x, 0.019f, 2.5f);
				newScale.y = Mathf.Clamp(newScale.y, 0.019f, 2.5f);
				newScale.z = Mathf.Clamp(newScale.z, 0.019f, 2.5f);

				rect.localScale = newScale;

				yield return new WaitForEndOfFrame();

			}

			rect.localScale = 2.5f * Vector3.one;

            quitButton.SetActive(true);

		}
    }

}