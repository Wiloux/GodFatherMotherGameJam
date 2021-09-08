using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public Rewired.Player playerController;
    public float speed;
    public float _speedMax;
    public float _accelerationTimer;
    public float _accelerationDuration;
    public AnimationCurve _accelerationCurve;
    private Rigidbody2D rg2D;

    public int force;
    private bool IsJumping;

    private SpriteRenderer SpriteR;

    private int X = 0;

    public bool faceR = true;
    public bool needJump;

    public bool m_Grounded;
    public Transform m_GroundCheck;
    public float k_GroundedRadius;
    public LayerMask m_WhatIsGround;

    public UnityEvent OnLandEvent;

    Vector3 VeloRef = Vector3.zero;

    Vector2 _moveDir;
    private Vector3 _velocity = Vector3.zero;
    private void Awake()
    {
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }
        void Start()
    {
        SpriteR = GetComponent<SpriteRenderer>();
        rg2D = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        float moveHorizontal = playerController.GetAxis("Horizontal") * Time.deltaTime * speed;

        //float moveVertical = playerController.GetAxis("Vertical");

        _moveDir = new Vector2(moveHorizontal,0);



        bool wasGrounded = m_Grounded;
        m_Grounded = false;

 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        if (needJump)
        {
            //isGrounded = false;
            needJump = false;
            rg2D.AddForce(new Vector2(0f, force), ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_GroundCheck.transform.position, k_GroundedRadius);
    }

    void Update()
    {


        if (playerController.GetButtonDown("Jump") && m_Grounded)
        {
            needJump = true;
        }


        //rg2D.AddForce(Vector2.right * speed * Time.deltaTime);

        //if (playerController.GetButtonDown("Jump"))
        //{
        //    rg2D.velocity = Vector2.up * force;
        //    X++;
        //}

        //if (MoveDir.y != 0 || X != 0)
        //{
        //    IsJumping = true;
        //}
        //else
        //{
        //    IsJumping = false;
        //}


        // IsJumping = false;


        // float directionX = 0;

        //if (playerController.GetButton("Horizontal"))
        //{
        //    directionX = playerController.GetAxis("Horizontal") * Time.deltaTime * speed;
        //}
        if (playerController.GetAxis("Horizontal") != 0)
            SpriteR.flipX = playerController.GetAxis("Horizontal") > 0 ? true : false;

        //rg2D.velocity = new Vector2(_moveDir.x, 0);

        //Vector3 targetVelocity = new Vector2(_moveDir.x * 10f, rg2D.velocity.y);
        //// And then smoothing it out and applying it to the character
        //rg2D.velocity = Vector3.SmoothDamp(rg2D.velocity, targetVelocity, ref VeloRef, 1f);

        //if (playerController.GetAxis("Horizontal") < 0)
        //{
        //    faceR = false;
        //    SpriteR.flipX = false;
        //}
        //else if (playerController.GetAxis("Horizontal") > 0)
        //{
        //    faceR = true;
        //    SpriteR.flipX = true;
        //}

        //transform.Translate(directionX, 0, 0);


        /*float moveHorizontal = playerController.GetAxis("Horizontal");

        float moveVertical = playerController.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rg2D.AddForce(Vector2.right * speed * Time.deltaTime);


        //if (playerController.GetButtonDown("Jump")) { }
        if (playerController.GetButtonDown("Jump")) { }*/
        bool isMoving = _moveDir != Vector2.zero;
        if (isMoving)
        {
            if (_velocity != Vector3.zero)
            {
                _StartAcceleration();
            }
        }
        _ApplyAcceleration();
    }

    private void _StartAcceleration()
    {
        float currentSpeed = _velocity.magnitude;
        float accelerationTimerRatio = currentSpeed / _speedMax;
        _accelerationTimer = Mathf.Lerp(0f, _accelerationDuration, accelerationTimerRatio);
    }

    private Vector3 _ApplyAcceleration()
    {
        Vector3 velocity = Vector3.zero;
        _accelerationTimer += Time.deltaTime;
        if (_accelerationTimer < _accelerationDuration)
        {
            //Calculate acceleration according to timer and curve
            float ratio = _accelerationTimer / _accelerationDuration;
            ratio = _accelerationCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(0f, _speedMax, ratio);
            return _moveDir * speed;
        }
        else
        {
            //Speed Max
            return _moveDir * _speedMax;
        }
    }


    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if(collision.transform.tag == "Platform")
    //    isGrounded = true;
    //    else

    //}
}
