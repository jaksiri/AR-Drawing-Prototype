using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkShapePointsAction : UserAction
{
    private CreatingShapesState creatingShapesState;
    private Transform pointMarked;
    public MarkShapePointsAction(Transform pointMarked, CreatingShapesState state)
    {
        this.creatingShapesState = state;
        this.pointMarked = pointMarked;
    }
    public override void DoAction()
    {
        base.DoAction();
    }
    public override void UndoAction()
    {
        switch (creatingShapesState)
        {
            case CreatingShapesState.PlacingStart:
                break;
            case CreatingShapesState.PlacingEnd:
                break;
            case CreatingShapesState.PlacingHeight:
                break;
        }
    }
}
