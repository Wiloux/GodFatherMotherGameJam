using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class Missile : MonoBehaviour {
    [SerializeField] private float radius;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Equals("Tilemap")) {
            GameManager.instance.terrainDestruction.DestroyTerrain(transform.position, radius);
            Destroy(gameObject);
        }
    }
}
