using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour {

    private float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateSpeed();
        gameObject.transform.position += new Vector3(0f, 0f, speed);
    }
    // Update the speed of the cube
    void UpdateSpeed()
    {
        if (gameObject.transform.position.z < -15.0f) speed = 0.1f;
        else if (gameObject.transform.position.z > 15.0f) speed = -0.1f;
    }
}
