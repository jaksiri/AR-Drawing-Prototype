using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorEvents : MonoBehaviour
{
    public GameEvent updateDrawState;

    public void OuterSelected()
    {
        updateDrawState.Raise(this, DrawingState.Outer);
    }
    public void InnerSelected()
    {
        updateDrawState.Raise(this, DrawingState.Inner);
    }

}