using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSpringCollider : MonoBehaviour {

    private static string SpringBaseName = "SpringBase";
    private static SpringBaseManager springBaseManager;
    private Rigidbody keyRigidBody;

	// Use this for initialization
	void Start () {
        if (springBaseManager == null)
        {
            springBaseManager = GameObject.Find(SpringBaseName).GetComponent<SpringBaseManager>();
        }
        generateCollider();
        generateSpring();
        keyRigidBody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.z < 0)
        {
            Debug.Log("Below 0");
            keyRigidBody.isKinematic = true;
            keyRigidBody.position = new Vector3(keyRigidBody.position.x, keyRigidBody.position.y, 0f);
            keyRigidBody.isKinematic = false;
        }
	}

    private void generateCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        boxCollider.center = new Vector3(size.x / 2, -size.y / 2, boxCollider.center.z);
        boxCollider.size = new Vector3(size.x, size.y, boxCollider.size.z*5);
        boxCollider.isTrigger = false;
    }

    private void generateSpring()
    {
        springBaseManager.generateSpring(gameObject.GetComponent<Rigidbody>());
    }

}
