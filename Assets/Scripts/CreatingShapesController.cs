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
    private ARPlaneManager aRPlaneManager;
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
    private GameObject referencePlane2;
    private GameObject referencePlane3;
    private GameObject referencePlane4;
    private CreatingShapesState creatingShapesState = CreatingShapesState.PlacingStart;


    //Line Stuff
    private LineRenderer currLine;

    [Header("Prefabs to be placed")]
    public GameObject pointPrefab;
    public GameObject heightPlacementPlanePrefab;
    private GameObject prefabToBePlaced;
    public GameObject cubePrefab;
    public GameObject cylinderPrefab;
    public GameObject pyramidPrefab;
    public GameObject conePrefab;

    [Header("Distance Text Prefab")]
    public GameObject distanceTextPrefab;

    //private distance text reference
    private DistanceLabelReferences dlr1;
    private DistanceLabelReferences dlr2;
    private DistanceLabelReferences dlr3;
    private GameObject distanceText1;
    private GameObject distanceText2;
    private GameObject distanceText3;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        placementIndicatorMarker.SetActive(false);
        midScreen = new Vector2(Screen.width / 2, Screen.height / 2);

        prefabToBePlaced = cubePrefab;
        currentShapeDrawText.text = "Cube";
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

            //Distance Indicator
            PlaceDistanceText(distanceText1, firstPoint.transform.position, secondPoint.transform.position);
            dlr1.SetNumberText(ChangeDistanceTextValue(Vector3.Distance(firstPoint.transform.position, secondPoint.transform.position)));

        }
        if (creatingShapesState == CreatingShapesState.PlacingEnd)
        {
            thirdPoint.transform.position = placementIndicatorMarker.transform.position;
            thirdPoint.transform.localPosition = new Vector3(thirdPoint.transform.localPosition.x, thirdPoint.transform.localPosition.y, secondPoint.transform.localPosition.z);
            heightPoint.transform.position = thirdPoint.transform.position + heightPointInitialOffset;

            //Distance Indicator
            PlaceDistanceText(distanceText2, secondPoint.transform.position, thirdPoint.transform.position);
            dlr2.SetNumberText(ChangeDistanceTextValue(Vector3.Distance(secondPoint.transform.position, thirdPoint.transform.position)));

            DrawShapePreview();
        }
        else if (creatingShapesState == CreatingShapesState.PlacingHeight)
        {
            placementIndicatorMarker.SetActive(false);
            heightPoint.transform.position = hitPointFromRaycast();
            heightPoint.transform.localPosition = new Vector3(thirdPoint.transform.localPosition.x, heightPoint.transform.localPosition.y, thirdPoint.transform.localPosition.z);

            //Distance Indicator
            PlaceDistanceText(distanceText3, thirdPoint.transform.position, heightPoint.transform.position);
            dlr3.SetNumberText(ChangeDistanceTextValue(Vector3.Distance(thirdPoint.transform.position, heightPoint.transform.position)));

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
                    aRPlaneManager.SetTrackablesActive(true);

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
                        Destroy(firstPoint);
                        Destroy(secondPoint);
                        Destroy(thirdPoint);
                        Destroy(heightPoint);
                    }

                    break;
            }
        }
    }

    public void OnPressedMarked()
    {

        if (creatingShapesState == CreatingShapesState.PlacingStart)
        {
            firstPoint = Instantiate(pointPrefab, placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            secondPoint = Instantiate(pointPrefab, placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            thirdPoint = Instantiate(pointPrefab, placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);
            heightPoint = Instantiate(pointPrefab, placementIndicatorMarker.transform.position, placementIndicatorMarker.transform.rotation);

            //Line Drawing
            currLine = firstPoint.AddComponent<LineRenderer>();
            currLine.material = new Material(Shader.Find("Sprites/Default"));
            currLine.startColor = Color.white;
            currLine.endColor = Color.white;
            currLine.startWidth = 0.02f;
            currLine.endWidth = 0.02f;
            currLine.positionCount = 2;
            currLine.SetPosition(0, firstPoint.transform.position);

            ConfigureText(ref distanceText1, ref dlr1);
            dlr1.SetLabelText("Length");

            creatingShapesState = CreatingShapesState.PlacingMiddle;
        }
        else if (creatingShapesState == CreatingShapesState.PlacingMiddle)
        {
            rotation = Quaternion.LookRotation(secondPoint.transform.position - firstPoint.transform.position, hits[0].pose.up);
            firstPoint.transform.rotation = rotation;

            //Parenting points to the first point
            secondPoint.transform.SetParent(firstPoint.transform);
            thirdPoint.transform.SetParent(firstPoint.transform);
            heightPoint.transform.SetParent(firstPoint.transform);

            ConfigureText(ref distanceText2, ref dlr2);
            dlr2.SetLabelText("Width");

            //Clear Line
            Destroy(currLine);

            creatingShapesState = CreatingShapesState.PlacingEnd;
        }
        else if (creatingShapesState == CreatingShapesState.PlacingEnd)
        {
            ConfigureText(ref distanceText3, ref dlr3);
            dlr3.SetLabelText("Height");

            //Create plane for height raycasting
            referencePlane = Instantiate(heightPlacementPlanePrefab, thirdPoint.transform.position, rotation);
            referencePlane2 = Instantiate(heightPlacementPlanePrefab, thirdPoint.transform.position, rotation);
            referencePlane3 = Instantiate(heightPlacementPlanePrefab, thirdPoint.transform.position, rotation);
            referencePlane4 = Instantiate(heightPlacementPlanePrefab, thirdPoint.transform.position, rotation);

            //Rotating the reference planes for raycast target
            referencePlane2.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            referencePlane3.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
            referencePlane4.transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);

            //Set Plane Transform Parent
            referencePlane.transform.SetParent(firstPoint.transform);
            referencePlane2.transform.SetParent(firstPoint.transform);
            referencePlane3.transform.SetParent(firstPoint.transform);
            referencePlane4.transform.SetParent(firstPoint.transform);


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

            //Parent the distance labels
            distanceText1.transform.SetParent(parent.transform);
            distanceText2.transform.SetParent(parent.transform);
            distanceText3.transform.SetParent(parent.transform);

            //User Action
            PlaceCustomShapeAction pcsa = new PlaceCustomShapeAction(placedCustomGameObject);
            pcsa.DoAction();

            //Clear Preview Object
            Destroy(previewOfGameObject);
            Destroy(firstPoint);
            Destroy(referencePlane);
            Destroy(referencePlane2);
            Destroy(referencePlane3);
            Destroy(referencePlane4);

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

        midPoint = GetMidPoint(firstPoint.transform.position, thirdPoint.transform.position);

        float distanceX = Vector3.Distance(secondPoint.transform.position, thirdPoint.transform.position);
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
        return new Vector3((startPoint.x + endPoint.x) / 2, (startPoint.y + endPoint.y) / 2, (startPoint.z + endPoint.z) / 2);
    }

    private void PlaceDistanceText(GameObject obj, Vector3 startPoint, Vector3 endPoint)
    {
        if (!obj.activeInHierarchy)
        {
            obj.SetActive(true);
        }
        obj.transform.position = GetMidPoint(startPoint, endPoint);
    }

    private string ChangeDistanceTextValue(float value)
    {
        return System.Math.Round(value * 100.0f, 1) + " cm.";
    }

    private void ConfigureText(ref GameObject go, ref DistanceLabelReferences dlr)
    {
        go = Instantiate(distanceTextPrefab);
        dlr = go.GetComponent<DistanceLabelReferences>();
    }
}

public enum CreatingShapesState
{
    PlacingStart, PlacingMiddle, PlacingEnd, PlacingHeight
}
