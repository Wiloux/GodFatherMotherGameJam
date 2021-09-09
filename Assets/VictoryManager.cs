using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManager : MonoBehaviour
{

    public Camera mainCam;

    public List<PlayerController> players = new List<PlayerController>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = mainCam.WorldToScreenPoint(players[1].transform.position);
        float ratio = screenPos.y / mainCam.pixelHeight;

    }
}
