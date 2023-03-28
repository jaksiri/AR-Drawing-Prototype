using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceShapeSelectorEvents : MonoBehaviour
{
    public GameEvent updatePlacementState;

    public void AnchorSelected()
    {
        updatePlacementState.Raise(this, PlacementState.Anchor);
    }
    public void HoverSelected()
    {
        updatePlacementState.Raise(this, PlacementState.Hover);
    }

}