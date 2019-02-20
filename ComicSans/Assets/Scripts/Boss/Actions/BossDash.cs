using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDash", menuName = "Boss/Dash", order = 4)]
public class BossDash : BossAction {

	public float aceleration = 10.0f;

	public float chargeTime;
	
    public Vector2 positionConstraints = new Vector2( 8, 8);

    public int bounceAmount = 3;

	// Used to know when to FlipX on the boss.
    public bool lookingRight;

    public List<AnimationSet> chargeAnimations;

    public List<AnimationSet> dashAnimations;

    public string audioName;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionDash(this));
    }
}