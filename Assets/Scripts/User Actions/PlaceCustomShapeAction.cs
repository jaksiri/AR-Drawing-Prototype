using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCustomShapeAction : UserAction
{
    private GameObject go;

    public PlaceCustomShapeAction(GameObject obj)
    {
        this.go = obj;
    }

    public override void DoAction()
    {
        base.DoAction();
    }

    public override void UndoAction()
    {
        GameObject.Destroy(go);
    }
}
