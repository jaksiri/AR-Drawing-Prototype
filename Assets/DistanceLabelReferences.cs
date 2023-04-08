using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DistanceLabelReferences : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelText;
    [SerializeField]
    private TMP_Text numberText;
    [SerializeField]
    private GameObject container;

    private Canvas canvas;

    private void OnEnable()
    {
        canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = GameObject.Find("UI Camera").GetComponent<Camera>();
    }

    public void SetLabelText(string value)
    {
        labelText.text = value;
    }

    public void SetNumberText(string value)
    {
        numberText.text = value;
    }

    public void OnShowPlanesChanged(Component comp, object data)
    {
        if (data is bool)
        {
            container.SetActive((bool)data);
        }
    }
}
