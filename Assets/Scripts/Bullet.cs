using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;

    private void Update()//Auto Rotation Bullet
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
        {
            //Type1
            //Destroy(gameObject);
            //Type2 Object Pulling
            gameObject.SetActive(false);
        }
    }
}
