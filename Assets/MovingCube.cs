﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{

    private float speed = 3f;
    private float pushPower = 20f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateSpeed();
        //gameObject.GetComponent<CharacterController>().Move(new Vector3(0f, 0f, speed));
        gameObject.transform.position += new Vector3(0f, 0f, speed);
    }
    // Update the speed of the cubes
    void updateSpeed()
    {
        if (gameObject.transform.position.z < -15.0f) speed = 3f;
        else if (gameObject.transform.position.z > 15.0f) speed = -3f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody otherBody = hit.collider.attachedRigidbody;

        if (otherBody.isKinematic == false)
        {
            Vector3 force = hit.controller.velocity * pushPower;
            otherBody.AddForceAtPosition(force, hit.point);
        }
    }

}
