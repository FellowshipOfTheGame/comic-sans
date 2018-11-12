using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Idle", order = 4)]
public class BossIdle : BossAction {

    public float idleTime;

    public string idleAnimationName;
    public int idleAnimationValue;

    public override void Start()
    {
        caller.ActionIdle(this);
    }
}