using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace ComicSans.Projectiles.Boss
{
        
    [AddComponentMenu("Scripts/Projectiles/Boss/Bounce")]
    public class BossBounceProjectile : SimpleProjectile
    {

        [SerializeField] private int bounceAmount = 5;
        private int bounces = 0;

        protected override void OnEnable()
        {

            base.OnEnable();

            bounces = 0;

        }

        protected override void FixedUpdate () 
        {

            if(bounces == bounceAmount)
            {
                Despawn();
                return;
            }

            Vector2 vel;

            if(transform.position.x > SceneSettings.instance.positionConstraints.x)
            {
                vel = new Vector2(-_rigidbody.velocity.x, _rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }
            else if(transform.position.x < -SceneSettings.instance.positionConstraints.x)
            {
                vel = new Vector2(-_rigidbody.velocity.x, _rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }

            if(transform.position.y > SceneSettings.instance.positionConstraints.y)
            {
                vel = new Vector2(_rigidbody.velocity.x, -_rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }
            else if(transform.position.y < -SceneSettings.instance.positionConstraints.y)
            {
                vel = new Vector2(_rigidbody.velocity.x, -_rigidbody.velocity.y);
                _rigidbody.velocity = vel;

                bounces++;
            }

        }
    }

}