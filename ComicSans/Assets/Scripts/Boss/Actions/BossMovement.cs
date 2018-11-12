using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newMovement", menuName = "Boss/Movement", order = 1)]
public class BossMovement : BossAction {

    public Vector2 positionTarget;
    public float velocityModifier;

    public string animationName;
    public int animationValue;

    public override void Start()
    {
        caller.StartCoroutine(caller.ActionMove(this));
    }
}