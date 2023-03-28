using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSceneAction : UserAction
{
    private GameObject oldDrawingParent;
    private GameState prevGameState;

    public override void DoAction()
    {
        this.oldDrawingParent = GameManager.Instance.drawingParent;
        this.prevGameState = GameManager.Instance.currentGameState;

        oldDrawingParent.SetActive(false);
        GameManager.Instance.drawingParent = null;
        base.DoAction();
    }

    public override void UndoAction()
    {
        if (oldDrawingParent == null)
        {
            return;
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
