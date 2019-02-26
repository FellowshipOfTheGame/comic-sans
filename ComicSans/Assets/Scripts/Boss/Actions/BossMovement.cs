using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newMovement", menuName = "Boss/Movement", order = 1)]
public class BossMovement : BossAction {

    [Tooltip("Where the Boss should move to.")]
    public Vector2 positionTarget;
    [Tooltip("Modifier to be applied to the Boss velocity.")]
    public float velocityModifier;

    [Tooltip("List of parameters (int) to be set on the Boss animator.")]
    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionMove(this));
    }
}