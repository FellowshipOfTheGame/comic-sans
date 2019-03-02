using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newBossPattern", menuName = "Boss/Pattern", order = 1)]
public class BossPattern : ScriptableObject {

    public int chance;
    public List<BossPattern> nextPattern;
    public List<BossAction> actions;    

}