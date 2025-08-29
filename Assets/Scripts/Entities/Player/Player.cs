using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Player Component
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Animator playerAnim;

    #endregion

    [Header("Timer")]
    [Range(0.1f, 0.5f)] public float coyoteTime;
    public float lastGroundTime = 0;
    private float lastJumpTime;

    [Header("GroundChecks")]
    public float checkDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int groundLayerIndex;
    public bool grounded;

    [Header("Run")]
    public float runMaxSpeed;
    private float targetSpeed;
    public float movementSpeed;
    public float acceleration;
    public float decceleration;
    private Vector3 mousePos;

    [Header("Jump")]
    public float jumpHeight;
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
    private float gravityScale;

    [Header("Respawn")]
    public float respawnOffset;
    public float fallThreshold;
    private GameObject currPlatform;

    [Header("Animations")]
    [SerializeField] GameObject runAnimEffect;
    [SerializeField] GameObject jumpAnimEffect;
    [SerializeField] GameObject doubleJumpAnimEffect;
    [SerializeField] GameObject wallJumpAnimEffect;

    [Header("Misc")]
    public float frictionAmount;

    [SerializeField] int platformCount = 0;

    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        playerCollider = this.GetComponent<Collider2D>();
        playerAnim = this.GetComponent<Animator>();

        gravityScale = rb.gravityScale;
    }

    // Start is called before the first frame update
    void Start()
    {

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

        if (currPlatform != null)
        {
            PlayerFailSafety();
        }
        #endregion

        #region Run

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0) && !isJumping)
        {
            if (mousePos.x < 0)
            {
                targetSpeed = -runMaxSpeed;
            }
            else
            {
                targetSpeed = runMaxSpeed;
            }

            runAnimEffect.SetActive(true);

        }
        else
        {
            targetSpeed = 0;
            runAnimEffect.SetActive(false);
        }

        Run(1);

        #endregion

        //applying friction to the player to decrease the slippery feeling while deccelerating
        #region Friction

        //check if player is grounded and not currently trying to move
        if (grounded && !Input.GetMouseButton(0))
        {
            //use either friction amount or the players current velocity
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            //adjust to the current movement direction
            amount *= Mathf.Sign(rb.velocity.x);
            //applies force against the current movement direction
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion

        #region Jump

        if (Input.GetMouseButtonUp(0) && lastJumpTime <= jumpCD)
        {
            if (!isJumping && lastGroundTime > 0)
            {
                Jump();
            }
            else if (onWall && !wallJumped)
            {
                float wallDir = -Mathf.Sign(gameObject.transform.position.x);
                WallJump(wallDir, wallJumpForceMult);
                jumpCount--;
                wallJumped = true;
                wallJumpAnimEffect.SetActive(true);
            }
            else if (jumpCount < 2)
            {
                wallJumped = false;
                float dir = Mathf.Sign(mousePos.x);
                WallJump(dir, doubleJumpForceMult);
                doubleJumpAnimEffect.SetActive(true);
            }

            jumpCount++;
        }

        #endregion

        #region JumpGravity
        if (isJumping && onWall && lastJumpTime <= jumpCD && rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * wallGravityMult;
        }
        else if (isJumping && Mathf.Abs(rb.velocity.y) < jumpHangThreshold)
        {
            rb.gravityScale = gravityScale * jumpHangMultiplier;
        }
        else if (rb.velocity.y < 0)
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
        if (Mathf.Sign(direction.x) != Mathf.Sign(rb.velocity.x) && rb.velocity.x != 0)
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

    private bool IsGrounded()
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
        return hit.collider != null;
    }

    private void Run(float lerpAmount)
    {
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        float acccelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

        if (isJumping && Mathf.Abs(rb.velocity.y) < jumpHangThreshold)
        {
            acccelRate *= jumpHangAccelMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        float speedDif = targetSpeed - rb.velocity.x;
        movementSpeed = speedDif * acccelRate;
    }

    private void Jump()
    {
        lastJumpTime = 0;

        float force = jumpHeight;
        if (rb.velocity.y < 0)
        {
            force -= rb.velocity.y;
        }

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        isJumping = true;
        lastGroundTime = 0;

        playerAnim.SetTrigger("jump");
        jumpAnimEffect.SetActive(true);
    }

    private void WallJump(float dir, float forceMult)
    {
        lastJumpTime = 0f;

        Vector2 force = new Vector2(runMaxSpeed * jumpSpeedDecrease, jumpHeight);
        force.x *= dir;
        force.x *= forceMult;

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= rb.velocity.x;
        }

        if (Mathf.Sign(rb.velocity.y) < 0)
        {
            force.y -= rb.velocity.y;
        }

        rb.AddForce(force, ForceMode2D.Impulse);

        playerAnim.SetTrigger("jump");
    }

    private void PlayerFailSafety()
    {
        if (gameObject.transform.position.y < currPlatform.transform.position.y - fallThreshold)
        {
            Vector3 respawnPos = new Vector3(currPlatform.transform.position.x,
                                             currPlatform.transform.position.y + respawnOffset,
                                             currPlatform.transform.position.z);
            gameObject.transform.position = respawnPos;
        }
    }

    private void SetAnimParameters()
    {
        playerAnim.SetBool("running", targetSpeed != 0);
        playerAnim.SetBool("onWall", onWall && grounded);
        playerAnim.SetBool("falling", rb.velocity.y <= -jumpHangThreshold);
        playerAnim.SetBool("grounded", grounded);
    }

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
                if (platformY > currPlatform.transform.position.y && grounded)
                {
                    currPlatform = collision.gameObject;

                    platformCount++;
                    Debug.Log("Here");
                }
            }

        }

    }
}
