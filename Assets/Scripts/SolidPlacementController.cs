using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class SolidPlacementController : MonoBehaviour
{
    private bool _allowPlace = false;
    // Parenting
    private bool _placedParent = false;
    private GameObject parent;

    // Referencing Scripts
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager aRPlaneManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private RaycastHit hit;

    // UI Reference
    public TMP_Text currentShapeButtonText;

    [Header("Events")]
    public GameEvent updateGameState;
    [Header("Prefabs")]
    public GameObject currentShapePrefab;
    public GameObject cubePrefab;
    public GameObject rectanglePrefab;
    public GameObject cylinderPrefab;
    public GameObject pyramidPrefab;
    public GameObject conePrefab;
    public GameObject spherePrefab;
    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.PlacingShape:
                    StartCoroutine(DelayedPlaceSolid());
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

    public void OnShapeChanged(Component comp, object data)
    {
        _allowPlace = false;
        if (data is CurrentShape)
        {
            currentShapeButtonText.text = data.ToString();
            switch (data)
            {
                case CurrentShape.Cone:
                    currentShapePrefab = conePrefab;
                    break;
                case CurrentShape.Cube:
                    currentShapePrefab = cubePrefab;
                    break;
                case CurrentShape.Cylinder:
                    currentShapePrefab = cylinderPrefab;
                    break;
                case CurrentShape.Pyramid:
                    currentShapePrefab = pyramidPrefab;
                    break;
                case CurrentShape.Rectangle:
                    currentShapePrefab = rectanglePrefab;
                    break;
                case CurrentShape.Sphere:
                    currentShapePrefab = spherePrefab;
                    break;
            }
        }

        if (GameManager.Instance.currentGameState == GameState.PlacingShape)
        {
            StartCoroutine(DelayedPlaceSolid());
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
    private IEnumerator DelayedPlaceSolid()
    {
        yield return new WaitForSeconds(0.5f);
        _allowPlace = true;
    }

    public void UndoPlaceShape()
    {
        if (GameManager.Instance.currentGameState != GameState.PlacingShape)
        {
            updateGameState.Raise(this, GameState.PlacingShape);
        }
    }
    private bool InstantiateGameObjectAtTouch(Vector2 touchPosition, PlaceShapeAction action)
    {
        var ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = Instantiate(currentShapePrefab);
            go.transform.position = hit.point;
            action.DoAction(go);
            go.transform.SetParent(parent.transform, true);

            //Delaying placement of multiple objects
            _allowPlace = false;
            StartCoroutine(DelayedPlaceSolid());
            return true;
        }
        return false;
    }

    private bool InstantiateGameObjectAtARPlane(Vector2 touchPosition)
    {
        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var placedObject = Instantiate(currentShapePrefab, hitPose.position, hitPose.rotation);
            var placeAction = new PlaceShapeAction(this);
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
            StartCoroutine(DelayedPlaceSolid());
            return true;
        }
        return false;
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

        if (!InstantiateGameObjectAtTouch(touchPosition, new PlaceShapeAction(this)))
        {
            InstantiateGameObjectAtARPlane(touchPosition);
        }
    }
}
