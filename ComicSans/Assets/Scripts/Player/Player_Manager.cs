using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Movement))]
[RequireComponent(typeof(Player_Health))]
[RequireComponent(typeof(Player_Shot))]
[AddComponentMenu("Scripts/Player/Manager")]
public class Player_Manager : MonoBehaviour {

	public static Player_Manager manager;

	[HideInInspector] public Player_Movement movement;
	[HideInInspector] public Player_Health health;
	[HideInInspector] public Player_Shot shooting;
	
	void Start () 
	{
		if(manager != null)
		{
			Destroy(gameObject);
			return;
		}

		manager = this;

		movement = GetComponentInChildren<Player_Movement>();
		health = GetComponentInChildren<Player_Health>();
		shooting = GetComponentInChildren<Player_Shot>();

	}
}
