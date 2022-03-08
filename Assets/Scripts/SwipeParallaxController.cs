using UnityEngine;

public class SwipeParallaxController : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] [Range(10000, 30000f)] private float maxMouseDeltaPc;
    [SerializeField] [Range(0.001f, 0.1f)] private float fullSpeed;
    [SerializeField] [Range(1, 50f)] private float decelerationRate;
    [SerializeField] [Range(0, 2f)] private float[] layersScrollSpeed;
    [SerializeField] private Transform[] environmentLayer;
    #endregion

    private const float minSwipeLength = 0.01f;
    private const float scrollSensitivity = 10f;
    private const float _leftBoundary = -45f;
    private const float _rightBoundary = 45f;

    private Swipe swipeDirection;
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    private Vector2 fingerHoldPos;

    private float holdTime;
    private float currentSpeed;

    private void Start()
    {
#if !UNITY_EDITOR
        fullSpeed *= 4f;
#endif
    }

    private float getDynamicDeceleration()
    {
        return (currentMouseDelta / maxMouseDeltaPc) * decelerationRate;
    }

    private void Update()
    {
        DetectSwipe();
        HandleSwipe();
        AssertStopScrolling();
    }

    private void DetectSwipe()
    {
        if (GetMouseInput())
        {
            currentSpeed = fullSpeed;
            if (currentMouseDelta < minSwipeLength)
            {
                // Swipe was not long enough, abort
                ResetScrollSpeed();
                return;
            }
            swipeDirection = GetSwipeDirByTouch();
        }
    }

    private void AssertStopScrolling()
    {
        if (currentSpeed > 0)
        {
#if UNITY_EDITOR
            currentSpeed -= Time.deltaTime / decelerationRate;
#else
            currentSpeed -= Time.deltaTime;
#endif
        }
        else
        {
            ResetScrollSpeed();
        }
    }

    private void ResetScrollSpeed()
    {
        currentSpeed = 0;
        swipeDirection = Swipe.None;
    }

    private void HandleSwipe()
    {
        float targetX;
        Vector2 target;

        if (IsSwipingLeft())
        {
            if (environmentLayer[environmentLayer.Length - 2].transform.position.x >= _leftBoundary)
            {
                for (int i = 0; i < environmentLayer.Length; i++)
                {
                    targetX = (i == 0) ? environmentLayer[i].position.x + scrollSensitivity * layersScrollSpeed[i] : environmentLayer[i].position.x - scrollSensitivity * layersScrollSpeed[i];
                    target = new Vector2(targetX, environmentLayer[i].position.y);
                    environmentLayer[i].position = Vector3.Lerp(environmentLayer[i].position, target, currentSpeed);
                }
            }
        }
        else if (IsSwipingRight())
        {
            if (environmentLayer[environmentLayer.Length - 2].transform.position.x <= _rightBoundary)
            {
                for (int i = 0; i < environmentLayer.Length; i++)
                {
                    targetX = (i == 0) ? environmentLayer[i].position.x - scrollSensitivity * layersScrollSpeed[i] : environmentLayer[i].position.x + scrollSensitivity * layersScrollSpeed[i];
                    target = new Vector2(targetX, environmentLayer[i].position.y);
                    environmentLayer[i].position = Vector3.Lerp(environmentLayer[i].position, target, currentSpeed);
                }
            }
        }
    }

    private bool IsSwipingRight() => IsSwipingDirection(Swipe.Right);
    private bool IsSwipingLeft() => IsSwipingDirection(Swipe.Left);

    private bool GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))  // Swipe/Click started
        {
            holdTime = 0;
            fingerDownPos = (Vector2)Input.mousePosition;
            return true;
        }
        else if (Input.GetMouseButton(0)) // Swipe/Drag
        {
            holdTime += Time.deltaTime;
            fingerHoldPos = (Vector2)Input.mousePosition;
            return true;
        }
        else
        {
            fingerUpPos = (Vector2)Input.mousePosition;
            return false;
        }
    }

    private bool IsDirection()
    {
        return (fingerHoldPos.x < fingerDownPos.x) ? true : false;
    }

    private Swipe GetSwipeDirByTouch()
    {
        return IsDirection() ? Swipe.Left : Swipe.Right;
    }

    private bool IsSwipingDirection(Swipe swipeDir) => swipeDirection == swipeDir;
    private Vector2 currentSwipe => fingerHoldPos - fingerDownPos;
    private float currentMouseDelta => (holdTime == 0) ? 0 : (fingerHoldPos - fingerDownPos).magnitude / holdTime;
    private float mouseDeltaBase => (fingerHoldPos - fingerDownPos).magnitude * getDynamicDeceleration();

    private enum Swipe
    {
        None,
        Left,
        Right,
    };
}


