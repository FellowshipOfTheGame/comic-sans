using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{
        
    [AddComponentMenu("Scripts/Projectiles/Boss/Bounce")]
    public class BossBounceProjectile : SimpleProjectile
    {

        [Tooltip("Amount of times for the projectile to bounce.")]
        [SerializeField] private int bounceAmount = 5;

        // Current amount of times the projectile has bounced.
        private int bounces = 0;

        protected override void OnEnable()
        {

            base.OnEnable();
            bounces = 0;

        }

        protected override void FixedUpdate () 
        {

            // Despawns the projectile if the bounce amount has been reached.
            if(bounces == bounceAmount)
            {
                Despawn();
                return;
            }
            
            // Detects if the projectile has hit a wall and changes it's direction.
            if(transform.position.x > SceneSettings.instance.positionConstraints.x)
            {
                Vector2 vel = new Vector2(-_rigidbody.velocity.x, _rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }
            else if(transform.position.x < -SceneSettings.instance.positionConstraints.x)
            {
                Vector2 vel = new Vector2(-_rigidbody.velocity.x, _rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }

            if(transform.position.y > SceneSettings.instance.positionConstraints.y)
            {
                Vector2 vel = new Vector2(_rigidbody.velocity.x, -_rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }
            else if(transform.position.y < -SceneSettings.instance.positionConstraints.y)
            {
                Vector2 vel = new Vector2(_rigidbody.velocity.x, -_rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }

        }
    }

}