using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState {RELEASED, PRESSING, PRESSED, RELEASING};
public enum AnimationType {SPRING, BALLOON}

public class InitializeCollider : MonoBehaviour
{
    private float colliderDepth = 200f;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";
    public ButtonState buttonState = ButtonState.RELEASED;
    private ButtonAnimation animationScript;
    public AnimationType animationType;

    // Use this for initialization
    void Start()
    {
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
        //generateCollider();
        setAnimationScript();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (buttonState)
        {
            case (ButtonState.PRESSING):
                Debug.Log(string.Format("{0} is pressed", gameObject.name));
                animationScript.pressAnimation(gameObject);
                break;

            case (ButtonState.RELEASING):
                Debug.Log(string.Format("{0} released", gameObject.name));
                animationScript.releaseAnimation(gameObject);
                break;

            default:
                break;
        }
    }

    //private void generateCollider()
    //{
    //    BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

    //    Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
    //    boxCollider.center = new Vector3(size.x / 2, -size.y / 2, colliderDepth / 2);
    //    boxCollider.size = new Vector3(size.x, size.y, colliderDepth);
    //    boxCollider.isTrigger = true;
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    buttonState = ButtonState.PRESSING;
    //}

    //void OnTriggerStay(Collider other)
    //{
    //    Debug.Log(string.Format("{0} is being pressed", gameObject.name));

    //}

    //void OnTriggerExit(Collider other)
    //{
    //    buttonState = ButtonState.RELEASING;
    //}

    void setAnimationScript()
    {
        switch (animationType)
        {
            case (AnimationType.BALLOON):
                animationScript = gameObject.AddComponent<BalloonAnimation>();
                break;

            case (AnimationType.SPRING):
                animationScript = gameObject.AddComponent<SpringAnimation>();
                break;
            default:
                break;
        }
    }
}
