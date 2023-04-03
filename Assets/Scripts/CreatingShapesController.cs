using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class CreatingShapesController : MonoBehaviour
{
    private bool _activated = false;
    private bool _placedParent = false;
    private GameObject parent;
    private Vector2 midScreen;
    private Vector3 midPoint;
    private ARRaycastManager aRRaycastManager;
    public TMP_Text currentShapeDrawText;

    [Header("Placement Marker")]
    public GameObject placementIndicatorMarker;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private RaycastHit hit;
    private GameObject previewOfGameObject;

    // Marker Points
    private GameObject firstPoint;
    private GameObject secondPoint;
    private GameObject thirdPoint;
    private GameObject heightPoint;
    private Vector3 heightPointInitialOffset = new Vector3(0.0f, 0.01f, 0.0f);
    private Quaternion rotation;
    private GameObject referencePlane;
    private CreatingShapesState creatingShapesState = CreatingShapesState.PlacingStart;


    //Line Stuff
    private LineRenderer currLine;

    [Header("Prefabs to be placed")]
    public GameObject heightPlacementPlanePrefab;
    private GameObject prefabToBePlaced;
    public GameObject cubePrefab;
    public GameObject cylinderPrefab;
    public GameObject pyramidPrefab;
    public GameObject conePrefab;

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
        if (creatingShapesState == CreatingShapesState.PlacingMiddle)
        {
            secondPoint.transform.position = placementIndicatorMarker.transform.position;
            currLine.SetPosition(1, placementIndicatorMarker.transform.position);
        }
        if (creatingShapesState == CreatingShapesState.PlacingEnd)
        {
            thirdPoint.transform.position = placementIndicatorMarker.transform.position;
            heightPoint.transform.position = thirdPoint.transform.position + new Vector3(0.0f, 0.03f, 0.0f);


            DrawShapePreview();
        }
        else if (creatingShapesState == CreatingShapesState.PlacingHeight)
        {
            placementIndicatorMarker.SetActive(false);
            heightPoint.transform.position = hitPointFromRaycast();


            DrawShapePreview();
        }
    }

    public void OnDrawCurrentShapeChanged(Component comp, object data)
    {
        if (data is CurrentShapeDraw)
        {
            currentShapeDrawText.text = data.ToString();
            switch (data)
            {
                case CurrentShapeDraw.Cube:
                    prefabToBePlaced = cubePrefab;
                    break;
                case CurrentShapeDraw.Cone:
                    prefabToBePlaced = conePrefab;
                    break;
                case CurrentShapeDraw.Cylinder:
                    prefabToBePlaced = cylinderPrefab;
                    break;
                case CurrentShapeDraw.Pyramid:
                    prefabToBePlaced = pyramidPrefab;
                    break;
            }
        }
    }

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {

            switch (data)
            {
                case GameState.CreatingShape:
                    _activated = true;
                    placementIndicatorMarker.SetActive(true);

                    //Reset Conditions
                    creatingShapesState = CreatingShapesState.PlacingStart;

                    if (previewOfGameObject != null)
                    {
                        Destroy(previewOfGameObject);
                    }
                    if (firstPoint != null)
                    {
                        Destroy(currLine);
                        Destroy(firstPoint);
                    }

                    firstPoint = null;
                    secondPoint = null;
                    thirdPoint = null;
                    heightPoint = null;
                    rotation = default;

                    // Setting Parent
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
                    _activated = false;
                    placementIndicatorMarker.SetActive(false);
                    if (previewOfGameObject != null)
                    {
                        Destroy(currLine);
                        Destroy(previewOfGameObject);
                    }

                    break;
            }
        }
    }

    public void OnPressedMarked()
    {

        if (creatingShapesState == CreatingShapesState.PlacingStart)
        {
            firstPoint = Instantiate(new GameObject(), placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            secondPoint = Instantiate(new GameObject(), placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            thirdPoint = Instantiate(new GameObject(), placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            heightPoint = Instantiate(new GameObject(), placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);

            //Parenting points to the first point
            secondPoint.transform.SetParent(firstPoint.transform);
            thirdPoint.transform.SetParent(firstPoint.transform);
            heightPoint.transform.SetParent(firstPoint.transform);

            var markShapePointsAction = new MarkShapePointsAction(firstPoint, creatingShapesState);
            markShapePointsAction.DoAction();

            //Line Drawing
            currLine = firstPoint.AddComponent<LineRenderer>();
            currLine.material = new Material(Shader.Find("Sprites/Default"));
            currLine.startColor = Color.white;
            currLine.endColor = Color.white;
            currLine.startWidth = 0.02f;
            currLine.endWidth = 0.02f;
            currLine.positionCount = 2;
            currLine.SetPosition(0, firstPoint.transform.position);

            creatingShapesState = CreatingShapesState.PlacingMiddle;
        }
        else if (creatingShapesState == CreatingShapesState.PlacingMiddle)
        {
            var markShapePointsAction = new MarkShapePointsAction(secondPoint, creatingShapesState);
            markShapePointsAction.DoAction();

            rotation = Quaternion.LookRotation(secondPoint.transform.position - firstPoint.transform.position, hits[0].pose.up);
            Debug.Log("Rotation: " + rotation.eulerAngles);

            //Clear Line
            Destroy(currLine);

            creatingShapesState = CreatingShapesState.PlacingEnd;
        }
        else if (creatingShapesState == CreatingShapesState.PlacingEnd)
        {
            var markShapePointsAction = new MarkShapePointsAction(thirdPoint, creatingShapesState);
            markShapePointsAction.DoAction();

            //Create plane for height raycasting
            referencePlane = Instantiate(heightPlacementPlanePrefab, thirdPoint.transform.position, rotation);

            creatingShapesState = CreatingShapesState.PlacingHeight;
        }
        else
        {
            //Check if parent exists
            if (!_placedParent)
            {
                parent = Instantiate(new GameObject(), midPoint, rotation);
                parent.AddComponent<TwoFingerRotate>();
                GameManager.Instance.drawingParent = parent;
                _placedParent = true;
            }

            //Instantiating the object and setting parent
            GameObject placedCustomGameObject = Instantiate(previewOfGameObject, previewOfGameObject.transform.position, previewOfGameObject.transform.rotation);
            placedCustomGameObject.transform.SetParent(parent.transform);

            //Clear Preview Object
            Destroy(previewOfGameObject);
            Destroy(firstPoint);
            Destroy(referencePlane);

            // Changing State
            placementIndicatorMarker.SetActive(true);
            creatingShapesState = CreatingShapesState.PlacingStart;
        }
    }

    private void DrawShapePreview()
    {
        if (previewOfGameObject == null)
        {
            previewOfGameObject = Instantiate(prefabToBePlaced);
        }
        if (!previewOfGameObject.activeInHierarchy)
        {
            previewOfGameObject.SetActive(true);
        }


        midPoint = GetMidPoint(firstPoint.transform.position, new Vector3(thirdPoint.transform.position.x, thirdPoint.transform.position.y, secondPoint.transform.position.z));

        // float distanceX = Vector3.Distance(secondPoint.transform.position, thirdPoint.transform.position);
        float distanceX = Mathf.Abs(secondPoint.transform.position.x - thirdPoint.transform.position.x);
        float distanceY = Vector3.Distance(heightPoint.transform.position, thirdPoint.transform.position);
        float distanceZ = Vector3.Distance(firstPoint.transform.position, secondPoint.transform.position);

        previewOfGameObject.transform.position = midPoint;
        previewOfGameObject.transform.rotation = rotation;
        previewOfGameObject.transform.localScale = new Vector3(distanceX, distanceY, distanceZ);
    }

    private Vector3 hitPointFromRaycast()
    {
        var ray = Camera.main.ScreenPointToRay(midScreen);
        int layerMask = 1 << 6;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.point.y < thirdPoint.transform.position.y)
            {
                return thirdPoint.transform.position + heightPointInitialOffset;
            }

            return hit.point;
        }
        else
        {
            return thirdPoint.transform.position + heightPointInitialOffset;
        }
    }

    private Vector3 GetMidPoint(Vector3 startPoint, Vector3 endPoint)
    {
        return new Vector3((startPoint.x + endPoint.x) / 2, endPoint.y, (startPoint.z + endPoint.z) / 2);
    }
}

public enum CreatingShapesState
{
    PlacingStart, PlacingMiddle, PlacingEnd, PlacingHeight
}

//Todo: Set parents for all other points and lock axis when dragging