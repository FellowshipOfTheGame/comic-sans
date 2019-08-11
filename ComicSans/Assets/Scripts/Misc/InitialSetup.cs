using UnityEngine;

using ComicSans.UI;

namespace ComicSans
{

    // Performs the initial setup to run the game.
    [AddComponentMenu("Scripts/Misc/Initial Setup")]
    public class InitialSetup : MonoBehaviour
    {

        [Tooltip("UIController to be used.")]
        [SerializeField] private UIController uiController = null;

        void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Sets the time scale to default.
            Time.timeScale = 1;   

            SceneSettings.instance.OnReady();
            uiController.FadeInScene();
                
        }
    }
}
