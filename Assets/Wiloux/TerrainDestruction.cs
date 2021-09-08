using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainDestruction : MonoBehaviour
{

    public TileBase brokenTile;

    public float radius;
    public Tilemap terrain;
    public GameObject spr;
    // Start is called before the first frame update
    void Start()
    {


    }


    void DestroyTerrain(Vector3 explosionPosition, float radius)
    {


        for (int x = -(int)radius; x <= radius; x++)
        {
            for (int y = -(int)radius; y <= radius; y++)
            {

                if ((y == -radius && x == -radius) || (y == -radius && x == radius ) || (y == radius && x == -radius) || (y == radius && x == radius))
                {

                }
                else { 
                    //if (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) < Mathf.Pow(radius, 2))
                    //{
                    Vector3Int tilePos = terrain.WorldToCell(explosionPosition + new Vector3(x, y, 0));
                    if (terrain.GetTile(tilePos) != null)
                    {
                        DestroyTile(tilePos);
                    }
                    //}
                }
            }
        }
    }

    void DestroyTile(Vector3Int tilePos)
    {

        if (terrain.GetTile(tilePos).name != "UndestructibleBlock")
        {
            if (terrain.GetTile(tilePos).name == "StrongBlock")
            {

                terrain.SetTile(tilePos, brokenTile);
            }
            else
            {
                terrain.SetTile(tilePos, null);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spawnPos.z = 0;
            spr.transform.position = spawnPos;
            DestroyTerrain(spawnPos, radius);
        }
    }
}
