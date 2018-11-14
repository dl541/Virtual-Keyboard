using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpringAnimation : ButtonAnimation{

    public static float maxDepth = 20f;
    private static int maxFrameIndex = 1;
    private int frameIndex = 0;

    override public void pressAnimation(Button button)
    {
        //Render this button first, but after the panel
        button.gameObject.transform.SetSiblingIndex(1);

        //Shading
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);

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
            button.gameObject.transform.position += new Vector3(0f, 0f, maxDepth/maxFrameIndex);
            frameIndex += 1;
        }

    }

    override public void releaseAnimation(Button button)
    {
        button.gameObject.transform.position = new Vector3(button.gameObject.transform.position.x, button.gameObject.transform.position.y, 0f);

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);

        button.GetComponent<InitializeCollider>().buttonState = ButtonState.RELEASED;

    }
}
