using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class Missile : MonoBehaviour {
    [SerializeField] private float radius;
    public GameObject explosion;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Tilemap")) {
            GameManager.instance.terrainDestruction.DestroyTerrain(transform.position, radius);
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
