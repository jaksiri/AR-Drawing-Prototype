using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceShapeAction : UserAction
{
    private GameObject placedObject;
    private SolidPlacementController placementController;

    public PlaceShapeAction(SolidPlacementController placementController)
    {
        this.placementController = placementController;
    }

    public void DoAction(GameObject go)
    {
        placedObject = go;
        base.DoAction();
    }

    public override void UndoAction()
    {
        placementController.UndoPlaceShape();
        GameObject.Destroy(placedObject);
    }
}
