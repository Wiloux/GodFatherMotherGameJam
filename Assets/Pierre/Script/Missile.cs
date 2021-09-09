using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class Missile : MonoBehaviour {
    [SerializeField] private float radius;
    public GameObject explosion;


    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //if (collision.gameObject.name.Equals("Tilemap")) {
        if (GameManager.instance.whatIsGround.Contains(collision.gameObject.layer)) {
            GameManager.instance.terrainDestruction.DestroyTerrain(transform.position, radius);
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
