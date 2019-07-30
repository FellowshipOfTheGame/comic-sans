using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans
{

    // Controls the user input.
    [AddComponentMenu("Scripts/Controller/Input")]
    public class InputController : MonoBehaviour
    {
        
        // Stores if the game is being played on Android or iOS.
        private bool isTouchDevice;

        // Stores the values the movement axis.
        public float xAxis, yAxis;

        public delegate void OnInput();

        // Used to call functions that need to happen when the Shot button is pressed.
        public event OnInput OnShotDown;

        // Used to call functions that need to happen when the Pause button is pressed.
        public event OnInput OnPauseDown;

        private void Awake() 
        {

            // Detects if the game is being played on a touch device.
            #if UNITY_IPHONE
                isTouchDevice = true;
            #elif UNITY_ANDROID
                isTouchDevice = true;
            #else
                isTouchDevice = false;
            #endif

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

            } else {

                // TODO

            }
        }

    }

}