using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    // Public variables for managing game state
    public DrawingState currentDrawingState;


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

        // Set Initial Conditions
        currentDrawingState = DrawingState.Outer;
    }

    public void UpdateGameState(Component comp, object data)
    {
        if (data is DrawingState)
        {
            DrawingState state = (DrawingState)data;
            currentDrawingState = state;
        }
    }
}


public enum DrawingState
{
    Inner, Outer
}