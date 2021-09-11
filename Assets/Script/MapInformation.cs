using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInformation : MonoBehaviour {
    [Header("Map")]
    public Transform blueSpawn;
    public Transform redSpawn;

    [Header("Scrolling")]
    public bool cameraMove = false;
    public Transform start = null;
    public Transform end = null;

    [Header("Background")]
    public Color backgroundTint;
}
