using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackMove", menuName = "Boss/Attack Move", order = 3)]
public class BossAttackMove : BossAction {

    [Tooltip("Number of times the Boss will stop and attack.")]
    public int numberOfSteps;

    [Tooltip("Where the Boss should move to.")]
    public Vector2 positionTarget;
    [Tooltip("Modifier to be applied to the Boss velocity.")]
    public float velocityModifier;

    [Tooltip("If the Boss should idle before attacking.")]
    public bool stopBeforeAttack;
    [Tooltip("If the Boss should idle after attacking.")]
    public bool stopAfterAttack;
    [Tooltip("The amount of time the Boss will wait when idleing.")]
    public float idleTime;

    [Tooltip("The id of the projectile on the Boss projectilePools.")]
    public string projectileId;
    [Tooltip("A projectile will be spawned in each projectileSpawn position relative to the Boss.")]
    public List<Vector2> projectileSpawns;

    [Tooltip("List of parameters (int) to be set on the Boss animator during movimentation.")]
    public List<AnimationSet> movementAnimations;
    [Tooltip("List of parameters (int) to be set on the Boss animator during idle time.")]
    public List<AnimationSet> idleAnimations;
    [Tooltip("List of parameters (int) to be set on the Boss animator attack.")]
    public List<AnimationSet> attackAnimations;

    [Tooltip("If the Boss should idle at the end of the attack.")]
    public bool idleAtEnd;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionAttackMove(this));
    }
}