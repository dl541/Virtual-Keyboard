using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonAnimation:MonoBehaviour
{
    public abstract void pressAnimation(GameObject button);
    public abstract void releaseAnimation(GameObject button);

    public void playAudio()
    {
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            gameObject.GetComponent<AudioSource>().Play();
            Debug.Log("Play typing sound.");
        }
    }
}