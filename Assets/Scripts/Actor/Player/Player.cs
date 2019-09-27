using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public bool LeftInput, RightInput, JumpInput;
    float RunSpeed;
    Vector2 JumpForce;
    Rigidbody2D rb;
    KeyCode LeftKey, RightKey, JumpKey;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RunSpeed = 15;
        JumpForce = Vector2.up * 15;
        SetDoubleJumpPermitted(true);
        SetWallJumpPermitted(true);

        LeftKey = KeyCode.A;
        RightKey = KeyCode.D;
        JumpKey = KeyCode.Space;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfOnGround();
        GetKeyboardInput();
        FaceForward();

        //if (CanWallJump()) Debug.Log("CanWallJump");
    }

    void FixedUpdate()
    {
        VerticalMovement();
        HorizontalMovement();
    }

    /// <summary>
    /// Check keyboard inputs
    /// </summary>
    void GetKeyboardInput()
    {
        // Jump
        if (Input.GetKeyDown(JumpKey) && (GetIsGrounded() == true || CanDoubleJump))
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

    /// <summary>
    /// Flip the player's sprite to face forward
    /// </summary>
    void FaceForward()
    {
        if (LeftInput) SetDirection(-1);
        else if (RightInput) SetDirection(1);

        transform.rotation = Quaternion.Euler(0, 90 - GetDirection() * 90, 0);
    }

    /// <summary>
    /// Move the player forward if trying to run
    /// </summary>
    void HorizontalMovement()
    {
        if (LeftInput || RightInput)
            rb.velocity = new Vector2(RunSpeed * GetDirection(), rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    /// <summary>
    /// Push the player upward if trying to jump
    /// </summary>
    void VerticalMovement()
    {
        if (JumpInput)
        {
            if (!IsGrounded) {
                SetHasDoubleJumped(true);
                ResetVerticalMovement();
            }
            rb.AddForce(JumpForce, ForceMode2D.Impulse);
            JumpInput = false;
        }
        AlterGravity();
    }

    /// <summary>
    /// Changes the player's gravity depending on their type of jump
    /// </summary>
    void AlterGravity()
    {
        float NormalFallGravity = 4;
        float ShortHopGravity = 6;
        float DefaultGravity = 3;

        if (rb.velocity.y < 0)                                             // Check if player is moving downwards
            rb.gravityScale = NormalFallGravity;                           // Increase gravity during fall
        else if (rb.velocity.y > 0 && Input.GetKey(JumpKey) == false)      // Check if player is doing a short hop
            rb.gravityScale = ShortHopGravity;                             // Apply higher gravity during hop
        else
            rb.gravityScale = DefaultGravity;                              // Apply normal gravity
    }

    /// <summary>
    /// Sets the y vector of the player's movement to 0.
    /// </summary>
    void ResetVerticalMovement() {
        Vector2 vel = rb.velocity;
        vel.y = 0;
        rb.velocity = vel;
    }
}