using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour {

    private const float DEADZONE = 100.0f;

    public static MobileInput Instance { get; set; }

    private bool tap, swipeRight, swipeLeft, swipeUp, swipeDown;
    private Vector2 swipeDelta, startTouch;

    public bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Reset all the booleans
        tap = swipeRight = swipeLeft = swipeUp = swipeDown = false;

        #region Standalone Inputs

        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }
        #endregion

        #region Mobile Inputs

        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }

        #endregion

        // Calculate distance
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            // Mobile
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            // Standalone
            else if (Input.GetMouseButtonDown(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        // Check for deadzone, swipe is longer than 100px
        if (swipeDelta.magnitude > DEADZONE)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // Right of Left
                if (x > 0)
                    swipeRight = true;
                else
                    swipeLeft = true;
            }
            else
            {
                // Up or Down
                if (y > 0)
                    swipeUp = true;
                else
                    swipeDown = true;
            }

            startTouch = swipeDelta = Vector2.zero;
        }

    }
}
