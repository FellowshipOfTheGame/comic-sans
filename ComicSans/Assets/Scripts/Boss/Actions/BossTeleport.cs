using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Teleport", order = 5)]
public class BossTeleport : BossAction {

    public enum TeleportType {AnimationThenTP, TPThenAnimation, AnimatedTP, JustTP};

    public TeleportType teleportType;

    public Vector2 destination;

    public float delay;

    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionTeleport(this));
    }
}