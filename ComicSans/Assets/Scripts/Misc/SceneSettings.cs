using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour {

	public string music;
	public bool showHUD = true;	

	[System.Serializable]
	public class BossSettings
	{
		public bool bossScene = true;
		public GameObject boss;
		public Vector2 spawm;
	}
	public BossSettings bossSettings;

	void Start () {
		
		if(music != null && music != "none")
			AudioControlCenter.instance.Play(music);

		if(!showHUD)
			HUDController.instance.DisableHUD();
		else
			HUDController.instance.EnableHUD();

		if(bossSettings.bossScene)
			SpawnBoss();

	}

	public void SpawnBoss()
	{
		Instantiate(bossSettings.boss, bossSettings.spawm, new Quaternion());
	}
}
