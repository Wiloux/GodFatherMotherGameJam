using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    private Rigidbody2D rg2D;

    public int force;
    private bool IsJumping;

    private SpriteRenderer SpriteR;

    private int X = 0;
    private int Y = 0;

    public bool faceR = true; 

    void Start()
    {
        SpriteR = GetComponent<SpriteRenderer>();
        rg2D = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        
    }
}
