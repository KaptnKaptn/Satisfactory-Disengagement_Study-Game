using System;
using System.Collections;
using UnityEngine;


//player movement script
public class Player_Joystick : MonoBehaviour
{
    #region Player Component
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Animator playerAnim;
    private SpriteRenderer playerSprite;

    #endregion

    [Header("Timer")]
    private float lastGroundTime = 0;
    private float lastJumpTime;
    [Range(0.1f, 0.5f)] public float coyoteTime;

    [Header("GroundChecks")]
    public float checkDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int groundLayerIndex;
    private bool grounded;

    [Header("Input")]
    private float horizontalMovement;
    private float verticalMovement;

    [Header("Run")]
    public float runMaxSpeed;
    private float targetSpeed;
    public float movementSpeed;
    public float acceleration;
    public float decceleration;
    public float accelInAir;
    public float deccelInAir;

    [Header("Jump")]
    public bool jumpPressed;
    public bool jumpReleased;
    public float baseJumpHeight;
    public float maxJumpHeight;
    public float jumpHeightAccel;
    public float jumpHeight;
    public Color maxReached;

    public float jumpHangMultiplier;
    public float jumpHangThreshold;
    public float jumpHangAccelMult;
    public float jumpHangMaxSpeedMult;
    public float jumpSpeedDecrease;

    public float doubleJumpForceMult;
    public float jumpCD;
    private bool isJumping;
    private int jumpCount;

    [Header("WallJump")]
    public LayerMask wallLayer;
    public float wallGravityMult;
    public float wallJumpForceMult;
    private bool onWall;
    private bool wallJumped;

    [Header("Gravity")]
    public float fallGravityMultiplier;
    public float fastFallGravityMultiplier;
    private float gravityScale;

    [Header("Respawn")]
    public float respawnOffset;
    public float fallThreshold;
    private GameObject currPlatform;
    public bool respawning;

    [Header("Animations")]
    [SerializeField] GameObject runAnimEffect;
    [SerializeField] GameObject jumpAnimEffect;
    [SerializeField] GameObject jumpChargeEffect;
    [SerializeField] GameObject doubleJumpAnimEffect;
    [SerializeField] GameObject wallJumpAnimEffect;

    [Header("Misc")]
    public Joystick joystick;
    public float frictionAmount;
    private GameManager gameManager;

