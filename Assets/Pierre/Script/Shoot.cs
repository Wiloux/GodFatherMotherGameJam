using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using ToolsBoxEngine;

public class Shoot : MonoBehaviour {
    public Rewired.Player playerController;

    public Transform firePoint;
    public GameObject bullet;
    public GameObject rocketVisual;

    public float speed = 20f;

    private Vector2 aimDirection = Vector2.up;
    private Vector2 bulletDirection = Vector2.up;

    public float cooldownDuration;
    private float cooldown;

    void Start() {
        playerController = GetComponent<PlayerController>().playerController;

        aimDirection = new Vector2(0f, 0f);
    }

    void Update()
    {
        Vector2 axis = Vector2.zero;

        if (playerController.controllers.hasMouse)
        {
            axis = GameManager.instance.mainCamera.ScreenToWorldPoint(playerController.controllers.Mouse.screenPosition) - transform.position;
        }
        else
        {
            axis.x = playerController.GetAxis("AimHorizontal");
            axis.y = playerController.GetAxis("AimVertical");
        }

        if (axis.x != 0f || axis.y != 0f)
        {
            aimDirection.Set(axis.x, axis.y);
            aimDirection.Normalize();

            firePoint.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, aimDirection));

            bulletDirection = firePoint.transform.rotation * Vector2.up;
            bulletDirection.Normalize();
        }

        rocketVisual.SetActive(cooldown <= 0);

        if (cooldown <= 0)
        {

            if (playerController.GetButtonDown("Fire"))
            {
                GameObject rocket = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
                rocket.GetComponent<Missile>().creator = GetComponent<PlayerController>();
                rocket.GetComponent<Rigidbody2D>().AddForce(speed * bulletDirection, ForceMode2D.Impulse);
                //Debug.DrawRay(rocket.transform.position, speed * bulletDirection, Color.green, 10f);
                cooldown = cooldownDuration;
            }
        }
        else
        {

            cooldown -= Time.deltaTime;
        }
    }
}
