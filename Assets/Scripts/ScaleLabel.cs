using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLabel : MonoBehaviour
{
    private float scaleFactor = 0.0007f;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(gameObject.transform.position, mainCamera.transform.position);
        gameObject.transform.localScale = new Vector3(scaleFactor * distance, scaleFactor * distance, 1);
    }
}
