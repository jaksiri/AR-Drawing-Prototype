using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSceneAction : UserAction
{
    private GameObject oldDrawingParent;
    private GameState prevGameState;
    private GameObject[] placedObjects;

    public override void DoAction()
    {
        if (GameManager.Instance.drawingParent != null)
        {
            this.oldDrawingParent = GameManager.Instance.drawingParent;
            oldDrawingParent.SetActive(false);
            GameManager.Instance.drawingParent = null;
        }

        this.prevGameState = GameManager.Instance.currentGameState;

        placedObjects = GameObject.FindGameObjectsWithTag("PlacedObject");
        foreach (var item in placedObjects)
        {
            Debug.Log(item.transform.root);
            item.SetActive(false);
        }

        base.DoAction();
    }

    public override void UndoAction()
    {

        if (oldDrawingParent == null || placedObjects.Length == 0)
        {
            return;
        }

        if (placedObjects.Length != 0)
        {
            foreach (var item in placedObjects)
            {
                item.SetActive(true);
            }
        }

        oldDrawingParent.SetActive(true);
        GameManager.Instance.drawingParent = oldDrawingParent;

        if (prevGameState == GameState.Drawing)
        {
            UIManager.Instance.UndoClearScene(GameState.Drawing);
        }
        else
        {
            UIManager.Instance.UndoClearScene(GameManager.Instance.currentGameState);
        }
    }
}
