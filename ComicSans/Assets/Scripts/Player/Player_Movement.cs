using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Scripts/Player/Movement")]
public class Player_Movement : MonoBehaviour {

    private Rigidbody2D rBody;
    private Vector2 vel;
    [SerializeField] private float speed;
    private Animator anim;
    private bool Going_Right=true;
    private Vector3 new_Scale;
    public Vector2 positionConstraints = new Vector2( 8, 8);

    // Use this for initialization
	void Start () {

        anim = Player_Manager.instance._animator;
        rBody = GetComponent<Rigidbody2D>();

	}
	
	// Update is called once per frame
	void Update () {

        vel.x = (Input.GetAxisRaw("Horizontal"));
        vel.y = Input.GetAxisRaw("Vertical");
        vel.Normalize();
        rBody.velocity = vel * speed;
        if (vel.x != 0)
            if(anim != null)                                                     //Define que o Player está se movendo para o Animator
                anim.SetBool("Mov_Horizontal", true);
        else
            if(anim != null)         
                anim.SetBool("Mov_Horizontal", false);
        if((vel.x<0  && Going_Right) || (vel.x > 0 && !Going_Right))        // Vira o player para a direção correta
        {
            Going_Right = !Going_Right;
            new_Scale = transform.localScale;
            new_Scale.x *= -1;
            transform.localScale = new_Scale;
        }

        // Garante que o player não escape da área de jogo.
        if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -positionConstraints.x, positionConstraints.x),
                                         Mathf.Clamp(transform.position.y, -positionConstraints.y, positionConstraints.y),
                                         0);
        

	}
}
