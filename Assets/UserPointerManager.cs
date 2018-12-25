using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPointerManager : MonoBehaviour {

    private double mass = 100;
    private Vector3 velocity = Vector3.zero;
    private Vector3 prevPosition;
	// Use this for initialization
	void Start () {
        prevPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
