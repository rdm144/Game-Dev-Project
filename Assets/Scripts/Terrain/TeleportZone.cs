using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Upon collision, teleports the player to this object's anchor position.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class TeleportZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.transform.position = transform.position;
    }
}