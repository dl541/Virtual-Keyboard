using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState {RELEASED, PRESSING, PRESSED, RELEASING};
public enum AnimationType {SPRING, BALLOON}

public class InitializeCollider : MonoBehaviour
{
    private float colliderDepth = 10000f;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";
    public ButtonState buttonState = ButtonState.RELEASED;
    private SpringAnimation animationScript;
    public AnimationType animationType;

    // Use this for initialization
    void Start()
    {
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
        GenerateCollider();
        SetAnimationScript();
    }

    // Update is called once per frame
    void Update()
    {
        switch (buttonState)
        {
            case (ButtonState.PRESSING):
                Debug.Log(string.Format("{0} is pressed", gameObject.name));
                animationScript.pressAnimation();
                break;

            case (ButtonState.RELEASING):
                Debug.Log(string.Format("{0} released", gameObject.name));
                animationScript.releaseAnimation();
                break;

            default:
                break;
        }
    }

    public void PressButton()
    {
        if (buttonState == ButtonState.RELEASED || buttonState == ButtonState.RELEASING)
        {
            buttonState = ButtonState.PRESSING;
        }
    }

    public void ReleaseButton()
    {
        if (buttonState == ButtonState.PRESSED || buttonState == ButtonState.PRESSING)
        {
            buttonState = ButtonState.RELEASING;
        }
    }

    private void GenerateCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        boxCollider.center = new Vector3(size.x / 2, -size.y / 2, 0f);
        boxCollider.size = new Vector3(size.x, size.y, colliderDepth);
        boxCollider.isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        ReleaseButton();
    }

    void SetAnimationScript()
    {
        switch (animationType)
        {
            case (AnimationType.BALLOON):
                break;

            case (AnimationType.SPRING):
                animationScript = gameObject.AddComponent<SpringAnimation>();
                break;
            default:
                break;
        }
    }
}
