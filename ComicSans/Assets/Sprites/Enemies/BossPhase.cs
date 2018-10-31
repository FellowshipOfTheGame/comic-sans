using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "newBossPhase", menuName = "Boss/Phase", order = 1)]
public class BossPhase : ScriptableObject {

    public BossPattern[] actionPatterns;

    public int lifeToNextPhase = 50;

}