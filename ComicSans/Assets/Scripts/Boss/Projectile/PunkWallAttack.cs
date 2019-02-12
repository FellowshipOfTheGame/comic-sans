using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Punk/Wall Attack")]
public class PunkWallAttack : PooledObject {

	[SerializeField] private float appearDelay;
	[SerializeField] private float disappearDelay;
	[SerializeField] private GameObject wall;

	public string audioName;

	private float timer;

	void OnEnable()
	{

		timer = 0;

	}

	void Update ()
	{
		
		if(!wall.activeSelf)
		{
			timer += Time.deltaTime;
			if(timer >= appearDelay)
			{
				wall.SetActive(true);
				timer = 0;
				if(audioName != null && audioName != "")
					AudioControlCenter.instance.Play(audioName);
			}
		}
		else
		{
			timer += Time.deltaTime;
			if(timer >= disappearDelay)
				if(origin != null)
					Despawn();
				else
					Destroy(this.gameObject);
			
		}
	}
}
