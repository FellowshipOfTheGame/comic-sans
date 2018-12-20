using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttack", menuName = "Boss/Attack", order = 2)]
public class BossAttack : BossAction {

    public string projectileId;
    public List<Vector2> projectileSpawns;

    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.ActionAttack(this);
    }
}
