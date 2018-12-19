using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Player/Movement")]
public class Player_Movement : MonoBehaviour {

    private Rigidbody2D rBody;
    private Vector2 vel;
    [SerializeField] private float speed;
    private Animator Anim;
    private bool Going_Right=true;
    private Transform trans;
    private Vector3 new_Scale;
    
    // Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        trans = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        vel.x = (Input.GetAxisRaw("Horizontal"));
        vel.y = Input.GetAxisRaw("Vertical");
        vel.Normalize();
        rBody.velocity = vel * speed;
        if (vel.x != 0)                                                     //Define que o Player está se movendo para o Animator
            Anim.SetBool("Mov_Horizontal", true);
        else        
            Anim.SetBool("Mov_Horizontal", false);
        if((vel.x<0  && Going_Right) || (vel.x > 0 && !Going_Right))        // Vira o player para a direção correta
        {
            Going_Right = !Going_Right;
            new_Scale = trans.localScale;
            new_Scale.x *= -1;
            trans.localScale = new_Scale;
        }
	}
}
