using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour {

	public string music;
	public bool showHUD = true;	

	void Start () {
		
		if(music != null && music != "none")
			AudioControlCenter.instance.Play(music);

		if(!showHUD)
			HUDController.instance.DisableHUD();
		else
			HUDController.instance.EnableHUD();

	}
}
