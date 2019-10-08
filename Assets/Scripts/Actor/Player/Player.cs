using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public bool LeftInput, RightInput, JumpInput, DashInput;
    bool IsAirDashing, IsGroundDashing, HasWallJumped;
    float RunSpeed, DashSpeed;
    int AirDashCounter, MaxAirDashFrames, PostWallJumpTimer, MaxPostWallJumpRestrictionFrames;
    Vector2 JumpForce;
    Rigidbody2D rb;
    Shader shaderGUItext;
    KeyCode LeftKey, RightKey, JumpKey, DashKey, ColourChangeKey;

    // Note: Could move to Actor class
    public enum Colour { Red, Green, Blue, Yellow }
    Colour colour;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RunSpeed = 15;
        DashSpeed = RunSpeed * 2;
        JumpForce = Vector2.up * 15;
        //SetDoubleJumpPermitted(true);
        //SetWallJumpPermitted(true);
        AirDashCounter = 0; // Holds the current number of airdash frames
        MaxAirDashFrames = 20; // Allow airdash for 20 fixed update frames
        PostWallJumpTimer = 0;
        MaxPostWallJumpRestrictionFrames = 4;
        IsAirDashing = false;
        IsGroundDashing = false;
        HasWallJumped = false;
        shaderGUItext = Shader.Find("GUI/Text Shader"); // Shader for after-images when dashing
        colour = Colour.Yellow;

        LeftKey = KeyCode.A; // Hard-coded keybinds. Remove later.
        RightKey = KeyCode.D;
        JumpKey = KeyCode.Space;
        DashKey = KeyCode.LeftShift;
        ColourChangeKey = KeyCode.Tab;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfOnGround();
        SetWallJumpPermitted(CanWallJump());
        GetKeyboardInput();
        FaceForward();
        UpdateColour();
        //if (CanWallJump()) Debug.Log("CanWallJump");
    }

    void FixedUpdate()
    {
        if(HasWallJumped)
        {
            PostWallJumpTimer++;
            if(PostWallJumpTimer >= MaxPostWallJumpRestrictionFrames)
            {
                HasWallJumped = false;
                PostWallJumpTimer = 0;
            }
        }
        else
            HorizontalMovement();
        VerticalMovement();
        
    }

    /// <summary>
    /// Check keyboard inputs
    /// </summary>
    void GetKeyboardInput()
    {
        // Jump
        if (Input.GetKeyDown(JumpKey) && (GetIsGrounded() == true || CanDoubleJump || CanWallJump()))
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

        if (Input.GetKeyDown(ColourChangeKey)) {
            ShiftColour();
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
                if(GetWallJumpPermitted())
                {
                    //ResetVerticalMovement();
                    rb.velocity = Vector2.zero;
                    rb.AddForce(JumpForce + Vector2.right * GetDirection() * -1 * RunSpeed, ForceMode2D.Impulse);
                    HasWallJumped = true;
                    PostWallJumpTimer = 0;
                }
                else if(!GetHasDoubleJumped())
                {
                    ResetVerticalMovement();
                    rb.AddForce(JumpForce, ForceMode2D.Impulse);
                    SetHasDoubleJumped(true);
                }
                else
                    ResetVerticalMovement();
            }
            else
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
        afterImage.GetComponent<SpriteRenderer>().color = GetCurrentSpriteColor();                               // set the after-image's color to red
    }

    void ShiftColour() {
        int colourAsInt = (int)colour;
        colourAsInt++;
        colourAsInt %= 4;
        colour = (Colour)colourAsInt;
    }

    void UpdateColour() {
        GetComponent<SpriteRenderer>().color = GetCurrentSpriteColor();
        gameObject.layer = GetCurrentLayer();
    }

    Color GetCurrentSpriteColor() {
        Color newColor;
        switch (colour) {
            case Colour.Red:
                newColor = Color.red;
                break;
            case Colour.Blue:
                newColor = Color.blue;
                break;
            case Colour.Green:
                newColor = Color.green;
                break;
            case Colour.Yellow:
                newColor = Color.yellow;
                break;
            default:
                newColor = Color.magenta;
                break;
        }
        return newColor;
    }

    int GetCurrentLayer() {
        int layer = 13;
        switch (colour) {
            case Colour.Red:
                layer = 15;
                break;
            case Colour.Blue:
                layer = 14;
                break;
            case Colour.Green:
                layer = 17;
                break;
            case Colour.Yellow:
                layer = 16;
                break;
        }
        return layer;
    }
}