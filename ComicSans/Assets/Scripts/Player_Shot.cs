using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shot : MonoBehaviour {

    [SerializeField] GameObject Bullet;
    [SerializeField] float spawn_time;
	
	void Update () {
        if (Input.GetButtonDown("Fire1"))
            InvokeRepeating("Shot", 0 , spawn_time);
        if (Input.GetButtonUp("Fire1"))
            CancelInvoke();
	}

    void Shot()
    {
        Instantiate(Bullet,this.transform);
    }
}
