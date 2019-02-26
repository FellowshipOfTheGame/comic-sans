﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Idle", order = 6)]
public class BossIdle : BossAction {

    [Tooltip("The amount of time the Boss will idle.")]
    public float idleTime;

    [Tooltip("List of parameters (int) to be set on the Boss animator.")]
    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionIdle(this));
    }
}