using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainDestruction : MonoBehaviour {
    public TileBase brokenTile;

    public float radius;
    public Tilemap terrain;

    public AudioClip blockDestroy;
    public AudioClip blockCantDestroy;

    public void DestroyTerrain(Vector3 explosionPosition, float radius) {
        bool playSound = true;
        for (int x = -(int)radius; x <= radius; x++) {
            for (int y = -(int)radius; y <= radius; y++) {
                if (!((y == -radius && x == -radius) || (y == -radius && x == radius) || (y == radius && x == -radius) || (y == radius && x == radius))) {
                    Vector3Int tilePos = terrain.WorldToCell(explosionPosition + new Vector3(x, y, 0));
                    if (terrain.GetTile(tilePos) != null) {
                        DestroyTile(tilePos, playSound);
                        playSound = false;
                    }
                }
            }
        }
    }

    void DestroyTile(Vector3Int tilePos, bool playSound = true) {
        if (!terrain.GetTile(tilePos).name.Equals("Metal")) {
            if (playSound) { SoundManager.Instance.PlaySoundEffectDirt(blockDestroy); }
            if (terrain.GetTile(tilePos).name.Equals("Stone")) {
                terrain.SetTile(tilePos, brokenTile);
            } else {
                terrain.SetTile(tilePos, null);
            }
        } else {
            if (playSound) { SoundManager.Instance.PlaySoundEffectDirt(blockCantDestroy); }
        }

    }
}
