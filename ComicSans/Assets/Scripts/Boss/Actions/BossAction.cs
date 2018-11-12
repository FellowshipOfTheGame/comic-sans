using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAction : ScriptableObject {

	[HideInInspector]
	public BossScript caller;

	// Use this for initialization of the action.
	public abstract void Start ();

}

[System.Serializable]
public struct AnimationSet {

	public string name;
	public int value;

}
