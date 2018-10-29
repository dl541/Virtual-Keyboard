using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTransitionManager : MonoBehaviour {
 
    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("left shift"))
        {
            animator.SetInteger("Shift", (animator.GetInteger("Shift") + 1) % 3);
        }
        else if (Input.GetKeyDown("left ctrl"))
        {
            animator.SetInteger("Symbol", (animator.GetInteger("Symbol") + 1) % 2);
        }
	}
}
