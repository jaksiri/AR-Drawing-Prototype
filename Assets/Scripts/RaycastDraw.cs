using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class RaycastDraw : MonoBehaviour
{

    [SerializeField]
    private GameObject drawnPrefab;
    private bool _allowDrawing;

    private RaycastHit hit;
    private string currentDrawMode;
    private DrawAction currentDrawAction;
    private ARPlaneManager aRPlaneManager;

    private void Awake()
    {
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.Drawing:
                    StartCoroutine(AllowDrawingDelayed());
                    aRPlaneManager.SetTrackablesActive(false);
                    break;
                default:
                    _allowDrawing = false;
                    break;
            }
        }

    }

    void Update()
    {
        if (GameManager.Instance.currentGameState != GameState.Drawing)
        {
            return;
        }
        if (!_allowDrawing)
        {
            return;
        }

        if (Input.touchCount != 1)
        {
            return;
        }
        try
        {
            Touch touch = Input.GetTouch(0);
            if (touch.position.IsPointOverUIObject())
            {
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    currentDrawAction = new DrawAction();
                    InstantiateGameObjectAtTouch(touch.position, currentDrawAction);
                    break;
                case TouchPhase.Moved:
                    if (currentDrawAction == null)
                    {
                        currentDrawAction = new DrawAction();
                    }
                    InstantiateGameObjectAtTouch(touch.position, currentDrawAction);
                    break;
                case TouchPhase.Ended:
                    currentDrawAction.DoAction();
                    currentDrawAction = null;
                    break;
            }

        }
        catch (Exception e)
        {
            Debug.Log("Can't get touch " + e.GetType());
        }



    }

    private void InstantiateGameObjectAtTouch(Vector2 touchPosition, DrawAction action)
    {
        var ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = Instantiate(drawnPrefab);
            go.transform.position = hit.point;
            go.tag = "DrawnObject";
            action.AddItemsToList(go);
            go.transform.SetParent(hit.transform);
        }

    }

    IEnumerator AllowDrawingDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        _allowDrawing = true;
    }
}
