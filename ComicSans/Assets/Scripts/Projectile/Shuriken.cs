using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Shuriken")]
public class Shuriken : ProjectileBase{

    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject Boss;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int col_times = 3;
    [SerializeField] private Vector2 walls;
    private Vector2 direction2D;
    private int col=0;

    protected override void OnEnable()
    {
        col = 0;

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("Damage"))
        {
            if (fooObj.name == "XingLing")
            {
                Boss = fooObj;
            }
        }


        if (Boss != null)
        {
            Vector3 direction = transform.position - Boss.transform.position;
            direction.Normalize();
            direction2D = new Vector2(direction.x, direction.y);
            rb.velocity = direction2D * speed;
        }
        else
            Debug.Log("Boss not defined");


    }

    // Update is called once per frame
    void Update () {
        if (transform.position.x > walls.x && rb.velocity.x > 0 || transform.position.x < -walls.x && rb.velocity.x < 0)
        {
            direction2D.x = direction2D.x * -1;
            rb.velocity = direction2D * speed;
            col++;
        }
        if (transform.position.y > walls.y && rb.velocity.y > 0 || transform.position.y < -walls.y && rb.velocity.y < 0)
        {
            direction2D.y = direction2D.y * -1;
            rb.velocity = direction2D * speed;
            col++;
        }
        if (col >= col_times)
        {
            Despawn();
        }
    }
}
