using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


public class DrawLineManager : MonoBehaviour
{
    private GameObject _penTip;
    public Slider lineThicknessSlider;
    private LineRenderer currLine;
    private bool _drawing;
    private bool _prevDrawing = false;
    private float _lineWidth = 0.05f;
    private int numClicks = 0;
    public bool drawing
    {
        get
        {
            return _drawing;
        }
        set
        {
            _drawing = value;
        }
    }
    public float lineWidth
    {
        get
        {
            return _lineWidth;
        }
        set
        {
            _lineWidth = value;
        }
    }

    private void Awake()
    {
        _penTip = gameObject;
        lineThicknessSlider.onValueChanged.AddListener(delegate { UpdateThickness(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (_drawing && !_prevDrawing)
        {
            GameObject go = new GameObject();
            go.transform.position = _penTip.transform.position;
            go.tag = "DrawnObject";
            currLine = go.AddComponent<LineRenderer>();
            currLine.startWidth = _lineWidth;
            currLine.endWidth = _lineWidth;
            _prevDrawing = true;
            numClicks = 0;
        }
        else if (_drawing && _prevDrawing)
        {
            currLine.endWidth = _lineWidth;
            currLine.positionCount = numClicks + 1;
            currLine.SetPosition(numClicks, _penTip.transform.position);
            numClicks++;
        }
        else
        {
            _prevDrawing = false;
        }
    }

    private void UpdateThickness()
    {
        _lineWidth = lineThicknessSlider.value;
    }
}
