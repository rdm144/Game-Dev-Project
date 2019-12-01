
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    PlayerMovement pm;
    PlayerWallJump pwj;
    Rigidbody2D rb;
    public bool DashInput;
    public bool JumpInput;
    public bool IsAirDashing, IsGroundDashing;
    public int AirDashCounter;
    public const int MaxAirDashFrames = 30;
    Shader shaderGUItext;
    KeyCode DashKey, JumpKey;

    public int GetMaxDashTime() { return MaxAirDashFrames; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        if (GetComponent<PlayerWallJump>() != null)
            pwj = GetComponent<PlayerWallJump>();
        AirDashCounter = 0;
        IsAirDashing = false;
        IsGroundDashing = false;
        shaderGUItext = Shader.Find("GUI/Text Shader"); // Shader for after-images when dashing
        DashKey = InputHandler.dash_key;
        JumpKey = InputHandler.jump_key;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
    }

    void GetInput()
    {
        DashInput = Input.GetKey(DashKey); // Dash input
        if (Input.GetKey(JumpKey)) // Get jump button input
            JumpInput = true;
    }

    void HorizontalMovement()
    {
        if (pm.IsGrounded || pwj.OnWall) AirDashCounter = 0; // Reset airdash counter when on the ground or on a wall
        if (DashInput)
        {
            pm.runHorizontalMovement = false;
            bool CanAirDash = ((AirDashCounter < MaxAirDashFrames) ? true : false); // Check if we are allowed to airdash
            if (!pm.IsGrounded && CanAirDash && !IsAirDashing && !IsGroundDashing && !JumpInput) // Begin air dash state
            {
                Accelerate(true);
                IsAirDashing = true;
                AirDashCounter++;
            }
            else if (IsAirDashing && CanAirDash && !pm.IsGrounded && Mathf.Abs(rb.velocity.x) > 0.5f && !JumpInput) // Continue air dash
            {
                Accelerate(true);
                AirDashCounter++;
            }
            else
            {
                Accelerate(false);
                IsGroundDashing = true;
                IsAirDashing = false;
            }
            CreateAfterImage();
        }
        else
        {
            IsGroundDashing = false;
            IsAirDashing = false;

            bool isSlashing = false;
            if (!isSlashing)
                pm.runHorizontalMovement = true;
        }
        JumpInput = false;
    }

    void Accelerate(bool FreezeY)
    {
        bool isSlashing = false;
        if (!isSlashing || !pm.IsGrounded)
        {
            Vector2 targetVelocity = pm.DashSpeed * pm.Direction * Vector2.right;
            float Accel_Multiplier = 1;
            if (IsAirDashing || pm.IsGrounded)
                Accel_Multiplier = 3;
            Vector2 force = new Vector2(targetVelocity.x - rb.velocity.x, 0) * pm.AccelerationRate * Accel_Multiplier;
            rb.AddForce(force, ForceMode2D.Force);
            /*
            if (Mathf.Abs(rb.velocity.x) < pm.DashSpeed) // check if our movement is a maximum
                rb.AddForce(Vector3.right * pm.AccelerationRate * pm.Direction, ForceMode2D.Impulse); // Accelerate horizontally
            else
                rb.velocity = new Vector2(pm.DashSpeed * pm.Direction, rb.velocity.y); // cap horizontal speed
            */
        }

        if(FreezeY)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
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
        afterImage.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;                               // set the after-image's color to red
        afterImage.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
        afterImage.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }
}