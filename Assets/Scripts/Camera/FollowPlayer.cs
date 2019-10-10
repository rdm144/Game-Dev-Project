using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    GameObject Player;
    Vector3 CameraOffset;
    Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        CameraOffset = new Vector3(0, 0, -10);
    }

    void LateUpdate() {
        SmoothLead();
    }

    void SimpleFollow() {
        transform.position = Player.transform.position + CameraOffset;
    }

    void SmoothFollow() {
        transform.position = Vector3.SmoothDamp(transform.position, Player.transform.position + CameraOffset, ref velocity, .3f);
    }

    void SmoothLead() {
        float direction = Mathf.Clamp(Player.GetComponent<Player>().GetVelocity().x, -1, 1);
        Vector3 target = Player.transform.position + CameraOffset + direction * Vector3.right * 8;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, .3f);
    }
}