using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceLabelReferences : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelText;
    [SerializeField]
    private TMP_Text numberText;

    private Canvas canvas;

    private void OnEnable()
    {
        canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
    }

    public void SetLabelText(string value)
    {
        labelText.text = value;
    }

    public void SetNumberText(string value)
    {
        numberText.text = value;
    }
}
