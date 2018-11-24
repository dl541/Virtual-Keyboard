using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BalloonAnimation : ButtonAnimation {

    override public void pressAnimation(GameObject button)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerDownHandler);
        playAudio();
    }

    override public void releaseAnimation(GameObject button)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button, pointer, ExecuteEvents.pointerUpHandler);
    }
}