    void Awake()
    {
        #region Player Setup
        rb = this.GetComponent<Rigidbody2D>();
        playerCollider = this.GetComponent<Collider2D>();
        playerAnim = this.GetComponent<Animator>();
        playerSprite = this.GetComponent<SpriteRenderer>();

        gravityScale = rb.gravityScale;
        jumpHeight = baseJumpHeight;

        currPlatform = null;
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        #region Timer

        lastGroundTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;

        #endregion

        #region Checks

        if (IsGrounded() && lastJumpTime <= jumpCD * 2)
        {
            grounded = true;
            isJumping = false;
            wallJumped = false;
            lastGroundTime = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            grounded = false;
        }

        if (playerCollider.IsTouchingLayers(wallLayer) && isJumping)
        {
            onWall = true;
        }
        else
        {
            onWall = false;
        }

        if (currPlatform != null && !respawning)
        {
            PlayerFailSafety();
        }
        #endregion
    
        #region Inputs
        if (!respawning)
        {
            if (Application.isEditor)
            {
                horizontalMovement = Input.GetAxis("Horizontal");
                verticalMovement = Input.GetAxis("Vertical");       //used to allow fast falling
            }
            else
            {
                horizontalMovement = joystick.Horizontal;
                verticalMovement = joystick.Vertical;               //used to allow fast falling
            }
        }
        #endregion

        #region Run

        targetSpeed = horizontalMovement * runMaxSpeed;

        if (joystick.Horizontal != 0)
        {
            runAnimEffect.SetActive(true);
        }
        else
        {
            runAnimEffect.SetActive(false);
        }

        Run(1);

        #endregion

        //applying friction to the player to decrease the slippery feeling while deccelerating
        #region Friction

        //check if player is grounded and not currently trying to move
        if (grounded && horizontalMovement == 0)
        {
            //use either friction amount or the players current velocity
            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(frictionAmount));
            //adjust to the current movement direction
            amount *= Mathf.Sign(rb.linearVelocity.x);
            //applies force against the current movement direction
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion

        #region Jump

        //let the player hold down the jump button to increase the height
        //notify once max charge reached
        if (grounded && (jumpPressed || Input.GetKey(KeyCode.Space)))
        {
            if (jumpHeight < maxJumpHeight)
            {
                jumpHeight += Time.deltaTime * jumpHeightAccel;
                jumpChargeEffect.SetActive(true);
            }
            else
            {
                jumpChargeEffect.SetActive(false);
                playerSprite.color = maxReached;
            }
        }

        //release the corresponding jump mechanic
        if ((jumpReleased || Input.GetKeyUp(KeyCode.Space)) && lastJumpTime <= jumpCD)
        {
            jumpPressed = false;
            jumpChargeEffect.SetActive(false);
            playerSprite.color = Color.white;

            //Jump
            if (!isJumping && lastGroundTime > 0)
            {
                Jump();
                jumpAnimEffect.SetActive(true);
                playerAnim.SetTrigger("jump");

                gameManager.AddEventToLog("Jump");
            }
            //Wall Jump
            else if (onWall && !wallJumped)
            {
                float wallDir = -Mathf.Sign(gameObject.transform.position.x);
                WallJump(wallDir, wallJumpForceMult);
                jumpCount--;
                wallJumped = true;
                wallJumpAnimEffect.SetActive(true);
                playerAnim.SetTrigger("jump");

                gameManager.AddEventToLog("WallJump");
            }
            //Double Jump
            else if (jumpCount < 2)
            {
                Jump();
                doubleJumpAnimEffect.SetActive(true);
                playerAnim.SetTrigger("doubleJump");

                gameManager.AddEventToLog("DoubleJump");
            }

            jumpCount++;
            jumpReleased = false;
            jumpHeight = baseJumpHeight;
        }

        #endregion

        //adjust gravity for mechanics or too smooth out gameplay
        #region JumpGravity
        //Respawn gravity
        if (respawning)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
            movementSpeed = 0;
            targetSpeed = 0;
        }
        //Wall slide gravity
        else if (isJumping && onWall && lastJumpTime <= jumpCD && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * wallGravityMult;
        }
        //Fast fall gravity
        else if (rb.linearVelocity.y < 0 && verticalMovement < 0)
        {
            rb.gravityScale = gravityScale * fastFallGravityMultiplier;
        }
        //Jump hang gravity -> smoother jump apex
        else if (isJumping && Mathf.Abs(rb.linearVelocity.y) < jumpHangThreshold)
        {
            rb.gravityScale = gravityScale * jumpHangMultiplier;
        }
        //Fall gravity -> smoother jump feel
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        #endregion

        #region Animations
        Vector3 direction = transform.localScale;
        if (Mathf.Sign(direction.x) != Mathf.Sign(rb.linearVelocity.x) && rb.linearVelocity.x != 0)
        {
            direction.x *= -1;
            transform.localScale = direction;

        }

