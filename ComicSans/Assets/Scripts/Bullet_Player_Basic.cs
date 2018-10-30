using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Player_Basic : MonoBehaviour {

    [SerializeField] private float bullet_speed;
	void Start () {
        GetComponent<Rigidbody2D>().velocity = Vector2.up * bullet_speed;
	}
	
}
