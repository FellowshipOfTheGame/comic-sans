using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "newMovement", menuName = "Boss/Movement", order = 1)]
public class Movement : ScriptableObject {

    
    public Vector2 positionTarget;
    public float velocityModifier = 1.0f;

}