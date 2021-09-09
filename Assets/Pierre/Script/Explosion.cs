using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    bool firstFrame = true;
    void Start()
    {
        Destroy(gameObject, 2f);
        StartCoroutine(DisableCollision());
    }

    private void Update()
    {
    }

    IEnumerator DisableCollision()
    {
       yield return new WaitForSecondsRealtime(0.1f);
        firstFrame = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (firstFrame)
        {
            if (collision.GetComponent<PlayerController>())
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                Vector2 knockbackDir = player.transform.position - transform.position;
                Debug.DrawLine(transform.position, (Vector3)knockbackDir + transform.position, Color.yellow);
                player.KnockBack(knockbackDir.normalized);
            }
        }
    }

}
