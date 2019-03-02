﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Scripts/Player/Health")]
public class Player_Health : MonoBehaviour {

    [SerializeField] private int hp;
    [SerializeField] private float Invencibility;
    [SerializeField] private Transform Spawn_Point;
    private Transform Player_trans;
    private Animator anim;
    [SerializeField] private Image[] HealthIcons;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {

            if (value >= 3)
                hp = 3;
            else
                hp = value;

            for(int i = 0; i < 3; i++) 
        {
                if(i < value)
                    HealthIcons[i].enabled = true;
                else
                    HealthIcons[i].enabled = false;
            }

        }
    }
    [SerializeField] private int Init_Hp;


    // Use this for initialization
    void Start () {

        hp = Init_Hp;
        Player_trans = gameObject.transform;
        anim = GetComponent<Animator>();
        PositionIcons();
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Damage")
            Take_Damage();         

    }

    void Take_Damage()
    {

        if(!anim.GetBool("Invencible")) 
        {
            Hp--;
            if (Hp > 0)
                StartCoroutine(Reset_Player());
            else
                Destroy(this.gameObject);
        }
    }

    IEnumerator Reset_Player()
    {
        Player_trans.position = Spawn_Point.position;
        anim.SetBool("Invencible", true);
        yield return new WaitForSeconds(Invencibility);
        anim.SetBool("Invencible", false);

    }

    private void PositionIcons() {

        // Positions the health icons.
        for(int i = 0; i < HealthIcons.Length; i++) {

            Vector3 iconPosition = HealthIcons[i].rectTransform.position;
            iconPosition.x = (Screen.width / 2) - (Screen.height / 1.66f) + (60 * i);
            HealthIcons[i].rectTransform.position = iconPosition; 

        }
        
    }
}