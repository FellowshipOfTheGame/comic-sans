using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{

    [AddComponentMenu("Scripts/Projectiles/Boss/Spin")]
    public class BossSpinProjectile : ProjectileBase {

        [SerializeField] private float speed = 5f;
        [SerializeField] private float width = 3.5f;
        [SerializeField] private float height = 4.3f;
        [SerializeField] private Vector2 center = Vector2.zero;

        protected override void OnEnable()
        {

            base.OnEnable();
            StartCoroutine(Spining());

        }
        
        IEnumerator Spining()
        {
            float angle = 0;

            while(angle < 2 * Mathf.PI)
            { 
                angle += Time.fixedDeltaTime * speed;
                transform.position = new Vector3(Mathf.Cos(angle+Mathf.PI/2) * width + center.x, Mathf.Sin(angle+Mathf.PI/2) * height + center.y, 0);
                yield return new WaitForFixedUpdate();
            }

            if(origin != null)
                Despawn();
            else
                Destroy(this.gameObject);
        }

        protected override void OnCollisionEnter2D(Collision2D collision) {}

    }
    
}
