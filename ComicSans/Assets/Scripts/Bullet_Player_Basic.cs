using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Player_Basic : MonoBehaviour {

    [SerializeField] private float bullet_speed;
    [SerializeField] private float max_y;

	void Awake () {
        GetComponent<Rigidbody2D>().velocity = Vector2.up * bullet_speed;
	}
	
    void Update () {

        if(transform.position.y > 10) Destroy(gameObject);

    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        Destroy(gameObject);
    }

}
