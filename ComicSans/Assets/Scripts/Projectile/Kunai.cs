using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Xing Ling/Kunai")]
public class Kunai : BossProjectileBase {

    public Rigidbody2D rb;
    public float speed;
    [SerializeField] private GameObject Bossa;
    private Vector2 direction2D;

    protected override void OnEnable () {

        base.OnEnable();

        Vector3 direction = transform.position - BossScript.instance.transform.position;
        direction.Normalize();
        direction2D = new Vector2(direction.x, direction.y);
        rb.velocity = direction2D * speed;

        Vector3 diff = transform.position - BossScript.instance.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, 90 + rot_z);

        //Randomize a direction to the kunai withing the rotation range 
        //rotation = Random.Range(-RotationRange, RotationRange) * Mathf.Deg2Rad;
        //Rotate the kunai according to the rotation randomized above
        //newRotation.eulerAngles = new Vector3(0, 0, rotation);
        //transform.rotation = newRotation;
        //Move the kunai in the direction it is pointed
        //rb.velocity = new Vector2(Mathf.Sin(rotation), -Mathf.Cos(rotation)) * speed;
    }
}
