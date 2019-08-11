using UnityEngine;

namespace ComicSans
{

    // Changes a sprite if on a mobile platform.
    [AddComponentMenu("Scripts/Misc/Change Sprite on Mobile")]   
    public class ChangeSpriteOnMobile : MonoBehaviour
    {

        [Tooltip("Target SpriteRenderer.")]
        [SerializeField] private SpriteRenderer target = null;
        [Tooltip("Sprite to be changed to.")]
        [SerializeField] private Sprite newSprite = null;


        void Start()
        {

            // Checks for mobile platform and changes the sprite.
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                target.sprite = newSprite;
            
        }
    }

}
