using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIdle", menuName = "Boss/Teleport", order = 5)]
public class BossTeleport : BossAction {

    
    public enum TeleportType {AnimationThenTP, TPThenAnimation, AnimatedTP, JustTP};
    [Tooltip("How the Boss should teleport to.")]
    public TeleportType teleportType;

    [Tooltip("Where the Boss should")]
    public Vector2 destination;

    [Tooltip("Time to be used when the Boss needs to wait.")]
    public float delay;

    [Tooltip("List of parameters (int) to be set on the Boss animator.")]
    public List<AnimationSet> animations;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionTeleport(this));
    }
}