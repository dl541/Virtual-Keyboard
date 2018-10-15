using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeCollider : MonoBehaviour
{
    private float colliderDepth = 200f;

    // Use this for initialization
    void Start()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        boxCollider.center = new Vector3(size.x / 2, -size.y / 2, colliderDepth / 2);
        boxCollider.size = new Vector3(size.x, size.y, colliderDepth);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
