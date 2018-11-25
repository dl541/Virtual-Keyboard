using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpringAnimation : ButtonAnimation{

    public static float maxDepth = 20f;
    private static int maxFrameIndex = 2;
    private int frameIndex = 0;
    private static InputFieldManager inputFieldManager;
    private static string inputFieldName = "InputField";

    private void Start()
    {
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
    }

    override public void pressAnimation(GameObject button)
    {
        //Render this button first, but after the panel
        button.transform.SetSiblingIndex(1);

        //Shading
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerDownHandler);

        //Play audio
        //playAudio();

        if (frameIndex >= maxFrameIndex)
        {
            button.GetComponent<InitializeCollider>().buttonState = ButtonState.PRESSED;
            frameIndex = 0;
        }
        else
        {
            //Move keyboard button
            button.transform.position += new Vector3(0f, 0f, maxDepth/maxFrameIndex);
            frameIndex += 1;
        }

    }

    override public void releaseAnimation(GameObject button)
    {
        button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y, 0f);

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerExitHandler);

        if (frameIndex >= maxFrameIndex)
        {
            button.GetComponent<InitializeCollider>().buttonState = ButtonState.RELEASED;
            inputFieldManager.append(button.name);
            frameIndex = 0;
        }
        else
        {
            //Move keyboard button
            button.transform.position += new Vector3(0f, 0f, maxDepth / maxFrameIndex);
            frameIndex += 1;
        }

    }
}
