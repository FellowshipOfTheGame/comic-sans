using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDash", menuName = "Boss/Dash", order = 4)]
public class BossDash : BossAction {

    [Tooltip("How fast the Boss should accelerate when dashing.")]
	public float aceleration = 10.0f;

    [Tooltip("The time the Boss will wait before dashing.")]
	public float chargeTime;

    [Tooltip("The amount of times the Boss will touch the wall before stopping.")]
    public int bounceAmount = 5;

    [Tooltip("if the Boss sprite is looking to the right (Used to know when to flip the Boss X-axis to look at the player).")]
    public bool lookingRight;

    [Tooltip("List of parameters (int) to be set on the Boss animator during the time before the dash.")]
    public List<AnimationSet> chargeAnimations;

    [Tooltip("List of parameters (int) to be set on the Boss animator during the dash.")]
    public List<AnimationSet> dashAnimations;

    [Tooltip("The audio to be played during the dash.")]
    public AudioInfo dashAudio;

    public override void DoAction()
    {
        caller.StartCoroutine(Dash());
    }

    public IEnumerator Dash() {

		// Sets the charge animation.
		caller.SetAnimation(chargeAnimations);

        Vector3 defaultScale = caller.transform.localScale;

		// Plays the audio.
		if(AudioController.instance != null)
			if(dashAudio != null)
			AudioController.instance.Play(dashAudio);

		// Waits for the charging time.
		float time = 0f;
		while(time < chargeTime)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		// Gets the 3D target position.
		Vector2 dirVector;
		if(PlayerScript.instance != null) {

			// Sets the dash animation.
			caller.SetAnimation(dashAnimations);

			dirVector = PlayerScript.instance.transform.position - caller.transform.position;
			dirVector.Normalize();

			float accel = 0;

			if(!lookingRight && dirVector.x > 0)
				caller.transform.localScale = new Vector3(-caller.transform.localScale.x, caller.transform.localScale.y, caller.transform.localScale.z);
			else if(lookingRight && dirVector.x < 0)
				caller.transform.localScale = new Vector3(-caller.transform.localScale.x, caller.transform.localScale.y, caller.transform.localScale.z);
			else
				caller.transform.localScale = defaultScale;

			int bounces = 0;

			// Used to guarantee that bounces can't be accounted 2 times because of physics limitations.
			bool bounceLeft = false;
			bool bounceRight = false;
			bool bounceTop = false;
			bool bounceBottom = false;

			// Move the boss to the target position.
			while (bounces < bounceAmount) {			


				// Doesn't execute the code if time is stoped.
				if(Time.timeScale != 0)
				{
					// Moves to the target position.
					if(bounces == 0)
						accel += aceleration * Time.deltaTime;
					caller.transform.Translate(dirVector * accel * Time.fixedDeltaTime);

					yield return new WaitForFixedUpdate();

					if(Mathf.Abs(caller.transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(caller.transform.position.y) > SceneSettings.instance.positionConstraints.y)
					{

						if(caller.transform.position.x > SceneSettings.instance.positionConstraints.x && !bounceRight)
						{
							dirVector = new Vector2(-dirVector.x, dirVector.y);
							
							bounceLeft = false;
							bounceRight = true;
						}
						else if(caller.transform.position.x < -SceneSettings.instance.positionConstraints.x && !bounceLeft)
						{
							dirVector = new Vector2(-dirVector.x, dirVector.y);

							bounceLeft = true;
							bounceRight = false;
						}

						if(caller.transform.position.y > SceneSettings.instance.positionConstraints.y && !bounceTop)
						{
							dirVector = new Vector2(dirVector.x, -dirVector.y);

							bounceTop = true;
							bounceBottom = false;
						}
						else if(caller.transform.position.y < -SceneSettings.instance.positionConstraints.y && !bounceBottom)
						{
							dirVector = new Vector2(dirVector.x, -dirVector.y);

							bounceTop = false;
							bounceBottom = true;
						}

						bounces++;

					}
				} else yield return new WaitForEndOfFrame();
			}
		}
		else 
		{
			Debug.Log("BossScript.Dash: Player not found");
		}

		caller.transform.localScale = defaultScale;
		caller.NextAction();

	}
}