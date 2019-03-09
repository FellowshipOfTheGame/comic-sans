using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour {

	public static SceneSettings instance;

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
		
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;

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
