using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoFingerRotate : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 direction;
    private float rotationFactor = 0.1f;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            switch (touch1.phase)
            {
                case TouchPhase.Began:
                    startPos = GetAverageTouch(touch1, touch2);
                    break;
                case TouchPhase.Moved:
                    Vector2 newTouch = GetAverageTouch(touch1, touch2);
                    direction = newTouch - startPos;
                    startPos = newTouch;
                    break;
                case TouchPhase.Stationary:
                    direction = default;
                    break;
                case TouchPhase.Ended:
                    startPos = default;
                    break;
            }
            RotateObject(direction);
        }

    }

    private Vector2 GetAverageTouch(Touch touch1, Touch touch2)
    {
        float avgx = (touch1.position.x + touch2.position.x) / 2;
        float avgy = (touch1.position.y + touch2.position.y) / 2;

        return new Vector2(avgx, avgy);
    }

    private void RotateObject(Vector2 direction)
    {
        Quaternion currentRotation = gameObject.transform.rotation;
        Quaternion toRotation = currentRotation * Quaternion.Euler(0, -direction.x, 0);
        gameObject.transform.rotation = Quaternion.Lerp(currentRotation, toRotation, rotationFactor);
    }
}