        SetAnimParameters();
        #endregion

    }

    void FixedUpdate()
    {
        rb.AddForce(movementSpeed * Vector2.right);
    }

    public void JumpButton()
    {
        if (!respawning)
        {
            jumpPressed = true;
        }
    }

    public void JumpRelease()
    {
        if (jumpPressed)
        {
            jumpReleased = true;
        }
    }

    //simple ground check 
    public bool IsGrounded()
    {
        Vector2 origin = new Vector2(
            playerCollider.bounds.center.x,
            playerCollider.bounds.min.y
            );

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            new Vector2(playerCollider.bounds.size.x * 0.99f, 0.001f),
            0f,
            Vector2.down,
            checkDistance,
            groundLayer
        );
        return hit.collider != null && rb.linearVelocity.y == 0;
    }


    private void Run(float lerpAmount)
    {
        //calculate target speed adjusted by current x-velocity
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);
        float acccelRate = 0f;

        //differ between horizontal air and ground movement
        if (lastGroundTime > 0)
        {
            acccelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        }
        else
        {
            acccelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelInAir : deccelInAir;
        }

        //create jump hang to smooth out the jump apex
        if (isJumping && Mathf.Abs(rb.linearVelocity.y) < jumpHangThreshold)
        {
            acccelRate *= jumpHangAccelMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        //calculate speed increase needed to smooth out direction changes
        float speedDif = targetSpeed - rb.linearVelocity.x;
        //calculate final movement speed 
        movementSpeed = speedDif * acccelRate;
    }

    private void Jump()
    {
        gameManager.PlaySFX("Jump");

        lastJumpTime = 0f;

        float force = jumpHeight;
        if (rb.linearVelocity.y < 0)
        {
            force -= rb.linearVelocity.y;
        }

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        isJumping = true;
        lastGroundTime = 0;
    }

    private void WallJump(float dir, float forceMult)
    {
        gameManager.PlaySFX("Jump");

        lastJumpTime = 0f;

        Vector2 force = new Vector2(runMaxSpeed * jumpSpeedDecrease, jumpHeight * forceMult);
        force.x *= dir;
        force.x *= forceMult;

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
        {
            force.x -= rb.linearVelocity.x;
        }

        if (Mathf.Sign(rb.linearVelocity.y) < 0)
        {
            force.y -= rb.linearVelocity.y;
        }

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    //logic behind player respawn on death
    private void PlayerFailSafety()
    {
        if (gameObject.transform.position.y < currPlatform.transform.position.y - fallThreshold)
        {
            gameManager.AddEventToLog("Death");

            Vector3 respawnPos = new Vector3(currPlatform.transform.position.x,
                                             currPlatform.transform.position.y + respawnOffset,
                                             currPlatform.transform.position.z);

            StartCoroutine(PlayerTeleport(respawnPos));
        }
    }

    //player death cutscene
    private IEnumerator PlayerTeleport(Vector3 respawnPos)
    {
        respawning = true;
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetTrigger("despawn");


        yield return new WaitForSeconds(1f);
        gameObject.transform.position = respawnPos;
        playerAnim.SetTrigger("respawn");
        gameManager.PlaySFX("Teleport");
        yield return new WaitForSeconds(1f);
        respawning = false;
    }

    private void SetAnimParameters()
    {
        playerAnim.SetBool("running", targetSpeed != 0);
        playerAnim.SetBool("onWall", onWall && !grounded && !respawning);
        playerAnim.SetBool("falling", rb.linearVelocity.y <= -jumpHangThreshold);
        playerAnim.SetBool("grounded", grounded);
    }

    public void EndGame()
    {
        respawning = true;
    }

    //used to safe current highest platform to use in the respawn logic
    //horizontal moving platforms not respawnable due to bugs
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayerIndex)
        {
            float platformY = collision.gameObject.transform.position.y;
            if (collision.gameObject == currPlatform)
            {
                return;
            }
            else if (currPlatform == null)
            {
                currPlatform = collision.gameObject;
            }
            else
            {
                if (platformY > currPlatform.transform.position.y &&
                    grounded &&
                    collision.tag != "NotRespawnable")
                {
                    currPlatform = collision.gameObject;
                }
            }

            if (currPlatform.tag != "Untagged")
            {
                Platform platform = currPlatform.GetComponent<Platform>();
                gameManager.UpdateLatestPlatform(platform.platformID);
            }
        }
    }
}