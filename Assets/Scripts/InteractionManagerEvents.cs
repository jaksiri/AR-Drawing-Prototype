using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class InteractionManagerEvents : MonoBehaviour
{
    public ARGestureInteractor aRGestureInteractor;
    // Start is called before the first frame update
    public void OnGameStateUpdate(Component comp, object data)
    {
        if (data is GameState)
        {
            switch (data)
            {
                case GameState.Home:
                    aRGestureInteractor.enabled = true;
                    break;
                default:
                    aRGestureInteractor.enabled = false;
                    break;
            }
        }
    }
}
