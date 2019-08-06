using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{

    [AddComponentMenu("Scripts/Projectiles/Boss/Rotate")]
    public class BossRotateProjectile : ProjectileBase {

        [SerializeField] private float angularSpeed = 180;
        [SerializeField] private float rotateAmount = 180;

        protected override void OnEnable()
        {

            base.OnEnable();
            StartCoroutine(Rotate());

        }
        
        IEnumerator Rotate()
        {
            float curAngle = 0;

            while(angularSpeed > 0  && curAngle < rotateAmount || angularSpeed < 0  && curAngle > rotateAmount)
            {

                float angleStep = angularSpeed * Time.fixedDeltaTime;

                transform.Rotate(new Vector3(0, 0, angleStep));
                curAngle += angleStep;

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
