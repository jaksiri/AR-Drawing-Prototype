using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanelAnimation : MonoBehaviour
{
    private Vector3 initialPosition;
    private void OnEnable()
    {
        initialPosition = gameObject.transform.localPosition;
        LeanTween.moveLocal(gameObject, new Vector3(0, 1389, 0), 0.25f);
    }

    private void OnDisable()
    {
        LeanTween.moveLocal(gameObject, initialPosition, 0.5f);
    }
}
