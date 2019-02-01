using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Movement))]
[RequireComponent(typeof(Player_Health))]
[RequireComponent(typeof(Player_Shot))]
[AddComponentMenu("Scripts/Player/Manager")]
public class Player_Manager : MonoBehaviour {

	public static Player_Manager instance;

	[HideInInspector] public Player_Movement movement;
	[HideInInspector] public Player_Health health;
	[HideInInspector] public Player_Shot shooting;
	[HideInInspector] public Animator _animator;
	
	void Start () 
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		movement = GetComponentInChildren<Player_Movement>();
		health = GetComponentInChildren<Player_Health>();
		shooting = GetComponentInChildren<Player_Shot>();
		_animator = GetComponentInChildren<Animator>();
		if(_animator == null)
			Debug.Log("(Player_Manager) No Animator found on player!");

	}
}
