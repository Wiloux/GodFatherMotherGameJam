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

        Vector2 firePoint2D = new Vector2(firePoint.position.x, firePoint.position.y);

        firePoint.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, aimDirection)); 

        Vector2 bulletDirection = aimDirection - firePoint2D;
        bulletDirection.Normalize();
        //transform.Rotate (v, h, 0);

        //firePoint.transform.rotation = Quaternion.LookRotation(bulletDirection);

        Debug.DrawLine(firePoint2D, firePoint2D + aimDirection, Color.red);


        if (playerController.GetButtonDown("Fire"))
        {
            GameObject rocket = Instantiate(bullet, firePoint2D, Quaternion.identity);
            rocket.GetComponent<Rigidbody2D>().AddForce(speed * bulletDirection, ForceMode2D.Impulse);
            Debug.DrawRay(rocket.transform.position, speed * bulletDirection, Color.green, 10f);
        }
    }

}
