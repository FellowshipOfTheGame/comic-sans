using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttack", menuName = "Boss/Attack", order = 2)]
public class BossAttack : BossAction {

    public GameObject projectile;
    public List<Vector2> projectileSpawns;

    public List<AnimationSet> animations;

    public override void Start()
    {
        caller.ActionAttack(this);
    }
}
