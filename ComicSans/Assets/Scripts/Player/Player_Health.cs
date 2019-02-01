using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Scripts/Player/Health")]
public class Player_Health : MonoBehaviour {

    [SerializeField] private int hp = 3;
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
            if (value < 0)
                hp = 0;
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

    [SerializeField] private Image[] HealthIcons;

    [SerializeField] private float Invencibility;
    [SerializeField] private Transform Spawn_Point;

    private Animator anim;
    private Collider2D _collider;
    


    // Use this for initialization
    void Start () {

        anim = Player_Manager.instance._animator;
        if(anim == null)
            Debug.Log("(Player) Player has no animator!");

        _collider = GetComponent<Collider2D>();

        PositionIcons();
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Damage")
            Take_Damage();         

    }

    void Take_Damage()
    {

        Hp--;
        if (Hp > 0)
            StartCoroutine(Reset_Player());
        else
        {
            BossScript.instance.Win();
            Destroy(gameObject);
        }

    }

    IEnumerator Reset_Player()
    {
        float time = 0;

        transform.position = Spawn_Point.position;
        if(anim != null)
            anim.SetBool("Invencible", true);
        _collider.enabled = false;

        while(time < Invencibility)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(anim != null)
            anim.SetBool("Invencible", false);
        _collider.enabled = true;

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
