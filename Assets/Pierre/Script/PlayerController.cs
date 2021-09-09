using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;
using ToolsBoxEngine;

public class PlayerController : MonoBehaviour
{
    public Rewired.Player playerController;

    [Header("Movements")]
    //public float speed;
    public float speedMax;

    [Header("Acceleration")]
    [SerializeField] private float accelerationDuration;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float gravity = 5f;
    private float accelerationTimer;

    [Header("Frictions")]
    [SerializeField] private float frictionsDuration = 1f;
    [SerializeField] private AnimationCurve frictonsCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    private float frictionsTimer = 0f;

    [Header("Turn Around")]
    [SerializeField] private float turnAroundMaxDuration = 1f;
    [SerializeField] private AnimationCurve turnAroundCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    private bool isTurningAround = false;
    private float turnAroundTimer = 0f;
    private float turnAroundStartSpeed = 0f;
    private float turnAroundDuration = 0f;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 previousMoveDirection = Vector2.zero;
    private Vector2 velocity;
    public bool grounded = false;
    private bool wasGrounded = false;

    public float knockbackForce;

    [Header("Air")]
    public int jumpForce;
    private bool jumping;
    public Transform groundCheck;
    public float groundedRadius;

    private bool wasMoving = false;

    [Header("Sprite")]
    [SerializeField] private Transform body;
    public GameObject sprite;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Animator rocketAnimator;

    private bool faceR = true;
    private Rigidbody2D rb2D;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        UpdateMove();
        ApplySpeed();
    }

    void Update()
    {
        characterAnimator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (playerController.GetButtonDown("Jump") && grounded)
        {
            jumping = true;
        }


        if (playerController.GetAxis("Horizontal") != 0)
            //    //body.flipX = playerController.GetAxis("Horizontal") > 0 ? true : false;
            body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);



        float moveHorizontal = playerController.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(moveHorizontal, 0f);

        if (playerController.GetButtonDown("Gravity"))
        {
            if (gravity < 0)
            {
                gravity = gravity * (-1);
                sprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                gravity = gravity * (-1);
                sprite.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }

    }


    public void TurnAround()
    {
        if (playerController.GetAxis("Horizontal") != 0)
            //body.flipX = playerController.GetAxis("Horizontal") > 0 ? true : false;
            body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(groundCheck.transform.position, groundedRadius);
        Gizmos.DrawWireCube(groundCheck.transform.position, groundCheck.localScale);
    }

    private void UpdateGravity()
    {
        grounded = false;

        //Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, GameManager.instance.whatIsGround);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheck.localScale, 0f, GameManager.instance.whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
            }
        }

        if (!grounded)
        {
            velocity += Vector2.down * gravity;
        }
        else
        {
            if (!wasGrounded)
            {
                characterAnimator.SetTrigger("GroundContact");
                velocity.y = 0f;
            }

            if (jumping)
            {
                characterAnimator.ResetTrigger("GroundContact");
                characterAnimator.SetTrigger("Jump");
                velocity.y += jumpForce;
            }
        }

        wasGrounded = grounded;
        jumping = false;
    }

    private void UpdateMove()
    {
        bool isMoving = moveDirection != Vector2.zero;

        if (isMoving)
        {
            if (velocity != Vector2.zero)
            {
                if (Vector3.Dot(previousMoveDirection, moveDirection) < 0f)
                {
                    StartTurnAround();
                    characterAnimator.SetBool("TurnAround", true);
                }
            }
            else
            {
                StartAcceleration();
            }

            if (isTurningAround)
            {
                velocity = ApplyTurnAround().Override(velocity.y, Axis.Y);
            }
            else
            {
                velocity = ApplyAcceleration().Override(velocity.y, Axis.Y);
            }

            previousMoveDirection = moveDirection;
        }
        else if (grounded)
        {
            if (wasMoving)
            {
                StartFrictions();
            }
            velocity = ApplyFrictions().Override(velocity.y, Axis.Y);
        }

        wasMoving = isMoving;
    }

    private void StartAcceleration()
    {
        float currentSpeed = Mathf.Abs(velocity.x);
        float accelerationTimerRatio = currentSpeed / speedMax;
        accelerationTimer = Mathf.Lerp(0f, accelerationDuration, accelerationTimerRatio);
    }

    private Vector2 ApplyAcceleration()
    {
        Vector2 velocity = Vector2.zero;
        accelerationTimer += Time.deltaTime;
        if (accelerationTimer < accelerationDuration)
        {
            float ratio = accelerationTimer / accelerationDuration;
            ratio = accelerationCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(0f, speedMax, ratio);
            return moveDirection * speed;
        }
        else
        {
            return moveDirection * speedMax;
        }
    }

    private void StartFrictions()
    {
        float currentSpeed = Mathf.Abs(velocity.x);
        float frictionTimerRatio = Mathf.InverseLerp(0f, speedMax, currentSpeed);
        frictionsTimer = Mathf.Lerp(frictionsDuration, 0f, frictionTimerRatio);
    }

    private Vector2 ApplyFrictions()
    {
        frictionsTimer += Time.deltaTime;
        if (frictionsTimer < frictionsDuration)
        {
            //Calculate Frictions according to timer and curve
            float ratio = frictionsTimer / frictionsDuration;
            ratio = frictonsCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(speedMax, 0f, ratio);
            return new Vector2(velocity.x / speedMax * speed, 0f);
        }
        else
        {
            //Reset speed
            previousMoveDirection = Vector2.zero;
            return Vector2.zero;
        }
    }

    public void KnockBack(Vector2 direction)
    {
        Debug.Log(direction);
        velocity += direction * knockbackForce;
    }

    private void StartTurnAround()
    {
        isTurningAround = true;
        turnAroundTimer = 0f;
        turnAroundDuration = Mathf.Lerp(0f, turnAroundMaxDuration, Mathf.Abs(velocity.x) / speedMax);
        turnAroundStartSpeed = velocity.x;
    }

    private Vector2 ApplyTurnAround()
    {
        turnAroundTimer += Time.deltaTime;
        if (turnAroundTimer < turnAroundDuration)
        {
            float ratio = turnAroundTimer / turnAroundDuration;
            ratio = turnAroundCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(turnAroundStartSpeed, 0f, ratio);
            return velocity.normalized * Mathf.Abs(speed);
        }
        else
        {
            characterAnimator.SetBool("TurnAround", false);
            accelerationTimer = 0f;
            isTurningAround = false;
            return Vector3.zero;
        }
    }

    private void ApplySpeed()
    {
        rb2D.velocity = velocity;
    }
}
