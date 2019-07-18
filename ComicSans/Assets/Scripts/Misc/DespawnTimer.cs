using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
