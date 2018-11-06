using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Moviment : MonoBehaviour {

    private Rigidbody2D rBody;
    private Vector2 vel;
    [SerializeField] private float speed; 
    
    // Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        vel.x = Input.GetAxisRaw("Horizontal");
        vel.y = Input.GetAxisRaw("Vertical");
        vel.Normalize();
        rBody.velocity = vel * speed;
	}

   


}
