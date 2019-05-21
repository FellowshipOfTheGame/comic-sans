using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAction : ScriptableObject {

	[HideInInspector]
	public BossScript caller;

	// Use this for initialization of the action.
	public abstract void DoAction ();

}

// Struct to be used in order of facilitating the setting of animations.
[System.Serializable]
public struct AnimationSet {

	public string name;
	public int value;

}

// Struct to be used in order of facilitating the setting of projectile spwans.
[System.Serializable]
public struct ProjectileSpawn {

	public Vector2 position;
	public float rotation;

}
