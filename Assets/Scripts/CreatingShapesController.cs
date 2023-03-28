using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class CreatingShapesController : MonoBehaviour
{
    private bool _activated = false;
    private Vector2 midScreen;
    private ARRaycastManager aRRaycastManager;
    public GameObject placementIndicatorMarker;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Marker Points
    private Transform startPoint;
    private Transform endPoint;
    private Transform heightPoint;
    private CreatingShapesState creatingShapesState = CreatingShapesState.PlacingStart;

    [Header("Prefabs to be placed")]
    public GameObject prefabToBePlaced;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        placementIndicatorMarker.SetActive(false);
        midScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_activated)
        {
            return;
        }
        aRRaycastManager.Raycast(midScreen, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        if (hits.Count > 0)
        {
            placementIndicatorMarker.transform.position = hits[0].pose.position;
            placementIndicatorMarker.transform.rotation = hits[0].pose.rotation;
        }
    }

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.PlacingShape:
                    _activated = true;
                    placementIndicatorMarker.SetActive(true);
                    break;
                default:
                    _activated = false;
                    placementIndicatorMarker.SetActive(false);
                    break;
            }
        }
    }

    public void OnPressedMarked()
    {
        if (creatingShapesState == CreatingShapesState.PlacingStart)
        {
            aRRaycastManager.Raycast(midScreen, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
            if (hits.Count > 0)
            {
                startPoint.position = hits[0].pose.position;
                startPoint.rotation = hits[0].pose.rotation;
                creatingShapesState = CreatingShapesState.PlacingEnd;
                var markShapePointsAction = new MarkShapePointsAction(startPoint, creatingShapesState);
            }
        }
        else if (creatingShapesState == CreatingShapesState.PlacingEnd)
        {
            aRRaycastManager.Raycast(midScreen, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
            if (hits.Count > 0)
            {
                endPoint.position = hits[0].pose.position;
                endPoint.rotation = hits[0].pose.rotation;
                creatingShapesState = CreatingShapesState.PlacingHeight;
                var markShapePointsAction = new MarkShapePointsAction(endPoint, creatingShapesState);
            }
        }
        else
        {
            //Set heightPoint
            //Instantiate gameobject between the startPoint, endPoint, and heightPoint
        }
    }

    private void DrawShapePreview(Transform startPoint, Transform endPoint, Transform heightPoint)
    {

    }
}

public enum CreatingShapesState
{
    PlacingStart, PlacingEnd, PlacingHeight
}
