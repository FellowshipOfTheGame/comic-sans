using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.PoolingSystem;

namespace ComicSans
{

    // Base code for entities i.e. Boss and Player.
    public abstract class EntityScript : MonoBehaviour
    {

        [System.Serializable]
        public class Health
        {

            [HideInInspector] public EntityScript parent;

            public int maxHp;

            public int hp = 3;
            public int Hp
            {
                get
                {
                    return hp;
                }
                set
                {

                    // Guaratees the life is not higher than max.
                    if (value >= maxHp)
                        hp = maxHp;
                    if (value < 0)
                        hp = 0;
                    else
                        hp = value;

                    parent.UpdateLifeHUD();

                }
            }

            public float invincibilityTime = 2f;
        }
        public Health health;

        public float velocity = 4.0f;

        protected bool invincible;

        [Tooltip("Audios to be pre-initialized at Awake.")]
        [SerializeField] protected List<AudioInfo> warmupAudio;

        [Tooltip("Prefab pools to be pre-initialized at Awake.")]
        [SerializeField] protected List<PoolInfo> warmupPools;
        
        protected virtual void Awake()
        {

            // Assigns this script as the parent of the Health sub-class.
            health.parent = this;

            // Initializes the Hp value.
            health.Hp = health.maxHp;

            // Guarantees invincibility is disabled.
            invincible = false;

            // Pre-initializes audios at startup, so lag spikes in less powerfull devices are concentrated at the beginning. 
            foreach(AudioInfo audio in warmupAudio)
                AudioController.instance.Add(audio);

            // Pre-initializes ObjectPools at startup, so lag spikes in less powerfull devices are concentrated at the beginning.
            foreach(PoolInfo pool in warmupPools)
                PoolingController.instance.Add(pool);

        }

        protected abstract void UpdateLifeHUD();

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {

            // Takes damage.
            if (collision.collider.tag == "Damage")
                Damage();    

        }

        protected abstract void Damage();

        protected abstract IEnumerator Reset(float invincibilityMultiplier);

        protected abstract void Die();

    }
    
}
