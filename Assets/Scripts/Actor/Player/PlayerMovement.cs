using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerDash pd;
    Shader shaderGUItext;

    const float RUN_SPEED = 15;
    const float DASH_SPEED = RUN_SPEED * 2.5f;
    const float ACCELERATION_RATE = 5;
    const float JUMP_SPEED = 15f;
    const float NORMAL_GRAVITY = 3; // jumping upwards
    const float FALL_GRAVITY = 4; // falling from full jump
    const float HOP_GRAVITY = 6; // hopping

    [Range(-1, 1)] public int Direction;

    public bool IsGrounded;
    public bool CanRotate;
    public float gravityScale;

    public Transform groundCheck;
    public LayerMask groundMask;

    public bool LeftInput, RightInput, JumpInput, runApplyGravity, runHorizontalMovement;

    KeyCode LeftKey, RightKey, JumpKey;

    public float JumpSpeed { get { return JUMP_SPEED; } }
    public float DashSpeed { get { return DASH_SPEED; } }
    public float RunSpeed { get { return RUN_SPEED; } }
    public float NormalGravity { get { return NORMAL_GRAVITY; } }
    public float FallGravity { get { return FALL_GRAVITY; } }
    public float HopGravity { get { return HOP_GRAVITY; } }
    public float AccelerationRate { get { return ACCELERATION_RATE; } }

    // Start is called before the first frame update
    void Start()
    {
        Direction = 1;
        gravityScale = NORMAL_GRAVITY;
        IsGrounded = true;
        CanRotate = true;
        rb = GetComponent<Rigidbody2D>();
        if (GetComponent<PlayerDash>() != null)
            pd = GetComponent<PlayerDash>();
        runApplyGravity = true;
        runHorizontalMovement = true;
        LeftKey = InputHandler.left_key; // Hard-coded keybinds. Remove later.
        RightKey = InputHandler.right_key;
        JumpKey = InputHandler.jump_key;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        GetInput();
        FaceForward();

        // Prevent accidental super-jumps from frame-perfect bunny hopping
        float velY = Mathf.Clamp(rb.velocity.y, -3 * JUMP_SPEED, JUMP_SPEED);
        rb.velocity = new Vector3(rb.velocity.x, velY, 0);
    }

    private void FixedUpdate()
    {
        VerticalMovement();
        bool isAirDashing = false;
        if (pd != null) { isAirDashing = pd.IsAirDashing; }
        if (runApplyGravity && isAirDashing == false) { ApplyGravity(); }
        if (runHorizontalMovement) { HorizontalMovement(); }
    }

    void GetInput()
    {
        // Jump
        if (Input.GetKeyDown(JumpKey) && IsGrounded == true)
        {
            JumpInput = true;
        }
        // Left or Right movement
        if (Input.GetKey(LeftKey) && !Input.GetKey(RightKey))
        {
            LeftInput = true;
            RightInput = false;
        }
        else if (Input.GetKey(RightKey) && !Input.GetKey(LeftKey))
        {
            LeftInput = false;
            RightInput = true;
        }
        else
        {
            LeftInput = false;
            RightInput = false;
        }
    }

    public void FaceForward()
    {
        if (LeftInput) Direction = -1;
        else if (RightInput) Direction = 1;

        if(CanRotate)
            transform.rotation = Quaternion.Euler(0, 90 - Direction * 90, 0);
    }

    void CheckForGround()
    {
        //IsGrounded = Physics.CheckBox(groundCheck.position, groundCheck.localScale / 2, Quaternion.identity, groundMask);
        Vector3 pos = new Vector3(groundCheck.position.x, groundCheck.position.y + groundCheck.localScale.y, groundCheck.position.z);
        RaycastHit2D hit = Physics2D.BoxCast(pos, groundCheck.localScale / 2, 0, transform.up * -1, groundCheck.localScale.y, groundMask);
        if (hit)
        {
            if (hit.normal.normalized == (Vector2)transform.up.normalized)
                IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    void HorizontalMovement()
    {
        if (LeftInput || RightInput)
        {
            Vector2 targetVelocity = RUN_SPEED * Direction * Vector2.right;
            float Accel_Multiplier = 1;
            if (IsGrounded)
                Accel_Multiplier = 3;
            Vector2 force = new Vector2(targetVelocity.x - rb.velocity.x, 0) * ACCELERATION_RATE * Accel_Multiplier;
            rb.AddForce(force, ForceMode2D.Force);
        }
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void VerticalMovement()
    {
        if (JumpInput)
        {
            rb.AddForce(Vector2.up * JUMP_SPEED, ForceMode2D.Impulse);
            JumpInput = false;
        }
    }

    void ApplyGravity()
    {
        if (rb.velocity.y < 0)                                            // Check if player is moving downwards
            gravityScale = FALL_GRAVITY;                                  // Increase gravity during fall
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))       // Check if player is short hopping
            gravityScale = HOP_GRAVITY;                                   // Apply very high gravity during hop
        else                                                              // Else player must be moving upwards
            gravityScale = NORMAL_GRAVITY;                                // Apply normal upwards gravity
        rb.gravityScale = gravityScale;
    }
}