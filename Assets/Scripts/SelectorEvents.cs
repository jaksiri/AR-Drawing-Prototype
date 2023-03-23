using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorEvents : MonoBehaviour
{
    public GameEvent drawOnOuterSurface;
    public GameEvent drawOnInnerSurface;

    public void OuterSelected()
    {
        drawOnOuterSurface.Raise(this, DrawingState.Outer);
    }
    public void InnerSelected()
    {
        drawOnInnerSurface.Raise(this, DrawingState.Inner);
    }

}