using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAction : UserAction
{
    private List<GameObject> drawnItemList = new List<GameObject>();

    public override void UndoAction()
    {
        foreach (GameObject item in drawnItemList)
        {
            GameObject.Destroy(item);
        }
    }
    public void AddItemsToList(GameObject item)
    {
        drawnItemList.Add(item);
    }
}
