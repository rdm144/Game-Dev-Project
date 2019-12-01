using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler{
    // Keyboard input.
    public static KeyCode left_key = KeyCode.A;
    public static KeyCode right_key = KeyCode.D;
    public static KeyCode jump_key = KeyCode.Space;
    public static KeyCode dash_key = KeyCode.LeftShift;
    public static KeyCode colour_key = KeyCode.S;

    // F310 Input
    public static string jump_f310 = "joystick button 0";
    public static string colour_f310 = "joystick button 4";
    public static string dash_f310 = "joystick button 5";

    public static bool JumpKeyDown {
        get {
            return Input.GetKeyDown(jump_key) || Input.GetKeyDown(jump_f310);
        }
    }

    public static bool JumpKey {
        get {
            return Input.GetKey(jump_key) || Input.GetKey(jump_f310);
        }
    }

    public static bool LeftKey {
        get {
            return Input.GetKey(left_key) || Input.GetAxis("Horizontal") < -0.05f;
        }
    }

    public static bool RightKey {
        get {
            return Input.GetKey(right_key) || Input.GetAxis("Horizontal") > 0.05f;
        }
    }

    public static bool DashKey {
        get {
            return Input.GetKey(dash_key) || Input.GetKey(dash_f310);
        }
    }

    public static bool ColourKeyDown {
        get {
            return Input.GetKeyDown(colour_key) || Input.GetKeyDown(colour_f310);
        }
    }

}
