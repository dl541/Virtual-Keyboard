using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour {

    private static InputField inputField;

	// Use this for initialization
	void Start () {
        inputField = gameObject.GetComponent<InputField>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void append(string character)
    {
        inputField.text += character;
    }
}
