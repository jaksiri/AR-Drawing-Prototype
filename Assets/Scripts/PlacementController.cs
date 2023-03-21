using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{

    [SerializeField]
    private GameObject placedPrefab;

    private bool _placed = false;

    public bool placed
    {
        get
        {
            return _placed;
        }

        set
        {
            _placed = value;
        }
    }

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
            if (touchPosition.IsPointOverUIObject()) {
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
        placed = false;
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
            placed = true;
        }

    }
}
