using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class PlacementController : MonoBehaviour
{
    [Header("Events")]
    public GameEvent onObjectPlaced;
    public GameEvent onSceneCleared;

    [Header("Prefab")]
    [SerializeField]
    private GameObject placedPrefab;

    private bool _placed = false;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager aRPlaneManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        arRaycastManager.SetTrackablesActive(true);
        TogglePlaneActive();
    }

    public bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount == 1)
        {
            touchPosition = Input.GetTouch(0).position;
            if (touchPosition.IsPointOverUIObject())
            {
                return false;
            }
            return true;
        }
        touchPosition = default;
        return false;
    }
    public void ClearScene()
    {
        GameObject[] placedPrefabs = GameObject.FindGameObjectsWithTag("DrawingParent");
        foreach (GameObject item in placedPrefabs)
        {
            GameObject.Destroy(item);
        }
        //Triggering change for clear scene
        _placed = false;
        onSceneCleared.Raise(this, null);
        arRaycastManager.SetTrackablesActive(true);
        TogglePlaneActive();
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (!_placed && arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var x = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);

            //Triggering change for after object is placed
            _placed = true;
            onObjectPlaced.Raise(this, null);
            arRaycastManager.SetTrackablesActive(false);
            TogglePlaneActive();
        }

    }

    private void TogglePlaneActive()
    {
        foreach (var plane in aRPlaneManager.trackables)
        {
            plane.gameObject.SetActive(!_placed);
        }
    }
}
