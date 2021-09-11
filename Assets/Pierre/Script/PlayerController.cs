using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;
using ToolsBoxEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
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
    public float groundedTime = 0.1f;
    public float groundedTimer = 0f;

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

    public List<AudioClip> jump = new List<AudioClip>();

    [Header("VFX")]
    public ParticleSystem jumpVFX;
    public ParticleSystem activatePowerFx;
    public ParticleSystem readyPowerFx;
    public ParticleSystem psRun;
    public GameObject deathVFX;
    public ParticleSystem fxFlyingTrail;
    bool gaveup = true;

    [Header("SFX")]
    public AudioClip winP1;
    public AudioClip winP2;
    public AudioClip deathSFX;
    public AudioClip sfxGravityChange;
    public AudioClip sfxGravityLoaded;

    [HideInInspector] public int playerID;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        if (playerID != 1) {
            characterAnimator = transform.Find("CharacterRoot/character1").GetComponent<Animator>();
        } else {
            characterAnimator = transform.Find("CharacterRoot/character2").GetComponent<Animator>();

        }
    }

    private void FixedUpdate() {
        UpdateGravity();
        UpdateMove();
        UpdateExternalForces();
        ApplySpeed();
    }

    void Update() {
        if (GameManager.instance.ended) {
            if (!GameManager.instance.canRestart) { return; }
            if (playerController.GetButtonDown("Leave Game")) {
                GameManager.instance.Menu();
            } else if (playerController.GetAnyButtonDown()) {
                GameManager.instance.Restart();
            }
            return;
        }

        if (grounded && velocity.x != 0 && !psRun.isPlaying)
            psRun.Play();
        else if (psRun.isPlaying)
            psRun.Stop();


        characterAnimator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (playerController.GetButtonDown("Jump") && (grounded || groundedTimer > 0f)) {
            jumpVFX.Play();
            jumping = true;
            SoundManager.Instance.PlaySoundEffectList(jump);
        }

        if (playerController.GetAxis("Horizontal") != 0 && !isTurningAround)
            body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);
        //else if (isTurningAround)
        //    body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * -playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);

        float moveHorizontal = playerController.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(moveHorizontal, 0f);

        if (gravityCooldown <= 0f && !gaveup) {
            gaveup = true;
            readyPowerFx.Play();
            SoundManager.Instance.PlaySoundEffect(sfxGravityLoaded, 0.5f);
        }

        if (gravityCooldown <= 0f) {
            if (playerController.GetButtonDown("Gravity")) {
                SoundManager.Instance.PlaySoundEffect(sfxGravityChange);
                gaveup = false;
                activatePowerFx.Play();
                isUpsideDown = !isUpsideDown;
                gravity *= -1;
                float yScale = transform.localScale.y * -1;
                transform.localScale = new Vector3(0.5f, yScale, 1f);
                gravityCooldown = gravityCooldownDuration;

                Shoot shoot = GetComponent<Shoot>();
                shoot.AimAt(shoot.aimDirection);
            }

        } else {
            gravityCooldown -= Time.deltaTime;
        }
    }

    public void TurnAround() {
        if (playerController.GetAxis("Horizontal") != 0)
            //body.flipX = playerController.GetAxis("Horizontal") > 0 ? true : false;
            body.localScale = body.localScale.Override(Mathf.Abs(body.localScale.x) * playerController.GetAxis("Horizontal") > 0 ? 1 : -1, Axis.X);
    }

    private void OnDrawGizmosSelected() {
        //Gizmos.DrawWireSphere(groundCheck.transform.position, groundedRadius);
        Gizmos.DrawWireCube(groundCheck.transform.position, groundCheck.localScale);
    }

    private void UpdateGravity() {
        if (velocity.y * gravity > 0f) { velocity += Vector2.down * gravity; return; }
        //if (externalForces.y > 0f) { velocity += Vector2.down * gravity; return; }

        grounded = false;

        //Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, GameManager.instance.whatIsGround);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheck.localScale, 0f, GameManager.instance.whatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                grounded = true;
            }
        }

        if (!grounded) {
            velocity += Vector2.down * gravity;
            if (groundedTimer > 0f) { groundedTimer -= Time.fixedDeltaTime; }
        } else {
            if (!wasGrounded) {
                characterAnimator.SetTrigger("GroundContact");
                velocity.y = 0f;
                groundedTimer = groundedTime;
            }
        }

        if (jumping) {
            characterAnimator.ResetTrigger("GroundContact");
            characterAnimator.SetTrigger("Jump");
            velocity.y = 0f;
            if (!isUpsideDown)
                velocity.y += jumpForce;
            else
                velocity.y -= jumpForce;
        }

        jumping = false;
    }

    private void UpdateMove() {
        bool isMoving = moveDirection != Vector2.zero;

        if (isMoving) {
            if (velocity != Vector2.zero) {

                if (Vector3.Dot(previousMoveDirection, moveDirection) < 0f) {
                    StartTurnAround();
                }
            } else {
                StartAcceleration();
            }

            if (isTurningAround) {
                velocity = ApplyTurnAround().Override(velocity.y, Axis.Y);
            } else {
                velocity = ApplyAcceleration().Override(velocity.y, Axis.Y);
            }

            previousMoveDirection = moveDirection;
        } else if (grounded) {
            if (wasMoving) {
                StartFrictions();
            }
            velocity = ApplyFrictions().Override(velocity.y, Axis.Y);
        }

        wasMoving = isMoving;
    }

    private void UpdateExternalForces() {
        if (grounded && !wasGrounded) {
            externalForces.y = 0f;
        }

        if (externalForces.y * velocity.y < 0) {
            externalForces.y += velocity.y;
            velocity.y = 0f;
        }

        if (externalForces.x > speedMax) {
            externalForces.x = ApplyFrictions(externalForces).x;
        }

        if (grounded) {
            externalForces.x = ApplyFrictions(externalForces).x;
        }

        if (fxFlyingTrail.isPlaying && externalForces == Vector2.zero) {
            fxFlyingTrail.Stop();
        } else if (!fxFlyingTrail.isPlaying && externalForces != Vector2.zero) {
            fxFlyingTrail.Play();
        }
    }

    private void StartAcceleration() {
        float currentSpeed = Mathf.Abs(velocity.x);
        float accelerationTimerRatio = currentSpeed / speedMax;
        accelerationTimer = Mathf.Lerp(0f, accelerationDuration, accelerationTimerRatio);
    }

    private Vector2 ApplyAcceleration() {
        Vector2 velocity = Vector2.zero;
        accelerationTimer += Time.deltaTime;
        if (accelerationTimer < accelerationDuration) {
            float ratio = accelerationTimer / accelerationDuration;
            ratio = accelerationCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(0f, speedMax, ratio);
            return moveDirection * speed;
        } else {
            return moveDirection * speedMax;
        }
    }

    private void StartFrictions() {
        float currentSpeed = Mathf.Abs(velocity.x);
        float frictionTimerRatio = Mathf.InverseLerp(0f, speedMax, currentSpeed);
        frictionsTimer = Mathf.Lerp(frictionsDuration, 0f, frictionTimerRatio);
    }

    private Vector2 ApplyFrictions() {
        frictionsTimer += Time.deltaTime;
        if (frictionsTimer < frictionsDuration) {
            //Calculate Frictions according to timer and curve
            float ratio = frictionsTimer / frictionsDuration;
            ratio = frictonsCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(speedMax, 0f, ratio);
            return new Vector2(velocity.x / speedMax * speed, 0f);
        } else {
            //Reset speed
            previousMoveDirection = Vector2.zero;
            return Vector2.zero;
        }
    }

    private Vector2 ApplyFrictions(Vector2 velocity) {
        float currentSpeed = Mathf.Abs(velocity.x);
        float frictionTimerRatio = Mathf.Clamp01(Mathf.InverseLerp(0f, speedMax, currentSpeed));
        float frictionsTimer = Mathf.Lerp(frictionsDuration, 0f, frictionTimerRatio);
        frictionsTimer += Time.deltaTime;
        if (frictionsTimer < frictionsDuration) {
            float ratio = frictionsTimer / frictionsDuration;
            ratio = frictonsCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(speedMax, 0f, ratio);
            return new Vector2(currentSpeed / speedMax * speed * Mathf.Sign(velocity.x), 0f);
        } else {
            return Vector2.zero;
        }
    }

    //public void KnockBack(Vector2 direction) {
    //    Debug.Log(direction);
    //    velocity += direction * knockbackForce;
    //}

    private void StartTurnAround() {
        isTurningAround = true;
        turnAroundTimer = 0f;
        turnAroundDuration = Mathf.Lerp(0f, turnAroundMaxDuration, Mathf.Abs(velocity.x) / speedMax);
        turnAroundStartSpeed = velocity.x;
        characterAnimator.SetBool("TurnAround", true);
    }

    private Vector2 ApplyTurnAround() {
        turnAroundTimer += Time.deltaTime;
        if (turnAroundTimer < turnAroundDuration) {
            float ratio = turnAroundTimer / turnAroundDuration;
            ratio = turnAroundCurve.Evaluate(ratio);
            float speed = Mathf.Lerp(turnAroundStartSpeed, 0f, ratio);
            return velocity.normalized * Mathf.Abs(speed);
        } else {
            characterAnimator.SetBool("TurnAround", false);
            accelerationTimer = 0f;
            isTurningAround = false;
            return Vector3.zero;
        }
    }

    private void ApplySpeed() {
        rb2D.velocity = (velocity + externalForces);

        wasGrounded = grounded;
    }

    public void AddExternalForce(Vector2 force) {
        externalForces += force;
        velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("death")) {
            if (GameManager.instance.ended) { return; }
            SoundManager.Instance.PlaySoundEffect(deathSFX);
            GameObject o = Instantiate(deathVFX, transform.position, Quaternion.LookRotation(Vector3.forward, velocity + externalForces));
            // Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.down, velocity)))
            Destroy(o, 3f);

            int deltaDir = 0;
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            if (direction.x < 0 || direction.y < 0) {
                deltaDir += 1;
            }
            int id = playerID;

            deltaDir += (1 - id) * 2;
            GameManager.instance.StartEndScreen(deltaDir);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (externalForces != Vector2.zero && GameManager.instance.whatIsGround.Contains(collision.gameObject.layer)) {
            Vector2 position = (Vector2)transform.position;
            Vector2 barycenter = Vector2.zero;
            for (int i = 0; i < collision.contactCount; i++) {
                barycenter += collision.GetContact(i).point;
            }
            Vector2 contactPos = barycenter/ collision.contactCount;
            Vector2 contactDelta = contactPos - position;
            contactDelta.Normalize();
            if (Mathf.Abs(contactDelta.x) > Mathf.Abs(contactDelta.y)) {
                externalForces.x = 0f;
            } else {
                externalForces.y = 0f;
            }
        }
    }
}
