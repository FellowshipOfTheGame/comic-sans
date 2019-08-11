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

        
        private void Awake() 
        {

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
            {
                touchJoystick.SetActive(false);
                touchFire.gameObject.SetActive(false);
                touchPause.gameObject.SetActive(false);
            }

        }

        private void Update()
        {

            // Controls the input for mouse/keyboard and controller.
            if(!isMobileDevice) {

                // Takes input from regular sources.
                xAxis = UnityEngine.Input.GetAxis("Horizontal");
                yAxis = UnityEngine.Input.GetAxis("Vertical");

                if(UnityEngine.Input.GetButtonDown("Fire1"))
                    if(OnShotDown != null) OnShotDown();

                if(UnityEngine.Input.GetButtonDown("Pause"))
                    if(OnShotDown != null) OnPauseDown();

            }
        }

        // Enables or disables the UI touch controls.
        public void SetTouchUI(bool state)
        {

            touchJoystick.SetActive(state);
            touchFire.gameObject.SetActive(state);
            touchPause.gameObject.SetActive(state);

        }

        // Receives input for touch controls.
        public void InputTouch(string touchControl)
        {

            if(isMobileDevice) {

                Debug.LogWarning("InputController.TouchShot: Touch button activated on non-touch device!");  

            }

            if(touchControl == "shot")
                OnShotDown();
            else if(touchControl == "pause")
                OnPauseDown();

        }

    }

}