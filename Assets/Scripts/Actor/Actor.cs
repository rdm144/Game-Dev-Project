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
    int Direction;
    public LayerMask mask; // Must be set to "Ground" via inspector
    GameObject cube; // Only used to visualize the ground detection box in-game

    public float GetHealth() { return Health; }
    public int GetDirection() { return Direction; }
    public bool GetIsGrounded() { return IsGrounded; }

    public void SetHealth(float newHP) { Health = newHP; }
    public void SetIsGrounded(bool g) { IsGrounded = g; }
    public void SetDirection(int d) { if (d == -1 || d == 1) Direction = d; }

    private void Awake()
    {
        Direction = 1;
        IsGrounded = false;
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

        // check if ground detection box overlaps with the ground
        if ((Physics2D.OverlapBox(GroundDetectorCenter, GroundDetectorSize, 0f, mask) != null))
            SetIsGrounded(true);
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
}