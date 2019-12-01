using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedAfterImage : MonoBehaviour
{
    SpriteRenderer sr;
    float decriment;

	// Use this for initialization
	void Start ()
    {
        sr = GetComponent<SpriteRenderer>();
        decriment = 1 / 12f;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if(sr.color.a > 0)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - decriment);
        }
        else
        {
            Destroy(gameObject);
        }
	}
}