using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDash", menuName = "Boss/Dash", order = 4)]
public class BossDash : BossAction {

    [Tooltip("How fast the Boss should accelerate when dashing.")]
	public float aceleration = 10.0f;

    [Tooltip("The time the Boss will wait before dashing.")]
	public float chargeTime;
	
    [Tooltip("The bounds of the game area.")]
    public Vector2 positionConstraints = new Vector2( 8, 8);

    [Tooltip("The amount of times the Boss will touch the wall before stopping.")]
    public int bounceAmount = 5;

    [Tooltip("if the Boss sprite is looking to the right (Used to know when to flip the Boss X-axis to look at the player).")]
    public bool lookingRight;

    [Tooltip("List of parameters (int) to be set on the Boss animator during the time before the dash.")]
    public List<AnimationSet> chargeAnimations;

    [Tooltip("List of parameters (int) to be set on the Boss animator during the dash.")]
    public List<AnimationSet> dashAnimations;

    [Tooltip("The id of the audio on the AudioControlCenter to be played during the dash.")]
    public string audioId;

    public override void DoAction()
    {
        caller.StartCoroutine(caller.ActionDash(this));
    }
}