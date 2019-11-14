using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingGhost : Actor
{
    GameObject Target; // this will be the player
    Rigidbody2D body;
    public float ForceMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        ForceMagnitude = 20;
        body = GetComponent<Rigidbody2D>();
        Target  = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBecameVisible()
    {
        Launch();
    }

    void OnBecameInvisible()
    {
        body.velocity = Vector3.zero;
        Camera cam = Camera.main;
        Vector3 TeleportPosition = new Vector3(Target.transform.position.x + 20, Target.transform.position.y + 5, 0);
        /*Vector3 ScreenPoint = new Vector3(Screen.width, Screen.height, cam.nearClipPlane);
        // Get edge of the camera in world coordinates
        TeleportPosition = cam.ScreenToWorldPoint(ScreenPoint);
        TeleportPosition = new Vector3( TeleportPosition.x, TeleportPosition.y, 0);
        */
        transform.position = TeleportPosition;
        

    }

    void Launch()
    {
        Vector3 TargetDirection = Target.transform.position - transform.position; // Get direction from self to location
        TargetDirection.Normalize(); // Normalize the directional vector
        body.AddForce(TargetDirection * ForceMagnitude, ForceMode2D.Impulse); // Apply force where the magnitude points towards the target direction
    }
}
