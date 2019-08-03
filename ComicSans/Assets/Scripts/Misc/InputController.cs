using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans
{

    // Controls the user input.
    [AddComponentMenu("Scripts/Controller/Input")]
    public class InputController : MonoBehaviour
    {
        
        public static InputController instance;

        // Stores if the game is being played on Android or iOS.
        public bool isTouchDevice = false;

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
                isTouchDevice = true;
            else
                isTouchDevice = false;

            touchJoystick = GameObject.Find("TOUCH_JOYSTICK");
            touchFire = GameObject.Find("TOUCH_FIRE");
            touchPause = GameObject.Find("TOUCH_PAUSE");
            
            if(!isTouchDevice)
            {
                touchJoystick.SetActive(false);
                touchFire.gameObject.SetActive(false);
                touchPause.gameObject.SetActive(false);
            }

        }

        private void Update()
        {

            // Controls the input for mouse/keyboard and controller.
            if(!isTouchDevice) {

                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Vertical");

                if(Input.GetButtonDown("Fire1"))
                    if(OnShotDown != null) OnShotDown();

                if(Input.GetButtonDown("Cancel"))
                    if(OnShotDown != null) OnPauseDown();

            }
        }

        public void SetTouchUI(bool state)
        {

            touchJoystick.SetActive(state);
            touchFire.gameObject.SetActive(state);
            touchPause.gameObject.SetActive(state);

        }

        public void InputTouch(string touchControl)
        {

            if(isTouchDevice) {

                Debug.LogWarning("InputController.TouchShot: Touch button activated on non-touch device!");  

            }

            if(touchControl == "shot")
                OnShotDown();
            else if(touchControl == "pause")
                OnPauseDown();

        }

    }

}