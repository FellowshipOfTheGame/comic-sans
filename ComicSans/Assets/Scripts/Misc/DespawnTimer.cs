using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.PoolingSystem;

namespace ComicSans 
{

    // Despawns a pooled object after a time.
    public class DespawnTimer : PooledObject
    {

        [SerializeField] float despawnTime = 2f;

        void OnEnable()
        {

            StartCoroutine(DespawnTime());

        }

        IEnumerator DespawnTime() 
        {

            float time = 0;

            while (time < despawnTime)
            {
                
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

            }

            Despawn();

        }
    }

}
