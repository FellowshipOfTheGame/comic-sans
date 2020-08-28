using UnityEngine;

public class HideInWebGL : MonoBehaviour
{
    
    public GameObject target;

    void Start()
    {

        #if UNITY_WEBGL

            target.SetActive(false);

        #endif

    }
}
