using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        // If the player touches this object, load the next scene
        if (collision.tag == "Player")
        {
            Application.Quit();
        }
    }
}
