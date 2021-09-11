using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class Missile : MonoBehaviour {
    [SerializeField] private float radius = 2f;
    [SerializeField] private float knockBackRadius = 4f;
    public GameObject explosion;
    public PlayerController creator;
    public AudioClip explosionFX;

    public GameObject expVFX;

    [HideInInspector] public Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //if (collision.gameObject.name.Equals("Tilemap")) {
        if ((GameManager.instance.whatIsGround.Contains(collision.gameObject.layer) ||
            GameManager.instance.whatGoBoum.Contains(collision.gameObject.layer)) &&
            collision.GetComponent<PlayerController>() != creator
        ) {

            GameObject explosion = Instantiate(expVFX, transform.position, Quaternion.identity);
            Explosion lastExplosion = explosion.GetComponent<Explosion>();
            lastExplosion.radius = knockBackRadius;
            if (collision.gameObject.CompareTag("Player")) {
                lastExplosion.ToOblivion(collision.gameObject.GetComponent<PlayerController>());
            }

            GameManager.instance.terrainDestruction.DestroyTerrain(transform.position, radius);

            SoundManager.Instance.PlaySoundEffect(explosionFX);
            GameManager.instance.mainCamera.GetComponent<CameraController>().Shake(direction * 0.1f, 0.1f);
            Destroy(explosion, 2f);

            Destroy(gameObject);
        }
    }
}
