using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter2D : MonoBehaviour
{
    [Tooltip("The fastest the player can travel in the x axis")]
    [SerializeField] private float maxSpeed = 10f;

    [Tooltip("Amount of force added when the player jumps")]
    [SerializeField] private float jumpForce = 400f;
    
    [Tooltip("Whether or not a player can steer while jumping")]
    [SerializeField] private bool airControl = false;
    
    [Tooltip("A mask determining what is ground to the character")]
    [SerializeField] private LayerMask whatIsGround;
    
    [Tooltip("How long a player can move upwards in a jump")]
    [SerializeField] private float maxJumpTime = 1f;
    
    [Tooltip("The minimum time travelling upwards in a jump")]
    [SerializeField] private float minJumpTime = 0.1f;
    
    [Tooltip("Speed a player slides down a wall")]
    [SerializeField] private float wallSlideSpeed = 1f;
    
    [Tooltip("The multiplier of the upward jumpForce for a wall jump")]
    [SerializeField] private float wallJumpMult = 0.7f;
    
    [Tooltip("How much X-force is applied after a wall jump")]
    [SerializeField] private float wallJumpXForce = 100f;
    
    [Tooltip("Multiplier of maxJumpTime for climbing up a wall")]
    [SerializeField] private float wallScrambleMult;
    
    [Tooltip("Slowly dampens the jumpForce as the player goes up")]
    [SerializeField] private float jumpDamper = 0.1f;

    [Tooltip("Multiplier for gravity when the parachute is active")]
    [SerializeField] private float parachuteMult = 0.25f;

    [Tooltip("If Skreech has unlocked gliding")]
    public bool hasGlider = true;

    [HideInInspector] public Controls controls;

    public static bool canControl = true;       // If the player character can be controlled

    private float move = 0f;            // The value of horizontal movement (from -1 to 1)
    private Transform groundCheck;      // A position marking where to check if the player is grounded.
    const float groundedRadius = .5f;   // Radius of the overlap circle to determine if grounded
    private Transform ceilingCheck;     // A position marking where to check for ceilings
    const float ceilingRadius = .01f;   // Radius of the overlap circle to determine if the player can stand up
    private Animator anim;              // Reference to the player's animator component.
    private Rigidbody2D rb;
    private bool facingRight = true;    // For determining which way the player is currently facing.
    private float jumpTimeCounter;      // Remaining time left in jump
    private bool canJumpMore = false;   // If the player can continue jumping upwards
    private Transform wallCheck;        // Position where a player touching a wall is checked
    private bool isJumping = false;     // If the player is holding the jump button down
    private bool isWallJumping = false; // If the player has jumped off a wall
    private float curJumpForce;         // Jump force that changes based on jumpDamper
    private bool canWJLeft = true;      // If the player can wall jump of a wall on the left
    private bool canWJRight = true;     // If the player can wall jump of a wall on the right
    private bool isGliding = false;     // If the player is gliding with the parachute
    private float gravity;

    [HideInInspector] public bool isGrounded;            // Whether or not the player is grounded.
    [HideInInspector] public bool isWalled = false;      // If the player is touching a wall
    [HideInInspector] public bool isSliding = false;     // If the player is sliding down a wall

    private void Awake()
    {
        Time.timeScale = 1f;
        
        // Setting up references.
        groundCheck = transform.Find("GroundCheck");
        ceilingCheck = transform.Find("CeilingCheck");
        wallCheck = transform.Find("WallCheck");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;

        controls = new Controls();

        // Read value (ranges from -1 to 1) from Move control
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => move = 0f;

        controls.Gameplay.Jump.started += ctx => Jump();
        controls.Gameplay.Jump.canceled += ctx => JumpCancel();

        controls.Gameplay.Pause.started += ctx => Pause();
    }

    // This is needed because inputs are disabled by default
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        isWalled = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                // Debug.Log("Ground found: " + colliders[i].name);
                isGrounded = true;
                isGliding = false;
                ResetWallJumpAbility();
                break;
            }
        }

        // TODO: The player should slide on a wall if a circlecast to the wallcheck position hits anything designated as ground

        anim.SetBool("Ground", isGrounded);
        anim.SetBool("Glide", isGliding);

        // If the player should be sliding down a wall...
        if (isWalled && !isGrounded && ((facingRight && canWJRight) || (!facingRight && canWJLeft)))
        {
            isSliding = true;
        }    
        else
        {
            isSliding = false;
        }
        anim.SetBool("Walled", isSliding);

        Move(move);

        // If the player is sliding down a wall, clamp vertical velocity
        if (isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }

        // If the player continues jumping, dampen (decrease) upward velocity over time
        // Jumps will continue for at least minJumpTime seconds no matter how long the player presses the jump button
        if (canJumpMore && (isJumping || jumpTimeCounter < minJumpTime)) 
        {
            if (jumpTimeCounter < maxJumpTime * (isSliding ? wallScrambleMult : 1f))
            {
                rb.velocity = new Vector2(rb.velocity.x, curJumpForce);
                jumpTimeCounter += Time.deltaTime;
                curJumpForce -= jumpDamper;
            }
            else
            {
                isJumping = isWallJumping = false;
            }
        }
        else
        {
            canJumpMore = isJumping = isWallJumping = false;
        }

        // TODO: If the player is gliding, constrain downward velocity and lower gravity
        if (isGliding)
        {
            
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // Set the vertical animation
        anim.SetFloat("vSpeed", rb.velocity.y);

        // If currently facing right and (getting forced left or moving left into a wall), flip sprite horizontally
        if (facingRight && (rb.velocity.x < -1.0f || 
        (Mathf.Abs(rb.velocity.x) <= float.Epsilon && canControl && move < -float.Epsilon)))
            Flip();
        else if (!facingRight && (rb.velocity.x > 1.0f || 
        (Mathf.Abs(rb.velocity.x) <= float.Epsilon && canControl && move > float.Epsilon)))
            Flip();
    }

    public void Move(float move)
    {
        // Only control the player if grounded or airControl is turned on
        if (canControl && (isGrounded || airControl || isSliding))
        {
            // The Speed animator parameter is set to the absolute value of the horizontal input.
            anim.SetFloat("Speed", Mathf.Abs(move));

            // Move the character if speed doesn't exceed maxSpeed, but allow for turning around
            if ((move > 0 && rb.velocity.x < maxSpeed) || (move < 0 && rb.velocity.x > -maxSpeed))
            {
                rb.AddForce(new Vector2(move * maxSpeed * 10f, 0));
            }
        }
    }

    public void Jump()
    {
        if (canControl)
        {
            // If the player jumps from the ground...
            if (isGrounded && anim.GetBool("Ground"))
            {
                JumpHelper();
            }
            // If the player jumps from a wall...
            else if (isSliding && anim.GetBool("Walled"))
            {
                JumpHelper();

                isSliding = false;
                isWallJumping = true;
                anim.SetBool("Walled", false);

                // TODO: Add horizontal force (maxSpeed * wallJumpXForce) from kicking off a wall

                // Restrict walljumping from a singular wall (comment this block of code out to enable single-wall climbing)
                if(airControl)
                {
                    if (facingRight)
                    {
                        canWJLeft = true;
                        canWJRight = false;
                    }
                    else
                    {
                        canWJLeft = false;
                        canWJRight = true;
                    }
                }
            }
            // TODO: If the player attempts to glide... (Change false to something that would correspond to using the glider when pushing the jump button)
            else if (false)
            {
                isGliding = true;

                // TODO: Prevent gliding upwards
            }
        }
    }
    private void JumpHelper()
    {
        isJumping = canJumpMore = true;
        isGrounded = false;
        anim.SetBool("Ground", false);
        jumpTimeCounter = 0f;

        // Add a vertical force to the player.
        curJumpForce = jumpForce * (isWallJumping ? wallJumpMult : 1);
    }

    private void JumpCancel()
    {
        isJumping = isGliding = false;
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;
        // Rotate on the y-axis by 180 degrees
        transform.Rotate(new Vector3(0,180,0));
    }

    public void Pause() 
    {
        GameObject pauseMenu = FindObjectOfType<CanvasGroup>().transform.GetChild(0).gameObject;
        GameObject optionsMenu = FindObjectOfType<CanvasGroup>().transform.GetChild(1).gameObject;
        bool paused = pauseMenu.activeSelf || optionsMenu.activeSelf;

        if (!paused) 
        {
            pauseMenu.SetActive(true);
            controls.Gameplay.Disable();
        }
        else 
        {
            FindObjectOfType<ButtonListeners>().GetComponent<ButtonListeners>().OnClickResume();
        }

        optionsMenu.SetActive(false);
    }

    public void ResetWallJumpAbility()
    {
        canWJLeft = canWJRight = true;
    }
}