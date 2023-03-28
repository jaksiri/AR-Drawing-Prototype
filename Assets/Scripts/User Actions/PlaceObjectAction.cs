using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjectAction : UserAction
{
    private GameObject placedObject;
    private GuidePlacementController placementController;

    public PlaceObjectAction(GuidePlacementController placementController)
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
        placementController.UndoPlaceObject();
        GameObject.Destroy(placedObject);
    }
}
