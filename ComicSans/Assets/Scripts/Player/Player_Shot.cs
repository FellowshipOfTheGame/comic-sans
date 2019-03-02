using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Player/Shot")]
public class Player_Shot : MonoBehaviour {

    [SerializeField] float spawn_time;
    [SerializeField] ObjectPool bulletPool;

    public delegate void Shot();

    public static event Shot OnShot;

    private void Awake()
    {
        OnShot += Shot_Standard;
    }

    void Update () {
        if (Input.GetButtonDown("Fire1"))
            InvokeRepeating("Shoting", 0, spawn_time);
        if (Input.GetButtonUp("Fire1"))
            CancelInvoke();
	}

    void Shoting()
    {
        OnShot();
    }

    void Shot_Standard()
    {

        bulletPool.Spawn(transform.position + new Vector3( 0, 1, 0), transform.rotation);
    
    }
}
