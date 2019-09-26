using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    GameObject Player;
    Vector3 CameraOffset;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        CameraOffset = new Vector3(0, 0, -10);
    }

    void LateUpdate()
    {
        transform.position = Player.transform.position + CameraOffset;
    }
}