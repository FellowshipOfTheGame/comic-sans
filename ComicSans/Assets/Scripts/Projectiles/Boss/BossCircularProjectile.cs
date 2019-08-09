using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{

    // Peojectile that moves in a circular trajectory forming an elipses.
    [AddComponentMenu("Scripts/Projectiles/Boss/Circular")]
    public class BossCircularProjectile : ProjectileBase {

        [Tooltip("Speed in which the projectile performs the trajectory.")]
        [SerializeField] private float speed = 5f;

        [Tooltip("Width of the circular trajectory.")]
        [SerializeField] private float width = 3.5f;

        [Tooltip("Height of the circular trajectory.")]
        [SerializeField] private float height = 4.3f;

        [Tooltip("Center of the circular trajectory.")]
        [SerializeField] private Vector2 center = Vector2.zero;

        protected override void OnEnable()
        {

            base.OnEnable();
            StartCoroutine(Spining());

        }
        
        IEnumerator Spining()
        {
            float angle = 0;

            // Moves the projectile until returning to the start of the circular trajetory.
            while(angle < 2 * Mathf.PI)
            { 
                angle += Time.fixedDeltaTime * speed;

                // Calculates and sets the position based on the angle.
                transform.position = new Vector3(Mathf.Cos(angle+Mathf.PI/2) * width + center.x, Mathf.Sin(angle+Mathf.PI/2) * height + center.y, 0);
                
                yield return new WaitForFixedUpdate();
            }

            Despawn();
            
        }

    }
    
}
