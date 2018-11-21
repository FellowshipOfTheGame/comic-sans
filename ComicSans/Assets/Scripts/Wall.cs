using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("colisão");
        if (collision.tag == "Bullet")
            Destroy(collision.gameObject);
    }
}
