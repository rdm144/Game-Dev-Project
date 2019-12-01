using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJump : MonoBehaviour
{
    public bool OnWall;
    public Transform wallCheck;
    Rigidbody2D rb;
    PlayerMovement pm;
    PlayerDash pd;
    public bool JumpInput;
    const float SLIDE_GRAVITY = 3f; // wall-sliding gravity
    bool isDashing;
    float CurrentSpeed;
    KeyCode JumpKey;
    public LayerMask wallMask;

    private void Start()
    {
        JumpKey = InputHandler.jump_key;
        OnWall = false;
        isDashing = false;
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        wallMask = pm.groundMask;
        if (GetComponent<PlayerDash>() != null)
            pd = GetComponent<PlayerDash>();
    }

    private void Update()
    {
        if (pd != null) isDashing = pd.DashInput;
        CheckCanWallJump();                            // check if can wall jump
        if (OnWall == true)                            // if on wall, disable normal gravity, and slide down slowly or jump off
        {
            pm.runApplyGravity = false;                // disable normal gravity
            GetInput();                                // check for jump input
            VerticalMovement();                        // either jump off the wall, or slide down
        }
        else                                           // if not on wall, use normal gravity
        {
            pm.runApplyGravity = true;                 // Tell PlayerMovement to run ApplyGravity()
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(JumpKey)) // Get jump button input
        {
            JumpInput = true;
        }
    }

    void CheckCanWallJump()
    {
        // Check if next to wall
        //bool NextToWall = Physics.CheckBox(wallCheck.position, wallCheck.localScale / 2, Quaternion.identity, pm.groundMask);
        bool NextToWall = false;
        Vector3 pos = new Vector3(wallCheck.position.x - wallCheck.localScale.x * pm.Direction, wallCheck.position.y, wallCheck.position.z);
        RaycastHit2D hit = Physics2D.BoxCast(pos, wallCheck.localScale / 2, 0, transform.right, wallCheck.localScale.x, wallMask);
        if (hit)
        {
            //Debug.Log(hit.normal.normalized);
            if (hit.normal.normalized == (Vector2)transform.right.normalized * -1)
                NextToWall = true;
        }

        // if not grounded, is falling, holding a direction, and next to a wall, then you are wall sliding
        if (pm.IsGrounded == false && rb.velocity.y <= 0 && (pm.LeftInput || pm.RightInput || isDashing) && NextToWall == true)
            OnWall = true;
        else
            OnWall = false;
    }

    void VerticalMovement()
    {
        if (JumpInput) // if wall jumping, use normal gravity and hop off the wall
        {
            if (isDashing)
                CurrentSpeed = pm.DashSpeed * 0.8f;
            else
                CurrentSpeed = pm.RunSpeed * 1.25f;
            Vector2 kickForce = (-1f * pm.Direction * Vector2.right * CurrentSpeed) + (Vector2.up * pm.JumpSpeed);
            rb.AddForce(kickForce, ForceMode2D.Impulse);
            JumpInput = false;
        }
        else // if not wall jumping, then slide down
            ApplyWallSlideGravity();
    }

    void ApplyWallSlideGravity()
    {
        if(rb.velocity.y <= 0)                                             // cancel any built-up vertical speed
            ResetVerticalVelocity();
        rb.gravityScale = SLIDE_GRAVITY;
    }

    void ResetVerticalVelocity()
    {
        Vector3 newVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        rb.velocity = newVelocity;
    }
}