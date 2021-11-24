using System.Collections;
using UnityEngine;

public class SpidyMovement : MonoBehaviour
{
    //Paramètres
    public float moveSpeed;
    public float smoothTimeGround;
    public float smoothTimeAir;
    public float jumpForce;
    public float stickyForce;
    public float maxSpeed;
    public float webShootSpeed;
    public float webDashSpeed;
    public float webDashCutOffLenght;
    public float webDashingLifeSpan;
    public float glidingSpeed;
    public float maxGliderAngle;
    public float hackedSpeedCoefficient;
    public float flashingPeriod;
    public float noHitPeriod;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject webShootPreFab;
    public Camera cam;
    public GameObject webGlider;
    public AbilityDisplay abilityDisplay;
    public LineRenderer lineRenderer;

    //GroundCheck, WallCheck,CeilingCheck
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundCheckLayers;


    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public Transform wallCheckUp;
    public float wallCheckRadius;

    //Variable interne
    private int horizontalInput;
    private float horizontalMovement;
    private int verticalInput;
    private float verticalMovement;

    private Vector3 velocity;
    private float playerAngle;

    private GameObject webShootGO;

    private int damageTaken;

    private KeyCode keyCodeRight;
    private KeyCode keyCodeLeft;
    private KeyCode keyCodeUp;
    private KeyCode keyCodeDown;

    //Bool
    private bool isGrounded;
    private bool isJumping;
    private bool isWalledRight;
    private bool isWalledLeft;
    private bool isWalledUp;
    private bool isWebShooting;
    private bool canShoot;
    private bool isGliding;
    private bool canBeHit;

    private bool isHackedGlide;
    private bool isHackedWeb;
    private bool isDrawing;

    [HideInInspector]
    public bool isWebDashing;
    [HideInInspector]
    public bool isPaused;

    /*
    [HideInInspector]
    public bool istrulyGrounded;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isJumping;
    [HideInInspector]
    public bool isFliped;
    [HideInInspector]
    public bool isGamePaused;
    */

    public static SpidyMovement instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de SpidyMovement dans la scène");
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        horizontalInput = 0;
        horizontalMovement = 0;
        velocity = Vector3.zero;
        playerAngle = 0;
        damageTaken = 0;

        isGrounded = false;
        isJumping = false;
        isWalledRight = false;
        isWalledLeft = false;
        isWalledUp = false;
        isWebShooting = false;
        canShoot = true;
        isGliding = false;
        //isHackedSpeed = false;
        isHackedGlide = false;
        isHackedWeb = false;

