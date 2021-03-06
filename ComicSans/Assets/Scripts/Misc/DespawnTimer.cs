﻿using UnityEngine;

using System.Collections;

using ComicSans.PoolingSystem;

namespace ComicSans 
{

    // Despawns a pooled object after a time.
    [AddComponentMenu("Scripts/Misc/Despawn Timer")]
    public class DespawnTimer : PooledObject
    {

        [Tooltip("Time before despawning.")]
        [SerializeField] float despawnTime = 2f;

        void OnEnable()
        {

            StartCoroutine(DespawnTime());

        }

        IEnumerator DespawnTime() 
        {

            // Waits for the delay.
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
