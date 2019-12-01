using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoubleJump : MonoBehaviour
{
    PlayerMovement pm;
    PlayerWallJump pwj;
    Rigidbody2D rb;
    public bool JumpInput;
    public bool HasDoubleJumped;

    KeyCode JumpKey;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        if (GetComponent<PlayerWallJump>() != null)
            pwj = GetComponent<PlayerWallJump>();
        JumpKey = InputHandler.jump_key;
    }

    // Update is called once per frame
    void Update()
    {
        bool onWall = false;
        if (pwj != null)
            onWall = pwj.OnWall;
        if (onWall == false && pm.IsGrounded == false) // check if player is in the air
        {
            GetInput();
            VerticalMovement();
        }
        else
        {
            HasDoubleJumped = false;
            JumpInput = false;
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(JumpKey) && HasDoubleJumped == false) // Get jump button input
        {
            JumpInput = true;
        }
    }

    void VerticalMovement()
    {
        if (JumpInput == true)
        {
            ResetVerticalVelocity();
            rb.AddForce(Vector2.up * pm.JumpSpeed, ForceMode2D.Impulse);
            HasDoubleJumped = true;
            JumpInput = false;
        }
    }

    void ResetVerticalVelocity()
    {
        Vector3 newVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        rb.velocity = newVelocity;
    }
}