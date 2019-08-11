using UnityEngine;
using UnityEngine.UI;

namespace ComicSans.Input
{

    // Intermediates communication between UI controls and the InputController.
    [AddComponentMenu("Scripts/Misc/UI Touch Handler")]
    public class UITouchHandler : MonoBehaviour
    {
        // Informs the InputController that the shot button has been pressed.
        public void TouchShot() { InputController.instance.InputTouch("shot"); }

        // Informs the InputController that the pause button has been pressed.
        public void TouchPause() { InputController.instance.InputTouch("pause"); }

    }

}