using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitGhost : Actor
{
    Vector2 lowJumpForce, highJumpForce;
    Rigidbody2D rb;
    int jumpCounter;
    int bigJumpCount;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        lowJumpForce = Vector2.up * 10;
        highJumpForce = Vector2.up * 12;
        jumpCounter = 0;
        bigJumpCount = 3;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfOnGround();
        rb.velocity = rb.velocity * Vector2.up;
        if (IsGrounded) {
            if (++jumpCounter % bigJumpCount == 0)
                rb.AddForce(highJumpForce, ForceMode2D.Impulse);
            else
                rb.AddForce(lowJumpForce, ForceMode2D.Impulse);
        }
    }
}
