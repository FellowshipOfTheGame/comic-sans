using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackMove", menuName = "Boss/Attack Move", order = 3)]
public class BossAttackMove : BossAction {

    public int numberOfSteps;

    public Vector2 positionTarget;
    public float velocityModifier;

    public bool stopBeforeAttack;
    public bool stopAfterAttack;
    public float idleTime;

    public string projectileId;
    public List<Vector2> projectileSpawns;

    public List<AnimationSet> movementAnimations;
    public List<AnimationSet> idleAnimations;
    public List<AnimationSet> attackAnimations;

    public bool idleAtEnd;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionAttackMove(this));
    }
}