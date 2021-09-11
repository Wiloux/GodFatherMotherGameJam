using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public float radius = 2f;
    public float knockBackForce = 20f;
    public float oblivionFactor = 1.3f;

    public AudioClip explosionFX;

    void Start() {
        Boum();
        Destroy(gameObject, 2f);
    }

    private void Boum() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, GameManager.instance.whatGoBoum);

        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.CompareTag("Player")) {
                KnockPlayer(colliders[i].gameObject.GetComponent<PlayerController>());
            }
        }
    }

    private void KnockPlayer(PlayerController player) {
        Vector2 knockBack = player.transform.position - transform.position;
        Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
        float knockBackMag = knockBack.sqrMagnitude;
        float force = Mathf.InverseLerp(radius * radius, 0f, knockBackMag);
        //Debug.DrawLine(transform.position, (Vector3)knockbackDir * knockBackMag + transform.position, Color.yellow);
        //player.KnockBack(knockbackDir.normalized);
        player.AddExternalForce(knockbackDir * force * knockBackForce);
        //Debug.Log(knockbackDir * force * knockBackForce + " .. " + (knockbackDir * force * knockBackForce).magnitude);
    }

    public void ToOblivion(PlayerController player) {
        Vector2 knockBack = player.transform.position - transform.position;
        Vector2 knockbackDir = new Vector2(Mathf.Sign(knockBack.x), 0.2f * Mathf.Sign(knockBack.y)).normalized;
        player.AddExternalForce(knockbackDir * knockBackForce * oblivionFactor);
        Debug.Log("OBLIVION");
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
