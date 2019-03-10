using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttack", menuName = "Boss/Attack", order = 2)]
public class BossAttack : BossAction {

    [Tooltip("The id of the projectile on the Boss projectilePools.")]
    public string projectileId;
    [Tooltip("A projectile will be spawned in each projectileSpawn position relative to the Boss.")]
    public List<Vector2> projectileSpawns;

    [Tooltip("List of parameters (int) to be set on the Boss animator.")]
    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.ActionAttack(this);
    }
}
