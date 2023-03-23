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
    private string currentDrawMode;

    private void Awake()
    {
        placementController = GetComponent<PlacementController>();
        goList = new List<GameObject>();
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

    public void ObjectPlaced()
    {
        drawingParent = GameObject.FindGameObjectWithTag("DrawingParent");
        StartCoroutine(DrawingParentPlacedDelayed());
    }

    public void SceneCleared()
    {
        _drawingParentPlaced = false;
        drawingParent = null;
    }

    IEnumerator DrawingParentPlacedDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        _drawingParentPlaced = true;
    }
}
