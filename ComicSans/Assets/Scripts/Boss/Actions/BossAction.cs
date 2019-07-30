﻿using UnityEngine;

namespace ComicSans.Boss.ActionSystem
{

	// Base script for the an action in the Boss ActionSystem.
	public abstract class BossAction : ScriptableObject {

		[HideInInspector]
		public BossScript caller;

		// Use this for initialization of the action.
		public abstract void DoAction ();

	}

}
