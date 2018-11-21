using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newBossPhase", menuName = "Boss/Phase", order = 1)]
public class BossPhase : ScriptableObject {

    public BossPattern firstPattern;

    public int lifeToNextPhase = 50;

}