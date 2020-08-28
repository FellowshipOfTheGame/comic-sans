using UnityEngine;
using UnityEngine.EventSystems;

// Used to reselect UI in case the keyboard selection is lost.
public class ReselectUI : MonoBehaviour
{

    EventSystem eventSystem;

    public GameObject target;

    void Awake() {

        eventSystem = FindObjectOfType<EventSystem>();

    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {

            eventSystem.SetSelectedGameObject(target);

        }

    }
}
