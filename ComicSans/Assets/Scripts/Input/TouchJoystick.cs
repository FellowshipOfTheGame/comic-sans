using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ComicSans.Input
{

    // Controls the touch joystick that moves the Player.
    [AddComponentMenu("Scripts/Misc/Touch Joystick")]
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

        [Tooltip("Radius of the joystick on the UI.")]
        [SerializeField] private float radius = 60;

        [Tooltip("Percentage zone of the joystick where there will be no movement.")]
        [Range(0, 1)]
        [SerializeField] private float deadZone = 0.75f;

        [Tooltip("Sensitivity of the joystick.")]
        [Range(0, 1)]
        [SerializeField] private float sensitivity = 0.75f;

        [Tooltip("Image used for the joystick.")]
        [SerializeField] private Image joystickGraphic = null;

        // Stores the default joystick color.
        private Color defaultColor;

        [Tooltip("Color of the joystick when being dragged.")]
        [SerializeField] private Color dragColor = Color.white;

        // Stores the default joystick position.
        private Vector3 defaultPos;

        // If is using Unity Remote on the Editor.
        private bool usingRemote = false;

        
        private void Start() 
        { 

            // Verifies if is on Editor with Unity Remote Connected
            usingRemote = false;
            #if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isRemoteConnected)
                    usingRemote = true;
            #endif    
            
            // Gets the joystick default position and color.
            defaultPos = transform.position; 
            defaultColor = joystickGraphic.color;

            // Resets the InputController.
            if(InputController.instance != null)
            {

                InputController.instance.xAxis = 0;
                InputController.instance.yAxis = 0;

            }
            
        }

        private void Update()
        {

            // Resets the joystick if in a state different than Play.
            if(InputController.instance.IsMobileDevice || usingRemote)
            {

                if(GameController.instance != null && GameController.instance.currentGameState != GameController.GameState.Play)
                {

                    // Resets the joystick position and color.
                    transform.position = defaultPos; 
                    joystickGraphic.color = defaultColor;

                    // Sends the inputs to stop to the InputController.
                    if(InputController.instance != null)
                    {

                        InputController.instance.xAxis = 0;
                        InputController.instance.yAxis = 0;

                    }

                }

            }

        }

        public void OnPointerDown(PointerEventData data) {

            // Paints the joystick the correct color.
            joystickGraphic.color = dragColor;

        }

        // Resets to the default position.
        public void OnPointerUp(PointerEventData data) 
        { 

            // Resets the joystick position and color.
            transform.position = defaultPos; 
            joystickGraphic.color = defaultColor;

            // Sends the inputs to stop to the InputController.
            if(InputController.instance != null)
            {

                InputController.instance.xAxis = 0;
                InputController.instance.yAxis = 0;

            }

        }

        public void OnDrag(PointerEventData data)
        {

            // Calulates the difference between.
            float deltaX = data.position.x - defaultPos.x;
            float deltaY = data.position.y - defaultPos.y;

            // Used to keep the joystick inside it's circle.
            float h = Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY);
            float sin = deltaX / h;
            float cos = deltaY / h;
            
            // Guarantees the deltas are inside the joystick and are values between -1 and 1.
            float capDeltaX = Mathf.Clamp(deltaX / radius, -1.0f, 1.0f);
            float capDeltaY = Mathf.Clamp(deltaY / radius, -1.0f, 1.0f);

            // Gives more customization to the joystick fell.
            float modifiedDeltaX = Mathf.Sign(capDeltaX) * Mathf.Clamp(sensitivity * ((Mathf.Abs(capDeltaX) - deadZone) / (1 - deadZone)), 0, 1);
            float modifiedDeltaY = Mathf.Sign(capDeltaY) * Mathf.Clamp(sensitivity * ((Mathf.Abs(capDeltaY) - deadZone) / (1 - deadZone)), 0, 1);
            

            // Sends the input to the InputController.
            if(InputController.instance != null && (InputController.instance.IsMobileDevice || usingRemote))
            {

                InputController.instance.xAxis = modifiedDeltaX;
                InputController.instance.yAxis = modifiedDeltaY;

            }

            // Moves the joystick.
            transform.position = new Vector3(defaultPos.x + (Mathf.Abs(capDeltaX) * sin * radius), defaultPos.y + (Mathf.Abs(capDeltaY) * cos * radius), 0);

        }

    }

}
