using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpringAnimation : MonoBehaviour{

    public static float maxDepth = 80f;
    private int maxFrameIndex = 2;
    private int frameIndex = 0;
    private InputFieldManager inputFieldManager;
    private string inputFieldName = "InputField";

    private void Start()
    {
        inputFieldManager = GameObject.Find(inputFieldName).GetComponent<InputFieldManager>();
    }

    public void pressAnimation()
    {
        //Render this button first, but after the panel
        gameObject.transform.SetSiblingIndex(1);

        //Shading
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerDownHandler);

        if (frameIndex == 0)
        {
            PlayAudio();   
        }

        if (frameIndex >= maxFrameIndex)
        {
            gameObject.GetComponent<InitializeCollider>().buttonState = ButtonState.PRESSED;
            frameIndex = 0;
        }
        else
        {
            //Move keyboard button
            gameObject.transform.localPosition += new Vector3(0f, 0f, maxDepth/maxFrameIndex);
            frameIndex += 1;
        }

    }

    public void PlayAudio()
    {
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            gameObject.GetComponent<AudioSource>().Play();
            Debug.Log("Play typing sound.");
        }
    }

    public void releaseAnimation()
    {
        if (frameIndex == 0)
        {
            gameObject.transform.localPosition -= new Vector3(0f, 0f, maxDepth);
        }
        else
        {
            gameObject.transform.localPosition -= new Vector3(0f, 0f, maxDepth/maxFrameIndex * frameIndex);
        }

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerExitHandler);

        gameObject.GetComponent<InitializeCollider>().buttonState = ButtonState.RELEASED;
        inputFieldManager.append(gameObject.name);
        frameIndex = 0;

    }
}
