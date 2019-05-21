using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Bounce")]
public class BossBounceProjectile : BossProjectileBase{

    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int col_times = 3;
    [SerializeField] private Vector2 walls;
    private Vector2 direction2D;
    private int col=0;

    protected override void OnEnable()
    {

        base.OnEnable();

        col = 0;

        Vector3 direction = transform.position - BossScript.instance.transform.position;
        direction.Normalize();
        direction2D = new Vector2(direction.x, direction.y);
        rb.velocity = direction2D * speed;

        StartCoroutine(Movimentation());

    }

    IEnumerator Movimentation () {

        while(col < col_times)
        {

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

            yield return new WaitForEndOfFrame();

        }

        Despawn();

    }
}
