using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateKeyboard : MonoBehaviour {
    public GameObject keyboardBase;
    public GameObject buttonPrefab;

	// Use this for initialization
	void Start () {
        GenerateKeys();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateKeys()
    {
        instantiateKey();
    }

    void instantiateKey()
    {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.transform.SetParent(keyboardBase.transform, false);
        newButton.transform.position = new Vector3(50f, 50f, 50f);
        GameObject text = newButton.transform.Find("TextMeshPro Text").gameObject;
        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        textMesh.SetText("W");
    }
}
