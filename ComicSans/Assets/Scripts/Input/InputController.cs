using UnityEngine;
using UnityEngine.UI;

namespace ComicSans.Input
{

    // Controls the user input.
    [AddComponentMenu("Scripts/Controller/Input")]
    public class InputController : MonoBehaviour
    {
        
        public static InputController instance;

        // Stores if the game is being played on Android or iOS.
        private bool isMobileDevice;
        public bool IsMobileDevice {

            get { return isMobileDevice; }

        }

        // Stores the values the movement axis.
        public float xAxis, yAxis;

        public delegate void OnInput();

        // Used to call functions that need to happen when the Shot button is pressed.
        public event OnInput OnShotDown;

        // Used to call functions that need to happen when the Pause button is pressed.
        public event OnInput OnPauseDown;

        public GameObject touchJoystick = null;
        public GameObject touchFire = null;
        public GameObject touchPause = null;

        // If is using Unity Remote on the Editor.
        private bool usingRemote = false;

        
        private void Awake() 
        {

            // Verifies if is on Editor with Unity Remote Connected
            usingRemote = false;
            #if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isRemoteConnected)
                    usingRemote = true;
            #endif

            // Destroy this object if a previous instance already exists.
			if(instance != null)
			{
				Destroy(gameObject);
				return;
			}

            // Creates a singleton of this script.
			instance = this;
			DontDestroyOnLoad(gameObject);

            // Detects if the game is being played on a touch device.
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                isMobileDevice = true;
            else
                isMobileDevice = false;

            // Finds the touch controlls.
            touchJoystick = GameObject.Find("TOUCH_JOYSTICK");
            touchFire = GameObject.Find("TOUCH_FIRE");
            touchPause = GameObject.Find("TOUCH_PAUSE");
            
            // Disables the touch controls if not on a mobile platform.
            if(!isMobileDevice)
                SetTouchUI(false);
            else // Unlocks the mouse for dragging on a touch screen to work.
                Cursor.lockState = CursorLockMode.None;

        }

        private void Update()
        {

            // Controls the input for mouse/keyboard and controller.
            if(!isMobileDevice && !usingRemote) {

                // Takes input from regular sources.
                xAxis = UnityEngine.Input.GetAxis("Horizontal");
                yAxis = UnityEngine.Input.GetAxis("Vertical");

                if(UnityEngine.Input.GetButtonDown("Fire1"))
                    if(OnShotDown != null) OnShotDown();

                if(UnityEngine.Input.GetButtonDown("Pause"))
                    if(OnPauseDown != null) OnPauseDown();

            }
        }

        // Enables or disables the UI touch controls.
        public void SetTouchUI(bool state)
        {

            touchJoystick.SetActive(state);
            touchFire.SetActive(state);
            touchPause.SetActive(state);

        }

        // Receives input for touch controls.
        public void InputTouch(string touchControl)
        {

            if(!isMobileDevice)
                Debug.LogWarning("InputController.InputTouch: Touch button activated on non-touch device!");  

            if(touchControl == "shot") 
            {
                if(OnShotDown != null) OnShotDown();
            }
            else if(touchControl == "pause")
            {
                if(OnPauseDown != null) OnPauseDown();
            }
            else
                Debug.LogWarning("InputController.InputTouch: Invalid touch control!");  

        }

    }

}