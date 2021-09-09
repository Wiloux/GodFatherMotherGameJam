using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class askTurnAround : MonoBehaviour
{
    public PlayerController p;


    public void askForATurnAround()
    {
        p.TurnAround();
    }
}
