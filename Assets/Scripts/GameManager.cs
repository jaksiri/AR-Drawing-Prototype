using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    // Public variables for managing game state
    [Header("Current States")]
    public DrawingState currentDrawingState;
    public GameState currentGameState;
    public PlacementState currentPlacementState;
    public CurrentShape currentShapeState;

    public GameObject drawingParent;
    [Header("Events")]
    public GameEvent updateGameState;
    public GameEvent updateDrawState;
    public GameEvent updateCurrentShape;

    //List of User Action
    public List<UserAction> actionList = new List<UserAction>();

    //References
    [Header("Text References")]
    public TMP_Text gameStateText;

    private void Awake()
    {
        // Singleton Stuff
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Set Initial Conditions
        StartCoroutine(DelayedEventsUpdate());
    }

    public void UpdateGameState(Component comp, object data)
    {
        if (data is GameState)
        {
            currentDrawingState = (DrawingState)data;
            currentGameState = (GameState)data;
            gameStateText.text = currentGameState.ToString();
        }
        // switch (currentGameState)
        // {
        //     case GameState.CreatingPlane:
        //         break;
        //     case GameState.PlacingPlane:
        //         break;
        //     case GameState.Drawing:
        //         break;
        //     case GameState.CreatingShape:
        //         break;
        //     case GameState.PlacingShape:
        //         break;
        //     case GameState.Home:
        //         break;
        // }
    }

    public void UpdateDrawingState(Component comp, object data)
    {
        if (data is DrawingState)
        {
            currentDrawingState = (DrawingState)data;
        }
    }

    public void UpdatePlacementState(Component comp, object data)
    {
        if (data is PlacementState)
        {
            currentPlacementState = (PlacementState)data;
        }
    }

    public void UpdateCurrentShape(Component comp, object data)
    {
        if (data is CurrentShape)
        {
            currentShapeState = (CurrentShape)data;
        }
    }

    public void UndoLastAction()
    {
        if (actionList.Count == 0)
        {
            return;
        }
        actionList[actionList.Count - 1].UndoAction();
        actionList.RemoveAt(actionList.Count - 1);
    }

    private IEnumerator DelayedEventsUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        updateGameState.Raise(this, GameState.Home);
        updateDrawState.Raise(this, DrawingState.Outer);
        updateCurrentShape.Raise(this, CurrentShape.Cube);
    }
}


//Enums
#region 
public enum DrawingState
{
    Inner, Outer, None
}

public enum GameState
{
    Home, CreatingPlane, PlacingPlane, Drawing, CreatingShape, PlacingShape
}

public enum PlacementState
{
    Anchor, Hover
}

public enum CurrentShape
{
    Cube, Rectangle, Cylinder, Cone, Pyramid, Sphere
}

public enum CurrentShapeDraw
{
    Cube, Cylinder, Cone, Pyramid
}

#endregion