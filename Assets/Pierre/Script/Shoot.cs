using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using ToolsBoxEngine;

public class Shoot : MonoBehaviour {
    public Rewired.Player playerController;

    public Transform firePoint;
    public Transform rocketRoot;
    public GameObject bullet;
    public GameObject rocketVisual;
    public GameObject rocketLauncherVisual;

    public float speed = 20f;

    public AudioClip shootFX;

    public Vector2 aimDirection = Vector2.up;
    private Vector2 bulletDirection = Vector2.up;

    public float cooldownDuration;
    private float cooldown;

    void Start() {
        playerController = GetComponent<PlayerController>().playerController;

        aimDirection = new Vector2(0f, 0f);
    }

    void Update() {
        Vector2 axis = Vector2.zero;

        if (playerController.controllers.hasMouse) {
            axis = GameManager.instance.mainCamera.ScreenToWorldPoint(playerController.controllers.Mouse.screenPosition) - transform.position;
        } else {
            axis.x = playerController.GetAxis("AimHorizontal");
            axis.y = playerController.GetAxis("AimVertical");
        }

        if (axis.x != 0f || axis.y != 0f) {
            aimDirection.Set(axis.x, axis.y);
            aimDirection.Normalize();
            AimAt(aimDirection);
        }

        rocketVisual.SetActive(cooldown <= 0);

        if (cooldown <= 0) {
            if (playerController.GetButtonDown("Fire")) {
                Fire(aimDirection);
            }
        } else {
            cooldown -= Time.deltaTime;
        }
    }

    public void AimAt(Vector2 direction) {
        bulletDirection = direction;

        if (GetComponent<PlayerController>().isUpsideDown) {
            direction *= -1;
        }
        rocketRoot.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    public void Fire(Vector2 direction) {
        SoundManager.Instance.PlaySoundEffect(shootFX);
        GameObject rocket = Instantiate(bullet, firePoint.transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        rocket.GetComponent<Missile>().creator = GetComponent<PlayerController>();
        rocket.GetComponent<Rigidbody2D>().AddForce(speed * bulletDirection, ForceMode2D.Impulse);
        rocket.GetComponent<Missile>().direction = direction;
        cooldown = cooldownDuration;
    }
}
