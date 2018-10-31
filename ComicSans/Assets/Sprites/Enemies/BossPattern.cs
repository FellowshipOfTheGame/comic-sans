using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "newBossPattern", menuName = "Boss/Pattern", order = 1)]
public class BossPattern : ScriptableObject {

    public enum Type { Movement, Attack, Mix};

    public Type patternType;

    public Movement[] movement;
    public BossAttack attack;

}