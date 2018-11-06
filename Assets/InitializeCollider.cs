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
    private Button button;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";
    public ButtonState buttonState = ButtonState.RELEASED;
    private ButtonAnimation animationScript;
    public AnimationType animationType;

    // Use this for initialization
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
        generateCollider();
        setAnimationScript();
    }

    // Update is called once per frame
    void Update()
    {

        switch (buttonState)
        {
            case (ButtonState.PRESSING):
                buttonState = ButtonState.PRESSED;
                inputFieldManager.append(gameObject.name);
                Debug.Log(string.Format("{0} is pressed", gameObject.name));
                animationScript.pressAnimation(button);
                break;

            case (ButtonState.RELEASING):
                buttonState = ButtonState.RELEASED;
                Debug.Log(string.Format("{0} released", gameObject.name));
                animationScript.releaseAnimation(button);
                break;

            default:
                break;
        }
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
        buttonState = ButtonState.PRESSING;
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log(string.Format("{0} is being pressed", gameObject.name));

    }

    void OnTriggerExit(Collider other)
    {
        buttonState = ButtonState.RELEASING;
    }

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
