using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newBossPhase", menuName = "Boss/Phase", order = 1)]
public class BossPhase : ScriptableObject {

    public Vector2 initialPosition = new Vector2(0, 3);

    public float invincibilityDuration = 1.0f;

    public RuntimeAnimatorController animationController;

    public BossPattern firstPattern;

    public int lifeToNextPhase = 1000;

}