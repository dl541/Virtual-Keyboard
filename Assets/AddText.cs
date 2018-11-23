﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddText : MonoBehaviour {

    public GameObject buttonTextPrefab;
    private GameObject buttonText;
    private bool textAttach = true;

	// Use this for initialization
	void Start () {
        buttonText = Instantiate(buttonTextPrefab) as GameObject;
	}
    
    // Text attached after the object is instantiated to avoid text material not found issue
    void attachText()
    {
        buttonText.transform.SetParent(gameObject.transform, false);
        buttonText.transform.position = gameObject.GetComponentInParent<Transform>().position + new Vector3 (0f, 0f, -0.01f);
        buttonText.transform.localRotation = Quaternion.identity;
    }

	// Update is called once per frame
	void Update () {
        if (textAttach)
        {
            attachText();
            textAttach = false;
        }

    }
}
