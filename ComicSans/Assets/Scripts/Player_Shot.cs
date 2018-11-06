using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shot : MonoBehaviour {

    [SerializeField] GameObject Bullet;
    [SerializeField] float spawn_time;

    public delegate void Shot();

    public static event Shot OnShot;

    private void Awake()
    {
        OnShot += Shot_Standard;
    }

    void Update () {
        if (Input.GetButtonDown("Fire1"))
            InvokeRepeating("Shoting", 0 , spawn_time);
        if (Input.GetButtonUp("Fire1"))
            CancelInvoke();
	}

    void Shoting()
    {
        OnShot();
    }

    void Shot_Standard()
    {
        Instantiate(Bullet, this.transform);
    }
}
