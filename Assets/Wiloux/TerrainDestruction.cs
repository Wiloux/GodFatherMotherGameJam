using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainDestruction : MonoBehaviour
{

    public TileBase brokenTile;

    public float radius;
    public Tilemap terrain;

    public AudioClip blockDestroy;
    public AudioClip blockCantDestroy;

    public void DestroyTerrain(Vector3 explosionPosition, float radius)
    {
        for (int x = -(int)radius; x <= radius; x++)
        {
            for (int y = -(int)radius; y <= radius; y++)
            {

                if ((y == -radius && x == -radius) || (y == -radius && x == radius) || (y == radius && x == -radius) || (y == radius && x == radius))
                {

                }
                else
                {
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
        if (terrain.GetTile(tilePos).name != "Metal")
        {
            SoundManager.Instance.PlaySoundEffectDirt(blockDestroy);
            if (terrain.GetTile(tilePos).name == "Stone")
            {

                terrain.SetTile(tilePos, brokenTile);
            }
            else
            {
                terrain.SetTile(tilePos, null);
            }
        }
        else
        {
            SoundManager.Instance.PlaySoundEffectDirt(blockCantDestroy);
        }

    }
}
