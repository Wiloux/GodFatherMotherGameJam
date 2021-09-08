using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Shoot : MonoBehaviour {
    public Rewired.Player playerController;

    public Transform firePoint;
    public GameObject bullet;

    public float speed = 20f;

    public Vector2 aimDirection;
    private Vector2 bulletDirection;

    void Start() {
        playerController = GetComponent<PlayerController>().playerController;

        aimDirection = new Vector2(0f, 0f);
    }

    void Update() {
        float horizontal = playerController.GetAxis("AimHorizontal");
        float vertical = playerController.GetAxis("AimVertical");

        if (vertical != 0f || horizontal != 0f) {
            aimDirection.Set(horizontal, vertical);
            aimDirection.Normalize();

            firePoint.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, aimDirection));

            bulletDirection = firePoint.transform.rotation * Vector2.up;
            bulletDirection.Normalize();
        }

        if (playerController.GetButtonDown("Fire")) {
            GameObject rocket = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
            rocket.GetComponent<Rigidbody2D>().AddForce(speed * bulletDirection, ForceMode2D.Impulse);
            //Debug.DrawRay(rocket.transform.position, speed * bulletDirection, Color.green, 10f);
        }
    }

}
