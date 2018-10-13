using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On Trigger Enter is pressed");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("On Trigger Stay");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
    }
}
