using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlacementController))]
public class PlacedObjectManager : MonoBehaviour
{
    private PlacementController placementController;
    void Start()
    {
        placementController = GetComponent<PlacementController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
