using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSelectorDropdown : MonoBehaviour
{
    public GameEvent changeCurrentShape;
    public GameEvent changeCurrentShapeDraw;
    private Vector3 initialPosition;

    private void Start()
    {
        changeCurrentShape.Raise(this, CurrentShape.Cube);
        changeCurrentShapeDraw.Raise(this, CurrentShapeDraw.Cube);
    }
    private void OnEnable()
    {
        initialPosition = gameObject.transform.localPosition;
        LeanTween.moveLocal(gameObject, new Vector3(0, -2778 / 2, 0), 0.25f);
    }

    private void OnDisable()
    {
        LeanTween.moveLocal(gameObject, initialPosition, 0.5f);
    }

    public void ChooseCube()
    {
        changeCurrentShape.Raise(this, CurrentShape.Cube);
        changeCurrentShapeDraw.Raise(this, CurrentShapeDraw.Cube);

    }

    public void ChooseRectangle()
    {
        changeCurrentShape.Raise(this, CurrentShape.Rectangle);
    }

    public void ChooseCylinder()
    {
        changeCurrentShape.Raise(this, CurrentShape.Cylinder);
        changeCurrentShapeDraw.Raise(this, CurrentShapeDraw.Cylinder);
    }

    public void ChoosePyramid()
    {
        changeCurrentShape.Raise(this, CurrentShape.Pyramid);
        changeCurrentShapeDraw.Raise(this, CurrentShapeDraw.Pyramid);
    }

    public void ChooseCone()
    {
        changeCurrentShape.Raise(this, CurrentShape.Cone);
        changeCurrentShapeDraw.Raise(this, CurrentShapeDraw.Cone);
    }
}
