using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackMove", menuName = "Boss/Attack Move", order = 3)]
public class BossAttackMove : BossAction {

    public int numberOfSteps;

    public Vector2 positionTarget;
    public float velocityModifier;

    public float idleTime;

    public GameObject projectile;
    public List<Vector2> projectileSpawns;

    public string moveAnimationName;
    public int moveAnimationValue;

    public string attackAnimationName;
    public int attackAnimationValue;

    public string idleAnimationName;
    public int idleAnimationValue;

    public override void Start()
    {
        caller.currentCoroutine = caller.StartCoroutine(caller.ActionAttackMove(this));
    }
}