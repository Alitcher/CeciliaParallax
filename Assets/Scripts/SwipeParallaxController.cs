using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwipeParallaxController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] float minSwipeLength = 0.25f;
    [SerializeField] float maxSwipeLength; // TODO: define the max length users can swipe on android

    [Tooltip("Whether to detect eight or four cardinal directions")]
    [SerializeField] bool useEightDirections = false;

    #endregion
    private const float eightDirAngle = 0.906f;
    private const float fourDirAngle = 0.5f;

    private const float scrollSensitivity = 10f;
    private float currentSpeed;
    private float mouseDelta;

    [SerializeField] [Range(0.01f, 0.25f)] private float fullSpeed;
    [SerializeField] [Range(1, 50f)] private float decelerationRate;
    [SerializeField] [Range(0, 2f)] private float[] layersScrollSpeed;
    [SerializeField] private Transform[] environmentLayer;

    private const float _leftBoundary = -25f;
    private const float _rightBoundary = 25f;

    // Swipe layers speed values 

    private Dictionary<Swipe, Vector2> cardinalDirections = new Dictionary<Swipe, Vector2>()
    {
        { Swipe.Right,         Vector2.right           },
        { Swipe.Left,         Vector2.left             },
    };

    private Swipe swipeDirection;
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe => secondPressPos - firstPressPos;
    private void Start()
    {
#if !UNITY_EDITOR
        fullSpeed *= 4f;
#endif
    }

    private void Update()
    {
        DetectSwipe();
        HandleSwipe();
        AssertStopScrolling();
    }

    /// <summary>
    /// Attempts to detect the current swipe direction.
    /// Should be called over multiple frames in an Update-like loop.
    /// </summary>
    private void DetectSwipe()
    {
        if (GetMouseInput())
        {
            currentSpeed = fullSpeed;


            if (mouseDelta < minSwipeLength)
            {
                // Swipe was not long enough, abort
                ResetScrollSpeed();
                return;
            }

            swipeDirection = GetSwipeDirByTouch(currentSwipe);
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
        mouseDelta = 0;
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

    public void AddSpeed(float speedSwipe)
    {
        currentSpeed = speedSwipe;
    }

    private bool IsSwipingRight() => IsSwipingDirection(Swipe.Right);
    private bool IsSwipingLeft() => IsSwipingDirection(Swipe.Left);

    #region Helper Functions

#if CHEAT_BUILD
#endif
    private bool GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))  // Swipe/Click started
        {
            firstPressPos = (Vector2)Input.mousePosition;
            if (!Vector2.Equals((Vector2)Input.mousePosition, secondPressPos))
            {
                mouseDelta = firstPressPos.magnitude * decelerationRate;
            }
            return true;
        }
        else if (Input.GetMouseButton(0)) // Swipe/Drag
        {
            secondPressPos = (Vector2)Input.mousePosition;
            mouseDelta = (secondPressPos - firstPressPos).magnitude * decelerationRate;
            return true;
        }
        else
        {
            secondPressPos = (Vector2)Input.mousePosition;
            return false;
        }
    }

    private bool IsDirection(Vector2 direction, Vector2 cardinalDirection)
    {
        float angle = useEightDirections ? eightDirAngle : fourDirAngle;
        return Vector2.Dot(direction, cardinalDirection) > angle;
    }

    private Swipe GetSwipeDirByTouch(Vector2 currentSwipe)
    {
        currentSwipe.Normalize();
        KeyValuePair<Swipe, Vector2> swipeDir = cardinalDirections.FirstOrDefault(dir => IsDirection(currentSwipe, dir.Value));
        return swipeDir.Key;
    }

    private bool IsSwipingDirection(Swipe swipeDir) => swipeDirection == swipeDir;

    private enum Swipe
    {
        None,
        Left,
        Right,
    };
    #endregion
}


