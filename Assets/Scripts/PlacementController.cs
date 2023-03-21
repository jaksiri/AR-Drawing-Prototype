using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{
    public static event Action OnObjectPlaced;
    public static event Action OnClearedScene;

    [SerializeField]
    private GameObject placedPrefab;

    private bool _placed = false;

    public GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }

    private ARRaycastManager arRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    public bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
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
        GameObject[] placedPrefabs = GameObject.FindGameObjectsWithTag("DrawnObject");
        foreach (GameObject item in placedPrefabs)
        {
            GameObject.Destroy(item);
        }
        //Triggering change for clear scene
        _placed = false;
        OnClearedScene?.Invoke();
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
            OnObjectPlaced?.Invoke();
        }

    }
}
