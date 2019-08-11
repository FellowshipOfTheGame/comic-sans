using UnityEngine;

namespace ComicSans
{

    // Disables a GameObject if on a mobile platform.
    [AddComponentMenu("Scripts/Misc/Disable GameObject on Mobile")]   
    public class DisableGameObjectOnMobile : MonoBehaviour
    {

        [Tooltip("Target GameObject.")]
        [SerializeField] private GameObject target = null;


        void Start()
        {

            // Checks for mobile platform and disables the gameObject.
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                target.SetActive(false);
            
        }
    }

}
