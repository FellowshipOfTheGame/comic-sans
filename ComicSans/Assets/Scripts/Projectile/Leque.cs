using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Xing Ling/Leque")]
public class Leque : ProjectileBase {

    [SerializeField] private float speed = 5f;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float center_x;
    [SerializeField] private float center_y;

    protected override void OnEnable()
    {

        base.OnEnable();
        StartCoroutine(Spining());

    }
    
    IEnumerator Spining()
    {
        float timer = 0;

        while(timer < 2 * Mathf.PI)
        { 
            timer += Time.fixedDeltaTime * speed;
            transform.position = new Vector3(Mathf.Cos(timer+Mathf.PI/2) * width + center_x, Mathf.Sin(timer+Mathf.PI/2) * height + center_y, 0);
            yield return new WaitForFixedUpdate();
        }

        if(origin != null)
			Despawn();
		else
			Destroy(this.gameObject);
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {}

}
