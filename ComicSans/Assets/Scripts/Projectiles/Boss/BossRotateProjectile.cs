using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{

    // Rotates a projectile.
    [AddComponentMenu("Scripts/Projectiles/Boss/Rotate")]
    public class BossRotateProjectile : ProjectileBase {

        [Tooltip("Rotation velocity (degrees/s).")]
        [SerializeField] private float angularSpeed = 180;

        [Tooltip("Angle to rotate by (degrees).")]
        [SerializeField] private float rotateAmount = 180;

        protected override void OnEnable()
        {

            base.OnEnable();
            StartCoroutine(Rotate());

        }
        
        IEnumerator Rotate()
        {
            float curAngle = 0;

            // Rotates until reaching the angle.
            while(angularSpeed > 0  && curAngle < rotateAmount || angularSpeed < 0  && curAngle > rotateAmount)
            {

                float angleStep = angularSpeed * Time.fixedDeltaTime;

                transform.Rotate(new Vector3(0, 0, angleStep));
                curAngle += angleStep;

                yield return new WaitForFixedUpdate();

            }

            Despawn();

        }

        protected override void OnCollisionEnter2D(Collision2D collision) {}

    }
    
}
