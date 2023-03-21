using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RaycastDraw : MonoBehaviour
{
    private List<GameObject> goList;

    [SerializeField]
    private GameObject drawnPrefab;
    private PlacementController placementController;
    private GameObject drawingParent;
    private bool _drawingParentPlaced;
    private RaycastHit hit;
    // private bool _prevDrawing = false;
    // private int numClicks = 0;
    // private float _lineWidth = 0.05f;

    private void Awake()
    {
        placementController = GetComponent<PlacementController>();
        goList = new List<GameObject>();
        PlacementController.OnObjectPlaced += ObjectPlaced;
        PlacementController.OnClearedScene += SceneCleared;
    }
    private void OnDestroy()
    {
        PlacementController.OnObjectPlaced -= ObjectPlaced;
        PlacementController.OnClearedScene -= SceneCleared;
    }

    void Update()
    {
        if (!placementController.TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(touchPosition);

        if (_drawingParentPlaced && Physics.Raycast(ray, out hit))
        {
            GameObject go = Instantiate(drawnPrefab);
            go.transform.position = hit.point;
            go.tag = "DrawnObject";
            goList.Add(go);
            go.transform.SetParent(drawingParent.transform, true);
        }
    }

    public void Undo()
    {
        var count = goList.Count;
        if (count > 0)
        {
            GameObject.Destroy(goList[count - 1]);
            goList.RemoveAt(count - 1);
        }
    }

    private void ObjectPlaced()
    {
        _drawingParentPlaced = true;
        drawingParent = GameObject.FindGameObjectWithTag("DrawingParent");
    }

    private void SceneCleared()
    {
        _drawingParentPlaced = false;
        drawingParent = null;
    }
}
