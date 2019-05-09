using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour {

    private static TMP_InputField inputField;
    private int characterLimit = 30;

	// Use this for initialization
	void Start () {
        inputField = gameObject.GetComponent<TMP_InputField>();
        inputField.characterLimit = characterLimit;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void append(string character)
    {
        if (inputField.characterLimit <= inputField.text.Length)
        {
            inputField.text = inputField.text.Substring(1, inputField.characterLimit-1);
        }
        inputField.text += character;
    }

    public void deleteFirstCharacter()
    {
        if (inputField.text.Length != 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
