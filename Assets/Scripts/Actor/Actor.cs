using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General class for NPCs, enemies, players, etc.
/// </summary>
public class Actor : MonoBehaviour
{
    float Health; // Must be set by the Actor's child
    public bool IsGrounded;
    public bool HasDoubleJumped;
    //public bool DoubleJumpPermitted;
    public bool WallJumpPermitted;
    
    int Direction; // -1: left, 1: right
    LayerMask mask; // Must be set to "Ground" via inspector
    GameObject cube; // Only used to visualize the ground detection box in-game

    public float GetHealth() { return Health; }
    public int GetDirection() { return Direction; }
    public bool GetIsGrounded() { return IsGrounded; }
    public bool GetHasDoubleJumped() { return HasDoubleJumped; }
    //public bool GetDoubleJumpPermitted() { return DoubleJumpPermitted; }
    public bool GetWallJumpPermitted() { return WallJumpPermitted; }


    public void SetHealth(float newHP) { Health = newHP; }
    public void SetIsGrounded(bool g) { IsGrounded = g; }
    public void SetHasDoubleJumped(bool j) { HasDoubleJumped = j; }
    public void SetDirection(int d) { if (d == -1 || d == 1) Direction = d; }
    //public void SetDoubleJumpPermitted(bool j) { DoubleJumpPermitted = j; }
    public void SetWallJumpPermitted(bool j) { WallJumpPermitted = j; }

    private void Awake()
    {
        Direction = 1;
        IsGrounded = false;
        HasDoubleJumped = false;
        //DoubleJumpPermitted = false;
        //WallJumpPermitted = false;
    }

    /// <summary>
    /// Creates a thin box below the actor that looks for the floor's collider.
    /// </summary>
    public void CheckIfOnGround()
    {
        // Create Ground Detection box at the bottom of the actor, with a width of 0.05
        Vector2 GroundDetectorSize = new Vector2(GetComponent<BoxCollider2D>().size.x - 0.02f, 0.05f);
        Vector2 GroundDetectorCenter = (Vector2)transform.position +
            (Vector2.right * GetDirection() * GetComponent<BoxCollider2D>().offset.x * transform.localScale.x) +
            (Vector2.down * 0.5f* (GetComponent<BoxCollider2D>().size.y - GetComponent<BoxCollider2D>().offset.y + GroundDetectorSize.y) * transform.localScale.y);

        /*///////// Visualization for Ground Detection Box ///////////////
        Destroy(cube);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = GroundDetectorCenter;
        cube.transform.localScale = GroundDetectorSize;
        */////////////////////////////////////////////////////////////////

        UpdateMask();
        // check if ground detection box overlaps with the ground
        if ((Physics2D.OverlapBox(GroundDetectorCenter, GroundDetectorSize, 0f, mask) != null))
            OnGrounded();
        else
            SetIsGrounded(false);
    }

    /// <summary>
    /// Used for any damage calculations. Actor's children may override if needed.
    /// </summary>
    /// <param name="damage">Amount of damage to subtract from health value</param>
    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
    }

    /// <summary>
    /// Used once the actor is on the ground.
    /// Resets "IsGrounded" and "HasDoubleJumped" to their default values.
    /// </summary>
    public void OnGrounded() {
        SetIsGrounded(true);
        SetHasDoubleJumped(false);
    }

    public bool CanWallJump() {
        if (IsGrounded)
            return false;
        Vector2 WallDetectorSize = new Vector2(0.05f, GetComponent<BoxCollider2D>().size.y - 0.02f);
        Vector2 WallDetectorCenter = (Vector2)transform.position +
            (Vector2.right * GetDirection() * GetComponent<BoxCollider2D>().offset.x * transform.localScale.x) +
            (Vector2.right * GetDirection() * .5f * (GetComponent<BoxCollider2D>().size.x - GetComponent<BoxCollider2D>().offset.x + WallDetectorSize.x) * transform.localScale.x);

        /*///////// Visualization for wall Detection Box ///////////////
        Destroy(cube);
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = WallDetectorCenter;
        cube.transform.localScale = WallDetectorSize;
        */////////////////////////////////////////////////////////////////
        UpdateMask();
        return Physics2D.OverlapBox(WallDetectorCenter, WallDetectorSize, 0f, mask) != null;
        // return if is touching wall and is moving into said wall...

    }

    /// <summary>
    /// Returns if the actor is allowed to double jump in the moment.
    /// </summary>
    public bool CanDoubleJump {
        get { return !HasDoubleJumped /*&& DoubleJumpPermitted*/; }
    }

    void UpdateMask() {
        mask = 0x1F << 8;
        if (gameObject.layer == 13) return;
        mask ^= 1 << (gameObject.layer - 5);
    }
}