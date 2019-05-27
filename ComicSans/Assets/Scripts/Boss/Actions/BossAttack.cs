using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttack", menuName = "Boss/Attack", order = 2)]
public class BossAttack : BossAction {

    [Tooltip("The projectile pool to spawn the projectile from.")]
    public PoolInfo projectilePool;
    [Tooltip("A projectile will be spawned in each projectileSpawn position relative to the Boss.")]
    public List<ProjectileSpawn> projectileSpawns;

    [Tooltip("List of parameters (int) to be set on the Boss animator.")]
    public List<AnimationSet> animations;

    public override void DoAction()
    {
        Attack();
    }

    public void Attack()
	{

		// Sets the animation for this attack.
		caller.SetAnimation(animations);
		foreach(ProjectileSpawn spawn in projectileSpawns)
			PoolingController.instance.Spawn(projectilePool, caller.transform.position + new Vector3( spawn.position.x, spawn.position.y, 0), Quaternion.Euler(0, 0, spawn.rotation));

		caller.NextAction();

	}
}
