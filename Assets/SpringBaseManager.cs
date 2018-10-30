using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBaseManager : MonoBehaviour {

    private float springForce = 1000f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void generateSpring(Rigidbody other)
    {
        SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = other;
        springJoint.spring = springForce;
    }
}
