using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    public enum Colour { Green, Blue, Red, Yellow }
    Colour colour;
    PlayerWallJump pwj;
    PlayerMovement pm;
    int IgnoreLayer;

    // consts for the Ground layers. NOT the PLAYER layers!
    // Player layers are 13 thru 17.
    const int GROUND_LAYER = 8;
    const int RED_LAYER = 10;
    const int YELLOW_LAYER = 11;
    const int GREEN_LAYER = 12;
    const int BLUE_LAYER = 9;
    int[] masks = new int[] { GROUND_LAYER, BLUE_LAYER, RED_LAYER, YELLOW_LAYER, GREEN_LAYER }; // 8, 9, 10, 11, 12

    // Start is called before the first frame update
    void Start()
    {
        IgnoreLayer = 0;
        colour = Colour.Yellow;
        pm = GetComponent<PlayerMovement>();
        if (GetComponent<PlayerWallJump>() != null)
            pwj = GetComponent<PlayerWallJump>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.ColourKeyDown)
            ShiftColour();

        UpdateColour();
    }

    void ShiftColour()
    {
        int colourAsInt = (int)colour;
        colourAsInt++;
        colourAsInt %= 4;
        colour = (Colour)colourAsInt;
    }

    void UpdateColour()
    {
        GetComponent<SpriteRenderer>().color = GetCurrentSpriteColor();
        gameObject.layer = GetCurrentLayer();

        LayerMask finalMask = 0; // Bit-wise OR all masks except the current ground color layer
        for(int i = 0; i < masks.Length; i++)
        {
            if (IgnoreLayer != masks[i])
            {
                LayerMask tempLayerMask = 1 << masks[i]; // magic bit-shifting stuff idk it just works lol
                finalMask = finalMask | tempLayerMask; // bit-wise or
            }
        }

        if (pwj != null)
            pwj.wallMask = finalMask; // Change wall jump mask
        pm.groundMask = finalMask; // Change ground mask
    }

    Color GetCurrentSpriteColor()
    {
        Color newColor;
        switch (colour)
        {
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

    int GetCurrentLayer()
    {
        int layer = 13; // Default Player Layer
        switch (colour)
        {
            case Colour.Red:
                layer = 15; // Red Player
                IgnoreLayer = RED_LAYER;
                break;
            case Colour.Blue:
                layer = 14; // Blue Player
                IgnoreLayer = BLUE_LAYER;
                break;
            case Colour.Green:
                layer = 17; // Green Player
                IgnoreLayer = GREEN_LAYER;
                break;
            case Colour.Yellow:
                layer = 16; // Yellow Player
                IgnoreLayer = YELLOW_LAYER;
                break;
        }
        return layer;
    }
}
