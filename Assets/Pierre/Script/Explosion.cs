using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    bool firstFrame = true;
    void Start()
    {
        firstFrame = false;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.GetComponent<PlayerController>())
            {
                Debug.Log("hi");
                PlayerController player = collision.GetComponent<PlayerController>();
                Vector2 knockbackDir = player.transform.position - transform.position;
                player.KnockBack(knockbackDir.normalized);
            }
    }

}
