using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Shoot : MonoBehaviour
{
    public Rewired.Player playerController;

    public Transform firePoint;
    public GameObject bullet;

    public float speed = 20f;

    public Vector2 aimDirection;

    void Start()
    {
        playerController = GetComponent<PlayerController>().playerController;

        aimDirection = new Vector2 (0f, 0f);
    }


    void Update()
    {
        
        float v = playerController.GetAxis("AimVertical");
        float h = playerController.GetAxis("AimHorizontal");

        aimDirection.x = h;
        aimDirection.y = v;
        aimDirection.Normalize();

        Vector2 bulletDirection = (Vector2) transform.position - aimDirection;
        bulletDirection.Normalize();
        //transform.Rotate (v, h, 0);

        //firePoint.transform.rotation = Quaternion.LookRotation(bulletDirection);
        firePoint.transform.rotation = Quaternion.Euler(bulletDirection.x, 0, bulletDirection.y);

        Debug.Log(bulletDirection);

        if (playerController.GetButtonDown("Fire"))
        {
            GameObject rocket = Instantiate(bullet, transform.position, firePoint.rotation);
            rocket.GetComponent<Rigidbody2D>().velocity = speed*bulletDirection;
        }
    }

}
