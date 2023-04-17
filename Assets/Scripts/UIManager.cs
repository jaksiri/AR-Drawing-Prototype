using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private bool showSetting = false;
    private bool showShapeDropdown = false;
    private bool showShapeDrawDropdown = false;
    [Header("Events")]
    public GameEvent updateGameState;
    public GameEvent showPlanesEvent;

    [Header("UI References")]
    public GameObject settingMenu;

    [Header("Draw UI")]
    public GameObject drawGuidePlacementDoneButton;
    public GameObject drawUISelector;
    public GameObject drawDoneButton;
    [Header("Shapes Placement UI")]
    public GameObject shapesDoneButton;
    public GameObject currentShapeButton;
    public GameObject placedShapesDropdown;

    [Header("Shapes Creation UI")]
    public GameObject shapesCreationDoneButton;
    public GameObject shapesActionButton;
    public GameObject shapesSelectionButton;
    public GameObject shapesDrawSelectionDropdown;

    [Header("Settings UI")]
    public Toggle ShowHideToggle;

    [Header("AR Session")]
    public ARSession arSession;

    void Awake()
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

        // Initial Setup
        settingMenu.SetActive(false);
        placedShapesDropdown.SetActive(false);
        shapesDrawSelectionDropdown.SetActive(false);

        //Toggle
        ShowHideToggle.onValueChanged.AddListener(TogglePlaneGuide);
        TogglePlaneGuide(ShowHideToggle.isOn);
    }

    private void OnDestroy()
    {
        ShowHideToggle.onValueChanged.RemoveListener(TogglePlaneGuide);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Used only for updating internal Game State, DO NOT USE FOR SETTING GLOBAL GAME STATE
    public void OnGameStateUpdated(Component comp, object data)
    {
        if (data is GameState)
        {
            //Shape Creation
            shapesCreationDoneButton.SetActive((GameState)data == GameState.CreatingShape);
            shapesActionButton.SetActive((GameState)data == GameState.CreatingShape);
            shapesSelectionButton.SetActive((GameState)data == GameState.CreatingShape);

            //Drawing
            drawUISelector.SetActive((GameState)data == GameState.Drawing);
            drawDoneButton.SetActive((GameState)data == GameState.Drawing);

            //PlacingPlane
            drawGuidePlacementDoneButton.SetActive((GameState)data == GameState.PlacingPlane);

            //CreatingPlane

            //Placing Shape
            currentShapeButton.SetActive((GameState)data == GameState.PlacingShape);
            shapesDoneButton.SetActive((GameState)data == GameState.PlacingShape);

            switch (data)
            {
                case GameState.CreatingPlane:
                    break;
                case GameState.CreatingShape:
                    break;
                case GameState.Drawing:
                    break;
                case GameState.PlacingPlane:
                    break;
                case GameState.PlacingShape:
                    break;
            }
        }
    }

    public void ToggleSetting()
    {
        showSetting = !showSetting;
        settingMenu.SetActive(showSetting);
    }

    public void ToggleShapeDropdown()
    {
        showShapeDropdown = !showShapeDropdown;
        placedShapesDropdown.SetActive(showShapeDropdown);
    }

    public void ToggleShapeDrawDropdown()
    {
        showShapeDrawDropdown = !showShapeDrawDropdown;
        shapesDrawSelectionDropdown.SetActive(showShapeDrawDropdown);
    }

    public void TogglePlaneGuide(bool value)
    {
        showPlanesEvent.Raise(this, value);
    }
    public void SwitctToDraw()
    {
        ShowHideToggle.isOn = true;
        TogglePlaneGuide(true);
        updateGameState.Raise(this, GameState.Drawing);
    }

    public void SwitchToShape()
    {
        updateGameState.Raise(this, GameState.PlacingShape);
    }

    public void SwitchToCreate()
    {
        updateGameState.Raise(this, GameState.CreatingShape);
    }

    public void SwitchToGuide()
    {
        updateGameState.Raise(this, GameState.PlacingPlane);
    }

    public void FinishPlacingGuide()
    {
        updateGameState.Raise(this, GameState.Drawing);
    }

    public void FinishgPlacingShape()
    {
        updateGameState.Raise(this, GameState.Home);
    }

    public void FinishCreatingShape()
    {
        updateGameState.Raise(this, GameState.Home);
        
    }

    public void ClearScene()
    {
        if (GameManager.Instance.drawingParent == null && GameObject.FindGameObjectsWithTag("PlacedObject").Length == 0)
        {
            return;
        }

        var clearAction = new ClearSceneAction();
        clearAction.DoAction();
        arSession.Reset();

        if (GameManager.Instance.currentGameState == GameState.Drawing)
        {
            updateGameState.Raise(this, GameState.PlacingPlane);
        }
        else
        {
            updateGameState.Raise(this, GameManager.Instance.currentGameState);
        }
    }
    public void UndoClearScene(GameState state)
    {
        Debug.Log("Undo Clear Scene");
        updateGameState.Raise(this, state);
    }
}
