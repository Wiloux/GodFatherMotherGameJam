using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;
using ToolsBoxEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rewired.Player playerController;

    [Header("Movements")]
    //public float speed;
    public float speedMax;
    public Vector2 externalForces = Vector2.zero;
    private bool wasMoving = false;

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

    [Header("Gravity")]
    public float gravityCooldownDuration;
    private float gravityCooldown;
    public bool isUpsideDown;

    [Header("Sprite")]
    [SerializeField] private Transform body;
    public GameObject sprite;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Animator rocketAnimator;

    private bool faceR = true;
    private Rigidbody2D rb2D;
    private Animation anim;

    public GameObject menuFin;
    private bool death = false;

    public List<AudioClip> jump = new List<AudioClip>();

    public ParticleSystem jumpVFX;

    public int playerID;
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (playerID != 1)
        {
            characterAnimator = transform.Find("CharacterRoot/character1").GetComponent<Animator>();
        }
        else
        {
            characterAnimator = transform.Find("CharacterRoot/character2").GetComponent<Animator>();

        }

        menuFin.SetActive(false);
    }

    private void FixedUpdate()
    {
        UpdateGravity();
        UpdateMove();
        UpdateExternalForces();
        ApplySpeed();
    }

    void Update()
    {

        if (grounded && velocity.x != 0 && !psRun.isPlaying)
            psRun.Play();
        else if (psRun.isPlaying)
            psRun.Stop();


        characterAnimator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (playerController.GetButtonDown("Jump") && grounded)
        {
            jumpVFX.Play();
            jumping = true;
            SoundManager.Instance.PlaySoundEffectList(jump);
        }

        if (playerController.GetAxis("Horizontal") != 0)
            //    //body.flipX = playerController.GetAxis("Horizontal") > 0 ? true : false;
            body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);

        float moveHorizontal = playerController.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(moveHorizontal, 0f);

        if (gravityCooldown <= 0f)
        {

            if (playerController.GetButtonDown("Gravity"))
            {

                GetComponent<Shoot>().aimDirection *= -1;
                GetComponent<Shoot>().UpdateAim();
                isUpsideDown = !isUpsideDown;
                gravity *= -1;
                float yScale = transform.localScale.y * -1;
                transform.localScale = new Vector3(0.5f, yScale, 1f);
                gravityCooldown = gravityCooldownDuration;
            }


        }
        else
        {

            gravityCooldown -= Time.deltaTime;
        }

        if (death)
        {
            if (playerController.GetButtonDown("Jump"))
                BouttonJouer();

            if (playerController.GetButtonDown("Gravity"))
                BouttonMenu();
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
                if (!isUpsideDown)
                    velocity.y += jumpForce;
                else
                    velocity.y -= jumpForce;
            }
        }

        jumping = false;
    }


    public ParticleSystem psRun;

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

    private void UpdateExternalForces()
    {
        if (grounded && !wasGrounded)
        {
            externalForces.y = 0f;
        }

        if (externalForces.y * velocity.y < 0)
        {
            externalForces.y += velocity.y;
            velocity.y = 0f;
        }

        //if (externalForces.x * velocity.x < 0) {
        //    if (Mathf.Abs(externalForces.x + velocity.x) > 0) {
        //        externalForces.x += velocity.x;
        //    } else {
        //        externalForces.x = 0f;
        //    }
        //}

        if (externalForces.x > speedMax)
        {
            externalForces.x = ApplyFrictions(externalForces).x;
        }

        if (grounded)
        {
            externalForces.x = ApplyFrictions(externalForces).x;
        }
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

    private Vector2 ApplyFrictions(Vector2 velocity)
    {
        float currentSpeed = Mathf.Abs(velocity.x);
        float frictionTimerRatio = Mathf.Clamp01(Mathf.InverseLerp(0f, speedMax, currentSpeed));
        float frictionsTimer = Mathf.Lerp(frictionsDuration, 0f, frictionTimerRatio);
        frictionsTimer += Time.deltaTime;
        if (frictionsTimer < frictionsDuration)
        {
            float ratio = frictionsTimer / frictionsDuration;
            ratio = frictonsCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(speedMax, 0f, ratio);
            return new Vector2(currentSpeed / speedMax * speed * Mathf.Sign(velocity.x), 0f);
        }
        else
        {
            return Vector2.zero;
        }
    }

    //public void KnockBack(Vector2 direction) {
    //    Debug.Log(direction);
    //    velocity += direction * knockbackForce;
    //}

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
        rb2D.velocity = velocity + externalForces;

        wasGrounded = grounded;
    }

    public void AddExternalForce(Vector2 force)
    {
        externalForces += force;
        velocity = Vector2.zero;
    }


    public AudioClip winP1;
    public AudioClip winP2;

    public GameObject deathVFX;
    public AudioClip deathSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("death"))
        {
            SoundManager.Instance.PlaySoundEffect(deathSFX);
            GameObject o = Instantiate(deathVFX, transform.position, Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.down, velocity)));
            Destroy(o, 3f);

            int deltaDir = 0;
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            if (direction.x < 0 || direction.y < 0)
            {
                deltaDir += 1;
            }
            int id = playerID;
            if (id == 0)
            {
                SoundManager.Instance.PlaySoundEffect(winP2);

            }
            else
            {
                SoundManager.Instance.PlaySoundEffect(winP1);

            }
            deltaDir += (1 - id) * 2;
            Debug.Log(deltaDir);
            GameManager.instance.StartEndScreen(deltaDir);
        }
    }

    public void BouttonJouer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SceneFinal");
    }

    public void BouttonMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
