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

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //if (collision.gameObject.name.Equals("Tilemap")) {
        if (GameManager.instance.whatIsGround.Contains(collision.gameObject.layer) && collision.GetComponent<PlayerController>() != creator) {
            SoundManager.Instance.PlaySoundEffect(explosionFX);
            GameManager.instance.terrainDestruction.DestroyTerrain(transform.position, radius);
            Explosion lastExplosion = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
            lastExplosion.radius = knockBackRadius;
            if (collision.gameObject.CompareTag("Player")) {
                lastExplosion.ToOblivion(collision.gameObject.GetComponent<PlayerController>());
            }
            Destroy(gameObject);
        }
    }
}
