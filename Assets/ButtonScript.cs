using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    private Button button;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";

    // Use this for initialization
    void Start () {
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(outputToField);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   
    private void outputToField()
    {
        inputFieldManager.append(gameObject.name);
    }
}
