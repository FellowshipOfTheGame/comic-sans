using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "newBossAttack", menuName = "Boss/Attack", order = 1)]
public class BossAttack : ScriptableObject {

    public GameObject projectile;
    public Vector2[] projectileSpawns;

}