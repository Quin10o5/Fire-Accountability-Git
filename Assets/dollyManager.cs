using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages Cinemachine dolly carts for touch-driven vertical (Y) and horizontal (X) camera movement,
/// with adjustable clamping and adaptive X-axis speed adjustment based on proximity to center.
/// </summary>
public class DollyManager : MonoBehaviour
{
    [Header("Touch Settings")]
    [Tooltip("Simulate two touches with one touch (for testing in Editor)")]
    [SerializeField] private bool fakeTwoTouches = false;

    [Header("Cinemachine Dolly Carts")]
    [Tooltip("Dolly cart controlling vertical movement (Y axis)")]
    [SerializeField] private CinemachineDollyCart dollyY;
    [Tooltip("Dolly cart controlling horizontal movement (X axis)")]
    [SerializeField] private CinemachineDollyCart dollyX;

    [Header("Speed Multipliers")]
    [Tooltip("Multiplier for vertical dolly speed (Y axis)")]
    [SerializeField] private float speedMultiplierY = 1f;
    [Tooltip("Multiplier for horizontal dolly speed (X axis)")]
    [SerializeField] private float speedMultiplierX = 1f;

    [Header("X Clamp Settings")]
    [Tooltip("Maximum dolly Y position at which X clamping starts")]
    [SerializeField] private float maxYForXClamp = 1f;
    [Tooltip("Maximum X offset range when Y is at minimum")]
    [SerializeField] private float maxXRange = 0.5f;

    [Header("Adaptive Snap Settings")]
    [Tooltip("Distance from center (0.5) within which X speed adapts")]
    [SerializeField] private float snapThreshold = 0.2f;
    [Tooltip("Minimum X speed when exactly at center")]
    [SerializeField] private float snapMinSpeed = 0.5f;
    [Tooltip("Maximum X speed when at threshold distance")]
    [SerializeField] private float snapMaxSpeed = 2f;

    // Internal touch tracking variables
    private Vector3 startPoint = Vector3.zero;
    private Vector3 currentPoint = Vector3.zero;
    private Vector3 offset = Vector3.zero;
    private float lastMagnitude = 0f;

    
    [Header("X Clamp Smoothing")]
    [Tooltip("How fast the X pos eases into the clamp bounds")]
    [SerializeField] private float clampSmoothTime = 0.15f;
// internal velocity tracker for SmoothDamp
    private float clampXVel = 0f;



    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    
    /// <summary>
    /// Update is called once per frame: process input, move dolly carts, clamp, and adapt X speed.
    /// </summary>
    private void Update()
    {
        HandleTouchInput();

        if (IsDragging())
        {
            CalculateOffset();
            //MoveDollyCarts();
            ClampDollyX();
        }
        else
        { 
            ResetMovement();
        }
        AdaptDollyXSpeed();
        
    }

    #region Touch Input Handling

    private void HandleTouchInput()
    {
        int touchCount = Input.touchCount;
        bool twoTouches = fakeTwoTouches ? (touchCount == 1) : (touchCount == 2);
        if (!twoTouches)
        {
            ResetTouchData();
            return;
        }

        Vector3 avgPos = fakeTwoTouches
            ? Input.touches[0].position
            : (Input.touches[0].position + Input.touches[1].position) * 0.5f;

        TouchPhase phase = fakeTwoTouches
            ? Input.touches[0].phase
            : (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
                ? TouchPhase.Began
                : (Input.touches[0].phase == TouchPhase.Moved && Input.touches[1].phase == TouchPhase.Moved)
                    ? TouchPhase.Moved
                    : (Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended)
                        ? TouchPhase.Ended
                        : TouchPhase.Moved;

        switch (phase)
        {
            case TouchPhase.Began:
                startPoint = avgPos;
                break;
            case TouchPhase.Moved:
                currentPoint = avgPos;
                break;
            case TouchPhase.Ended:
                ResetTouchData();
                break;
        }
    }

    private bool IsDragging() => startPoint != Vector3.zero && currentPoint != Vector3.zero;

    private void ResetTouchData()
    {
        startPoint = Vector3.zero;
        currentPoint = Vector3.zero;
        offset = Vector3.zero;
        lastMagnitude = 0f;
        ResetMovement();
    }

    #endregion

    #region Movement Calculation
    
    public float maxOffsetForClamp = 0.7f;
    

    private void CalculateOffset()
    {
        offset = currentPoint - startPoint;
        lastMagnitude = offset.magnitude;
        offset.Normalize();
        if (Mathf.Abs(offset.x) > maxOffsetForClamp)
        {
            offset = new Vector3(offset.x, 0f, 0f);
        }
        else if (Mathf.Abs(offset.y) > maxOffsetForClamp)
        {
            offset = new Vector3(0f, offset.y, 0f);
        }
        offset = new Vector3(-offset.x, offset.y, 0f);
        dollyY.m_Speed = lastMagnitude * (speedMultiplierY / 1000f) * offset.y;
        dollyX.m_Speed = lastMagnitude * (speedMultiplierX / 1000f) * offset.x;
    }

    private void MoveDollyCarts()
    {
        dollyY.m_Position += dollyY.m_Speed * Time.deltaTime;
        dollyX.m_Position += dollyX.m_Speed * Time.deltaTime;
    }

    private void ResetMovement()
    {
        dollyY.m_Speed = 0f;
        dollyX.m_Speed = 0f;
    }

    #endregion

    #region Clamping & Adaptive Speed

    private void ClampDollyX()
    {
        float normalizedY   = Mathf.Clamp01(dollyY.m_Position / maxYForXClamp);
        float maxDollyXAbs  = maxXRange * (1f - normalizedY);
        float center        = 0.5f;
        float minLimit      = center - maxDollyXAbs;
        float maxLimit      = center + maxDollyXAbs;

        // Compute where we'd clamp to:
        float targetX = Mathf.Clamp(dollyX.m_Position, minLimit, maxLimit);

        if (dollyX.m_Position > center + maxDollyXAbs)
        {
            dollyX.m_Speed = 0f;
            dollyX.m_Position = Mathf.SmoothDamp(
                dollyX.m_Position,
                targetX,
                ref clampXVel,
                clampSmoothTime
            );
        }
        else if (dollyX.m_Position < center - maxDollyXAbs)
        {
            dollyX.m_Speed = 0f;
            dollyX.m_Position = Mathf.SmoothDamp(
                dollyX.m_Position,
                targetX,
                ref clampXVel,
                clampSmoothTime
            );
            
        }
    }

    /// <summary>
    /// Adjusts horizontal dolly speed based on proximity to center: lerp between snapMinSpeed and snapMaxSpeed.
    /// </summary>
    private void AdaptDollyXSpeed()
    {
        float center = 0.5f;
        float dist = Mathf.Abs(dollyX.m_Position - center);
        if (dist < snapThreshold && IsDragging())
        {
            float t = dist / snapThreshold;
            float adaptiveSpeed = Mathf.Lerp(snapMinSpeed, snapMaxSpeed, t);
            // Direction: toward center
            dollyX.m_Speed *= adaptiveSpeed;
        }
    }

    #endregion
}
