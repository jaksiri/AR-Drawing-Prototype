using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObjectManager : MonoBehaviour
{
    [Header("Object References")]

    [SerializeField]
    private GameObject outerCube;
    [SerializeField]
    private GameObject xPlaneFront, yPlaneFront, zPlaneFront, xPlaneBack, yPlaneBack, zPlaneBack;

    [Header("Material References")]

    [SerializeField]
    private Material transparentMaterial;
    [SerializeField]
    private Material outerMaterial, innerMaterial;


    private int layerIgnoreRaycast;
    private int defaultLayer = 0;

    private void Awake()
    {
        layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Start()
    {
        switch (GameManager.Instance.currentDrawingState)
        {
            case DrawingState.Inner:
                DrawOnInnerSurface();
                break;
            case DrawingState.Outer:
                DrawOnOuterSurface();
                break;
        }
    }

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is DrawingState)
        {
            switch (data)
            {
                case DrawingState.Inner:
                    DrawOnInnerSurface();
                    break;
                case DrawingState.Outer:
                    DrawOnOuterSurface();
                    break;
            }
        }
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.Drawing:
                    gameObject.layer = defaultLayer;
                    break;
                default:
                    gameObject.layer = layerIgnoreRaycast;
                    break;
            }
        }
    }

    public void OnShowPlanesChanged(Component comp, object data)
    {
        if (data is bool)
        {
            xPlaneFront.SetActive((bool)data);
            yPlaneFront.SetActive((bool)data);
            zPlaneFront.SetActive((bool)data);

            xPlaneBack.SetActive((bool)data);
            yPlaneBack.SetActive((bool)data);
            zPlaneBack.SetActive((bool)data);

            outerCube.SetActive((bool)data);
        }
    }

    public void DrawOnOuterSurface()
    {
        // Layer Modification
        xPlaneFront.layer = layerIgnoreRaycast;
        yPlaneFront.layer = layerIgnoreRaycast;
        zPlaneFront.layer = layerIgnoreRaycast;

        xPlaneBack.layer = layerIgnoreRaycast;
        yPlaneBack.layer = layerIgnoreRaycast;
        zPlaneBack.layer = layerIgnoreRaycast;

        outerCube.layer = defaultLayer;

        // Color Modification
        xPlaneFront.GetComponent<Renderer>().material = transparentMaterial;
        yPlaneFront.GetComponent<Renderer>().material = transparentMaterial;
        zPlaneFront.GetComponent<Renderer>().material = transparentMaterial;

        xPlaneBack.GetComponent<Renderer>().material = transparentMaterial;
        yPlaneBack.GetComponent<Renderer>().material = transparentMaterial;
        zPlaneBack.GetComponent<Renderer>().material = transparentMaterial;

        outerCube.GetComponent<Renderer>().material = outerMaterial;

    }

    public void DrawOnInnerSurface()
    {
        // Layer Modification
        xPlaneFront.layer = defaultLayer;
        yPlaneFront.layer = defaultLayer;
        zPlaneFront.layer = defaultLayer;

        xPlaneBack.layer = defaultLayer;
        yPlaneBack.layer = defaultLayer;
        zPlaneBack.layer = defaultLayer;

        outerCube.layer = layerIgnoreRaycast;

        // Color Modification
        xPlaneFront.GetComponent<Renderer>().material = innerMaterial;
        yPlaneFront.GetComponent<Renderer>().material = innerMaterial;
        zPlaneFront.GetComponent<Renderer>().material = innerMaterial;

        xPlaneBack.GetComponent<Renderer>().material = innerMaterial;
        yPlaneBack.GetComponent<Renderer>().material = innerMaterial;
        zPlaneBack.GetComponent<Renderer>().material = innerMaterial;

        outerCube.GetComponent<Renderer>().material = transparentMaterial;
    }
}

//Implement rotating,scaling