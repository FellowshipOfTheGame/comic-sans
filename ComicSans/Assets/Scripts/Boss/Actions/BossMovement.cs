using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newMovement", menuName = "Boss/Movement", order = 1)]
public class BossMovement : BossAction {

    public Vector2 positionTarget;
    public float velocityModifier;

    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionMove(this));
    }
}