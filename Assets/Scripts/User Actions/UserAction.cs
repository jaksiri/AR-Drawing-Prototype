using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserAction
{
    public bool isFinished;
    public virtual void DoAction()
    {
        addActionToGameManagerList();
    }

    // Always called by the Game Manager when the UI button is pressed
    public abstract void UndoAction();

    private void addActionToGameManagerList()
    {
        GameManager.Instance.actionList.Add(this);
    }
}
