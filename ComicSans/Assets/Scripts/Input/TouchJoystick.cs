using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ComicSans.Input
{

    // Controls the touch joystick that moves the Player.
    [AddComponentMenu("Scripts/Misc/Touch Joystick")]
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

        [Tooltip("Radius of the joystick on the UI.")]
        public float radius = 60;

        // Stores the default joystick position.
        private Vector3 defaultPos;

        private void Start() { defaultPos = transform.position; }

        public void OnPointerDown(PointerEventData data) {}

        // Resets to the default position.
        public void OnPointerUp(PointerEventData data) { transform.position = defaultPos; }

        public void OnDrag(PointerEventData data)
        {

            // Calulates the difference between.
            float deltaX = data.position.x - defaultPos.x;
            float deltaY = data.position.y - defaultPos.y;

            // Used to keep the joystick inside it's circle.
            float h = Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY);
            float sin = deltaX / h;
            float cos = deltaY / h;
            
            // Guarantees the deltas are inside the joystick.
            float capDeltaX = Mathf.Clamp(deltaX / radius, -1.0f, 1.0f);
            float capDeltaY = Mathf.Clamp(deltaY / radius, -1.0f, 1.0f);

            // Sends the input to the InputController.
            if(InputController.instance.IsMobileDevice)
            {
                InputController.instance.xAxis = capDeltaX;
                InputController.instance.yAxis = capDeltaY;
            }

            // Moves the joystick.
            transform.position = new Vector3(defaultPos.x + (Mathf.Abs(capDeltaX) * sin * radius), defaultPos.y + (Mathf.Abs(capDeltaY) * cos * radius), 0);

        }

    }

}
