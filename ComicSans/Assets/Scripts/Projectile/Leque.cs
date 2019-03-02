using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Projectiles/Boss/Leque")]
public class Leque : ProjectileBase{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float center_x;
    [SerializeField] private float center_y;
    private float pi;
    public Transform _trasnform;

    protected override void OnEnable()
    {
        base.OnEnable();
        _trasnform = transform;
        pi = Mathf.PI;
        StartCoroutine(Spining()) ;
    }
    
    IEnumerator Spining()
    {
        float timer = 0;
        while(timer<2*pi){ 
        timer += Time.deltaTime * speed;
        _trasnform.position = new Vector3(Mathf.Cos(timer+pi/2) * width + center_x, Mathf.Sin(timer+pi/2) * height + center_y, 0);
        yield return new WaitForEndOfFrame();
        }
        Despawn();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {

        

    }
}
