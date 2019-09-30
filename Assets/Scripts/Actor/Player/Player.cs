﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public bool LeftInput, RightInput, JumpInput, DashInput;
    bool IsAirDashing, IsGroundDashing;
    float RunSpeed, DashSpeed;
    int AirDashCounter, MaxAirDashFrames;
    Vector2 JumpForce;
    Rigidbody2D rb;
    Shader shaderGUItext;
    KeyCode LeftKey, RightKey, JumpKey, DashKey;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RunSpeed = 15;
        DashSpeed = RunSpeed * 2;
        JumpForce = Vector2.up * 15;
        SetDoubleJumpPermitted(true);
        SetWallJumpPermitted(true);
        AirDashCounter = 0; // Holds the current number of airdash frames
        MaxAirDashFrames = 20; // Allow airdash for 20 fixed update frames
        IsAirDashing = false;
        IsGroundDashing = false;
        shaderGUItext = Shader.Find("GUI/Text Shader"); // Shader for after-images when dashing

        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        JumpKey = KeyCode.Space;
        DashKey = KeyCode.LeftShift;
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
        DashInput = Input.GetKey(DashKey); // Dash input
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
        if (GetIsGrounded()) AirDashCounter = 0; // Reset airdash counter when on the ground
        if (DashInput)
            Dash();
        else
        {
            IsGroundDashing = false;
            IsAirDashing = false;
            if (LeftInput || RightInput)
                rb.velocity = new Vector2(RunSpeed * GetDirection(), rb.velocity.y);
            else
                rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Dash()
    {
        bool CanAirDash = ((AirDashCounter < MaxAirDashFrames) ? true : false); // Check if we are allowed to airdash
        CreateAfterImage();
        if (!GetIsGrounded() && CanAirDash && !IsAirDashing && !IsGroundDashing) // Begin air dash state
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(DashSpeed * GetDirection(), 0);
            IsAirDashing = true;
            AirDashCounter++;
        }
        else if (IsAirDashing && CanAirDash && !GetIsGrounded() && Mathf.Abs(rb.velocity.x) != 0) // Continue air dash
        {
            rb.velocity = new Vector2(DashSpeed * GetDirection(), 0);
            AirDashCounter++;
        }
        else // Either dash-jumping or dashing along the ground
        {
            rb.velocity = new Vector2(GetDirection() * DashSpeed, rb.velocity.y);
            IsGroundDashing = true;
            IsAirDashing = false;
        }
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
        if(!IsAirDashing)
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

    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject();                                                   // Create a new after-image object
        afterImage.name = "After Image";                                                            // Rename the object for debugging purposes
        afterImage.AddComponent<SpriteRenderer>();                                                  // give the after-image a sprite renderer
        afterImage.AddComponent<InstancedAfterImage>();                                             // add a script to monitor the after-image's transparency and destruction
        afterImage.transform.position = transform.position;                                         // place the after-image at the player's position
        afterImage.transform.localScale = transform.localScale;                                     // scale the after-image to the player
        afterImage.transform.rotation = transform.rotation;                                         // rotate the after-image according to the player's rotation
        afterImage.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;   // set the after-image's sprite to the player sprite
        afterImage.GetComponent<SpriteRenderer>().material.shader = shaderGUItext;                  // set sprite material to GUI Text
        afterImage.GetComponent<SpriteRenderer>().color = Color.red;                               // set the after-image's color to red
    }
}