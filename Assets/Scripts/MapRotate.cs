using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotate : MonoBehaviour
{
    int currentAngle;
    Vector2 baseGravity;
    float gravityStrength;

    enum GravityDirection { Down, Left, Up, Right}
    GravityDirection currentDirection;
    // Start is called before the first frame update
    void Start()
    {
        currentAngle = 0;
        baseGravity = Physics2D.gravity;
        gravityStrength = 10;
        currentDirection = GravityDirection.Down;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, currentAngle), 0.2f);
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (currentDirection == GravityDirection.Right)
                currentDirection = GravityDirection.Down;
            else
                currentDirection++;
            RotateGravity(currentDirection);
        }
    }

    void RotateGravity(GravityDirection direction) {
        switch(direction) {
            case GravityDirection.Down:
                Physics2D.gravity = Vector2.down * gravityStrength;
                break;
            case GravityDirection.Left:
                Physics2D.gravity = Vector2.left * gravityStrength;
                break;
            case GravityDirection.Up:
                Physics2D.gravity = Vector2.up * gravityStrength;
                break;
            case GravityDirection.Right:
                Physics2D.gravity = Vector2.right * gravityStrength;
                break;
        }
    }
}
