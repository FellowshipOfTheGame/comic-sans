using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Full_Health : MonoBehaviour {

    Image img;

	void Start () {
        img = GetComponent<Image>();
	}

    public void Turn_On()
    {
        img.enabled = true;
    }

    public void Turn_Off()
    {
        img.enabled = false;
    }
}
