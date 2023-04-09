using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class GuidePlacementController : MonoBehaviour
{
    [Header("Events")]
    public GameEvent updateGameState;

    [Header("Prefab")]
    [SerializeField]
    private GameObject placedPrefab;

    // Parenting
    private bool _placedParent = false;
    private GameObject parent;

    private bool _allowPlace = false;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager aRPlaneManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.PlacingPlane:
                    StartCoroutine(DelayedPlace());
                    aRPlaneManager.SetTrackablesActive(true);

                    if (GameManager.Instance.drawingParent == null)
                    {
                        _placedParent = false;
                        parent = null;
                    }
                    else
                    {
                        _placedParent = true;
                        parent = GameManager.Instance.drawingParent;
                    }
                    break;
                default:
                    _allowPlace = false;
                    break;
            }
        }
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

    public void UndoPlaceObject()
    {
        if (GameManager.Instance.currentGameState != GameState.PlacingPlane)
        {
            updateGameState.Raise(this, GameState.PlacingPlane);
        }
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        if (!_allowPlace)
        {
            return;
        }

        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var placedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            var placeAction = new PlaceObjectAction(this);
            placeAction.DoAction(placedObject);

            //Checking for existing parent
            if (!_placedParent)
            {
                parent = Instantiate(new GameObject(), hitPose.position, hitPose.rotation);
                // parent.AddComponent<TwoFingerRotate>();
                GameManager.Instance.drawingParent = parent;
                _placedParent = true;
            }
            placedObject.transform.SetParent(parent.transform);

            //Delaying placement of multiple objects
            _allowPlace = false;
            StartCoroutine(DelayedPlace());
        }

    }

    private IEnumerator DelayedPlace()
    {
        yield return new WaitForSeconds(0.4f);
        _allowPlace = true;
    }
}

//condition for adding newly placed object to the parent if the parent already exists
