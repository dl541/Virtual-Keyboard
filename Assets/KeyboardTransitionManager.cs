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
        if (Input.anyKeyDown)
        {
            animator.SetInteger("State", (animator.GetInteger("State") + 1) % 3);
        }
	}
}
