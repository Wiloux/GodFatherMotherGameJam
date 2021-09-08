using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public Rewired.Player playerController;
    public float speed;
    private Rigidbody2D rg2D;

    public int force;
    private bool IsJumping;

    private SpriteRenderer SpriteR;

    private int X = 0;

    public bool faceR = true;

    void Start()
    {
        SpriteR = GetComponent<SpriteRenderer>();
        rg2D = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //rg2D.AddForce(Vector2.right * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.UpArrow) && X < 1)
        {
            rg2D.velocity = Vector2.up * force;
            X++;
        }

        if (moveVertical != 0 || X != 0)
        {
            IsJumping = true;
        }
        else
        {
            IsJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IsJumping = false;
        }

        float directionX = 0;

        if (Input.GetButton("Horizontal"))
        {
            directionX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        }




        if (Input.GetAxis("Horizontal") < 0)
        {
            faceR = false;
            SpriteR.flipX = false;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            faceR = true;
            SpriteR.flipX = true;
        }

        transform.Translate(directionX, 0, 0);


        /*float moveHorizontal = playerController.GetAxis("Horizontal");

        float moveVertical = playerController.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rg2D.AddForce(Vector2.right * speed * Time.deltaTime);


        //if (playerController.GetButtonDown("Jump")) { }
        if (playerController.GetButtonDown("Jump")) { }*/
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            X = 0;
        }
    }
}
