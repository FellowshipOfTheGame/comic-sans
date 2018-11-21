using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour {

    [SerializeField] private int hp;
    [SerializeField] private float Invencibility;
    [SerializeField] private Transform Spawn_Point;
    private Transform Player_trans;
    private Collider2D col;
    private Animator anim;
    Full_Health Full_Icon;
    Mid_Health Mid_Icon;
    Low_Health Low_Icon;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            if (value >= 3)
            {
                hp = 3;
                Full_Icon.Turn_On();
                Mid_Icon.Turn_On();
                Low_Icon.Turn_On();
            }
            if (value == 2)
            {
                hp = 2;
                Full_Icon.Turn_Off();
                Mid_Icon.Turn_On();
                Low_Icon.Turn_On();
            }
            if(value == 1)
            {
                hp = 1;
                Full_Icon.Turn_Off();
                Mid_Icon.Turn_Off();
                Low_Icon.Turn_On();
            }
        }
    }
    [SerializeField] private int Init_Hp;


    // Use this for initialization
    void Start () {
        hp = Init_Hp;
        Player_trans = gameObject.transform;
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        Full_Icon = GameObject.FindGameObjectWithTag("Full_Health").GetComponent<Full_Health>();
        Mid_Icon = GameObject.FindGameObjectWithTag("Mid_Health").GetComponent<Mid_Health>();
        Low_Icon = GameObject.FindGameObjectWithTag("Low_Health").GetComponent<Low_Health>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy_Bullet")
        {
            Take_Damage();
            Debug.Log("colisão Player");
        }                
    }

    void Take_Damage()
    {
        if (hp > 1)
        {
            Hp -= 1;
            StartCoroutine(Reset_Player());
        }
        else
            Destroy(this.gameObject);
    }

    IEnumerator Reset_Player()
    {
        Player_trans.position = Spawn_Point.position;
        col.enabled = false;
        anim.SetBool("Invencible", true);
        yield return new WaitForSeconds(Invencibility);
        anim.SetBool("Invencible", false);
        col.enabled = true;
    }
}
