using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonAnimation:MonoBehaviour
{
    public abstract void pressAnimation(Button button);
    public abstract void releaseAnimation(Button button);
}