        isWebDashing = false;
        isDrawing = false;
        canBeHit = true;
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            //Condition saut
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);

            //Condition Wall
            isWalledRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundCheckLayers);
            isWalledLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundCheckLayers);
            isWalledUp = Physics2D.OverlapCircle(wallCheckUp.position, wallCheckRadius, groundCheckLayers);

            //Angle du joueur
            playerAngle = transform.eulerAngles.z;
            if (playerAngle >= 315 || playerAngle <= 45)
            {
                playerAngle = 0;
            }
            else if (playerAngle >= 45 && playerAngle <= 135)
            {
                playerAngle = 90;
            }
            else if (playerAngle >= 225 && playerAngle <= 315)
            {
                playerAngle = 270;
            }
            else
            {
                playerAngle = 180;
            }

            //Mouvements

            horizontalMovement = horizontalInput * moveSpeed * Time.fixedDeltaTime;
            verticalMovement = verticalInput * moveSpeed * Time.fixedDeltaTime;
            MovePlayer(horizontalMovement, verticalMovement);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }

            //Rotate
            if (isGrounded)
            {
                rb.gravityScale = 0;
                isGliding = false;
            }
            else
            {
                rb.gravityScale = 1;

                if (isWalledRight)
                {
                    RotatePlayer(90f);
                }
                else if (isWalledLeft)
                {
                    RotatePlayer(-90f);
                }
                else if (isWalledUp)
                {
                    RotatePlayer(180f);
                }
                else
                {
                    RotatePlayer(-playerAngle);
                }
            }

            //Saut
            if (isJumping /*&& rb.velocity.y < 0.1f*/)
            {
                JumpPlayer(playerAngle);
            }

            //Glide
            if (isGliding)
            {
                if (rb.velocity.y < -glidingSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -glidingSpeed);
                }
                else if (rb.velocity.y > glidingSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, glidingSpeed);
                }
            }



            //WebShoot
            if (isWebShooting)
            {
                isWebShooting = false;
                WebShoot();
            }

            if (isWebDashing)
            {
                Vector2 _targetVec = webShootGO.transform.position - transform.position;
                rb.velocity = _targetVec.normalized * webDashSpeed;

                if (_targetVec.magnitude < webDashCutOffLenght)
                {
                    ResetWebShoot();
                }
            }

            if (isDrawing)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, webShootGO.transform.position);
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            if (Parameters.azertyParameter)
            {
                keyCodeRight = KeyCode.D;
                keyCodeLeft = KeyCode.Q;
                keyCodeUp = KeyCode.Z;
                keyCodeDown = KeyCode.S;
            }
            else
            {
                keyCodeRight = KeyCode.D;
                keyCodeLeft = KeyCode.A;
                keyCodeUp = KeyCode.W;
                keyCodeDown = KeyCode.S;
            }

            //Input gauche droite
            if (Input.GetKey(keyCodeRight) && playerAngle != 90)
            {
                horizontalInput = 1;
            }
            else if (Input.GetKey(keyCodeLeft) && playerAngle != 270)
            {
                horizontalInput = -1;
            }
            else
            {
                horizontalInput = 0;
            }

            //Input haut bas
            if (Input.GetKey(keyCodeUp) && playerAngle != 180)
            {
                verticalInput = 1;
            }
            else if (Input.GetKey(keyCodeDown) && playerAngle != 0)
            {
                verticalInput = -1;
            }
            else
            {
                verticalInput = 0;
            }

            //Input saut
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    isJumping = true;
                }
                else if (!isWebDashing && !isHackedGlide)
                {
                    isGliding = !isGliding;
                }
            }

            //Input WebShoot
            if (Input.GetButtonDown("Fire2") && canShoot && !isHackedWeb)
            {
                isWebShooting = true;
                canShoot = false;
            }

            //Sprite Flip
            Flip(rb.velocity);

            //Variable pour animer
            //Speed
            float characterVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.y, 2));
            animator.SetFloat("Speed", characterVelocity);

            //Jump
            if (isJumping)
            {
                animator.SetTrigger("Jump");
            }

            //VerticalSpeed
            animator.SetFloat("VerticalSpeed", rb.velocity.y);

            //GroundCheck
            animator.SetBool("GroundCheck", isGrounded);

            //RelativeVerticalSpeed
            if (playerAngle == 0)
            {
                animator.SetFloat("RelativeVerticalSpeed",rb.velocity.y);
            }
            else if (playerAngle == 90)
            {
                animator.SetFloat("RelativeVerticalSpeed", -rb.velocity.x);
            }
            else if (playerAngle == 180)
            {
                animator.SetFloat("RelativeVerticalSpeed", -rb.velocity.y);
            }
            else if (playerAngle == 270)
            {
                animator.SetFloat("RelativeVerticalSpeed", rb.velocity.x);
            }
            
            webGlider.SetActive(isGliding);
            float webGliderAngle;
            if (rb.velocity.magnitude != 0)
            {
                webGliderAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg + 90f;
            }
            else
            {
                webGliderAngle = 0;
            }

            if (webGliderAngle > maxGliderAngle)
            {
                webGliderAngle = maxGliderAngle;
            }
            else if (webGliderAngle < -maxGliderAngle)
            {
                webGliderAngle = -maxGliderAngle;
            }

            webGlider.transform.rotation = Quaternion.Euler(0f, 0f, webGliderAngle);
        }
    }

    //Fonctions mouvements
    public void MovePlayer(float _horizontalMovement, float _verticalMovement)
    {
        Vector3 targetVelocity;

        if (playerAngle == 90)
        {
            rb.AddForce(new Vector2(stickyForce, 0f));
            targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        }
        else if (playerAngle == 270)
        {
            rb.AddForce(new Vector2(-stickyForce, 0f));
            targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        }
        else if (playerAngle == 180){
            rb.AddForce(new Vector2(0f, stickyForce));
            targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        }
        else
        {
            targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        }

        if (isGrounded && playerAngle == 0)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTimeGround);
        }
        else
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTimeAir);
        }

        

    }

    public void JumpPlayer(float _playerAngle)
    {
        if (rb.velocity.y < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        if (_playerAngle == 0)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
        }
        else if (_playerAngle == 90)
        {
            rb.AddForce(new Vector2(-jumpForce, jumpForce));
        }
        else if (_playerAngle == 270)
        {
            rb.AddForce(new Vector2(jumpForce, jumpForce));
        }
        else
        {
            rb.AddForce(new Vector2(0f, -jumpForce/2));
        }
        isJumping = false;
        //playerSound.MakeSound(0);
    }

    private void RotatePlayer(float _rotationAngle)
    {
        transform.Rotate(new Vector3(0f, 0f, _rotationAngle));
    }

    private void WebShoot()
    {
        webShootGO =  Instantiate(webShootPreFab, transform.position, transform.rotation);
        Vector2 shootDir = cam.ScreenToWorldPoint(Input.mousePosition) - new Vector3(transform.position.x, transform.position.y);
        shootDir.Normalize();
        webShootGO.GetComponent<Rigidbody2D>().velocity = shootDir * webShootSpeed;
        isDrawing = true;
    }

    public void ResetWebShoot()
    {
        isWebDashing = false;
        canShoot = true;
        isDrawing = false;
    }

    public void WebDash()
    {
        isWebDashing = true;
        StartCoroutine(WaitStopWebDashing());

        isGliding = false;
    }

    //Dégats

    public void TakeDamage()
    {
        if (canBeHit)
        {
            if (damageTaken == 0)
            {
                moveSpeed = moveSpeed * hackedSpeedCoefficient;
                abilityDisplay.activateMaskSprint();
            }
            else if (damageTaken == 1)
            {
                isHackedGlide = true;
                isGliding = false;
                abilityDisplay.activateMaskGlide();
            }
            else if (damageTaken == 2)
            {
                isHackedWeb = true;
                abilityDisplay.activateMaskWeb();
            }
            
            if (damageTaken == 3)
            {
                rb.gravityScale = 0.1f;
                GameMaster.instance.Lose();
                abilityDisplay.activateMaskLife();
                animator.SetTrigger("Ded");
            }
            else
            {
                damageTaken++;
                canBeHit = false;
                StartCoroutine(DamageAnimation());
                StartCoroutine(WaitNoHit());
            }
        }
    }

    //Coroutine
    private IEnumerator WaitStopWebDashing()
    {
        yield return new WaitForSeconds(webDashingLifeSpan);
        if (isWebDashing)
        {
            ResetWebShoot();
        }
    }
    private IEnumerator DamageAnimation()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
        yield return new WaitForSeconds(flashingPeriod);
        if (!canBeHit)
        {
            StartCoroutine(DamageAnimation());
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }

    private IEnumerator WaitNoHit()
    {
        yield return new WaitForSeconds(noHitPeriod);
        canBeHit = true;
    }

    //Fonctions autres
    void Flip(Vector3 _velocity)
    {
        if (playerAngle == 0)
        {
            if (_velocity.x > 0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (_velocity.x < -0.1f)
            {
                spriteRenderer.flipX = false;
            }
        }
        if (playerAngle == 180)
        {
            if (_velocity.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (_velocity.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
        }
        if (playerAngle == 90)
        {
            if (_velocity.y > 0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (_velocity.y < -0.1f)
            {
                spriteRenderer.flipX = false;
            }
        }
        if (playerAngle == 270)
        {
            if (_velocity.y > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (_velocity.y < -0.1f)
            {
                spriteRenderer.flipX = true;
            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wallCheckUp.position, wallCheckRadius);
    }

}
