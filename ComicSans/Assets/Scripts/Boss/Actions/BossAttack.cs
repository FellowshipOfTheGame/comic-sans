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
        Attack();
    }

    public void Attack()
	{

		// Sets the animation for this attack.
		caller.SetAnimation(animations);

		// Spawns all projectiles.
		if(caller.projectileDictionary.ContainsKey(projectileId))
			foreach(Vector2 spawn in projectileSpawns)
				caller.projectileDictionary[projectileId].Spawn(caller.transform.position + new Vector3( spawn.x, spawn.y, 0), caller.transform.rotation);
		else
			Debug.Log("BossAttack.Attack: Could not spawn projectile " + projectileId + " because there is no ObjectPool with that id!");

		caller.NextAction();

	}
}
