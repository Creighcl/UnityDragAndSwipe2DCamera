using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CameraDragAndSwipe : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    Queue<TouchPointHistoricalEntry> touchPointHistory = new Queue<TouchPointHistoricalEntry>();

    [Header("Swipe Controls")]
    int maxFramesInTouchPointHistory = 20;
    [SerializeField] float swipeForce = 44f;
    [SerializeField] float swipeThreshold = 44f;
    [SerializeField] float trajectoryLookbackSeconds = 0.1f;
    [SerializeField] float xAxisSensitivity = 1f;
    [SerializeField] float yAxisSensitivity = .33f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();     
    }

    void Update()
    {
        HandleUserInput();
    }

    private void HandleUserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchPointHistory.Clear();
            _rigidbody.velocity = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            CaptureFrameToHistory();
            PanCameraAcrossLastTwoHistoricalPoints();
            PruneCaptureFrameHistory();

        }

        if (Input.GetMouseButtonUp(0))
        {
            var history = new List<TouchPointHistoricalEntry>(touchPointHistory);
            SwipeCamera(history);
        }
    }

    private void SwipeCamera(List<TouchPointHistoricalEntry> history)
    {
        var swipeMagnitude = GetTouchMagnitudeFromHistory();
        if (swipeMagnitude >= swipeThreshold)
        {
            _rigidbody.velocity = GetTouchVelocityFromHistory() * swipeForce;
        }
    }

    private void CaptureFrameToHistory()
    {
        var thisFrame = new TouchPointHistoricalEntry(Time.deltaTime, Input.mousePosition);
        touchPointHistory.Enqueue(thisFrame);
    }

    private void PruneCaptureFrameHistory()
    {
        if (touchPointHistory.Count > maxFramesInTouchPointHistory)
        {
            touchPointHistory.Dequeue();
        }
    }

    private void PanCameraAcrossLastTwoHistoricalPoints()
    {
        if (touchPointHistory.Count > 1)
        {
            var secondLastFrame = touchPointHistory.Reverse().Skip(1).Take(1).SingleOrDefault();
            var lastFrame = touchPointHistory.Last();
            Vector2 lastWorldSpace = Camera.main.ScreenToWorldPoint(lastFrame.Position);
            Vector2 secondLastWorldSpace = Camera.main.ScreenToWorldPoint(secondLastFrame.Position);
            Vector2 movementThisFrame = secondLastWorldSpace - lastWorldSpace;
            transform.position = transform.position + (new Vector3(movementThisFrame.x, movementThisFrame.y, 0f));
        }
    }

    private Vector2 GetReleasedTouchDelta()
    {
        Vector2 lastTouchedPosition = touchPointHistory.Last().Position;
        Vector2 trailingTrajectoryBase = getSnapshotAtSecondsAgoFromHistory(trajectoryLookbackSeconds).Position;
        Vector2 delta = (trailingTrajectoryBase - lastTouchedPosition);
        return delta;
    }

    private float GetTouchMagnitudeFromHistory()
    {
        float magnitude = GetReleasedTouchDelta().magnitude;
        return magnitude;
    }

    private Vector2 GetTouchVelocityFromHistory()
    {
        Vector2 velocity = GetReleasedTouchDelta().normalized;
        return velocity;
    }

    private Vector2 GetSensitivityAdjustedVelocityFromHistory()
    {
        Vector2 velocity = GetTouchVelocityFromHistory();
        velocity.x = velocity.x * xAxisSensitivity;
        velocity.y = velocity.y * yAxisSensitivity;
        return velocity;
    }



    private TouchPointHistoricalEntry getSnapshotAtSecondsAgoFromHistory(float seconds)
    {
        var thumbThrough = touchPointHistory.Reverse<TouchPointHistoricalEntry>();
        var traversedTime = 0f;
        TouchPointHistoricalEntry returnSnap = thumbThrough.First();
        foreach (TouchPointHistoricalEntry snap in thumbThrough)
        {
            returnSnap = snap;
            traversedTime += snap.TimeDelta;
            if (traversedTime >= seconds)
            {
                break;
            }
        }

        return returnSnap;
    }

    private class TouchPointHistoricalEntry
    {
        public TouchPointHistoricalEntry(float timeDelta, Vector2 position)
        {
            TimeDelta = timeDelta;
            Position = position;
        }

        public float TimeDelta { get; set; }
        public Vector2 Position { get; set; }
    }
}
