using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InitializeCollider : MonoBehaviour
{
    private float colliderDepth = 200f;
    private Button button;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";

    // Use this for initialization
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
        generateCollider();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void generateCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        boxCollider.center = new Vector3(size.x / 2, -size.y / 2, colliderDepth / 2);
        boxCollider.size = new Vector3(size.x, size.y, colliderDepth);
        boxCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);

        inputFieldManager.append(gameObject.name);
        Debug.Log(string.Format("{0} is pressed", gameObject.name));
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log(string.Format("{0} is being pressed", gameObject.name));

    }

    void OnTriggerExit(Collider other)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        Debug.Log(string.Format("{0} released", gameObject.name));
    }

}
