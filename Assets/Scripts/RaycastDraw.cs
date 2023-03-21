using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RaycastDraw : MonoBehaviour
{
    private PlacementController placementController;
    private GameObject drawingParent;
    private RaycastHit hit;
    // private bool _prevDrawing = false;
    // private int numClicks = 0;
    // private float _lineWidth = 0.05f;

    void Start()
    {
        placementController = GetComponent<PlacementController>();
        drawingParent = GameObject.FindGameObjectWithTag("DrawingParent");
    }

    void Update()
    {
        if (!placementController.TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(touchPosition);

        if (placementController.placed && Physics.Raycast(ray, out hit))
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = hit.point;
            go.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            go.tag = "DrawnObject";
            go.transform.SetParent(drawingParent.transform, true);
        }
    }

    // bool TryGetTouchPosition(out Vector2 touchPosition)
    // {
    //     if (Input.touchCount > 0)
    //     {
            
    //         touchPosition = Input.GetTouch(0).position;
    //         return true;
    //     }
    //     touchPosition = default;
    //     return false;
    // }
}
