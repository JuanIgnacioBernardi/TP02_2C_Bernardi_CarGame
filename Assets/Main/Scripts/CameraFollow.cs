using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0f, 3f, -7f);
    public float positionSmoothTime = 0.15f;

    [Header("Rotation Settings")]
    public float rotationSmoothTime = 0.1f;
    public float pitchAngle = 10f; // downward tilt

    [Header("Look Ahead")]
    public float lookAheadDistance = 4f;  // how much does the camera "advance"
    public float lookAheadSmoothTime = 0.3f;

    [Header("Field of View")]
    public float baseFOV = 60f;
    public float maxFOVIncrease = 15f;    // Extra FOV at high speed
    public float fovSmoothTime = 0.3f;
    public float speedForMaxFOV = 50f;    // speed (m/s) for maximum FOV

    private Vector3 _currentVelocity;
    private float _currentRotationVelocity;
    private Vector3 _lookAheadVelocity;
    private Vector3 _currentLookAhead;
    private float _currentFOVVelocity;
    private Camera _cam;
    private Rigidbody _targetRb;

    void Start()
    {
        _cam = GetComponent<Camera>();
        if (target != null)
            _targetRb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandlePosition();
        HandleRotation();
        HandleFOV();
    }

    void HandlePosition()
    {
        // Look ahead: The camera moves forward in the direction the car is traveling.
        Vector3 lookAheadTarget = target.forward * lookAheadDistance;
        _currentLookAhead = Vector3.SmoothDamp(
            _currentLookAhead,
            lookAheadTarget,
            ref _lookAheadVelocity,
            lookAheadSmoothTime
        );
        // Desired position = behind the car + offset + look ahead
        Vector3 desiredPosition = target.TransformPoint(offset) + _currentLookAhead;

        // Suavizado de posición
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _currentVelocity,
            positionSmoothTime
        );
    }
    void HandleRotation()
    {
        // We looked towards the car + a little look ahead
        Vector3 lookAtTarget = target.position + _currentLookAhead;
        Vector3 direction = lookAtTarget - transform.position;

        if (direction == Vector3.zero) return;

        Quaternion desiredRotation = Quaternion.LookRotation(direction);

        // We apply a fixed pitch downwards
        desiredRotation *= Quaternion.Euler(pitchAngle, 0f, 0f);

        // Rotation smoothing
        float currentY = transform.eulerAngles.y;
        float desiredY = desiredRotation.eulerAngles.y;
        float smoothedY = Mathf.SmoothDampAngle(currentY, desiredY, ref _currentRotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(
            desiredRotation.eulerAngles.x,
            smoothedY,
            0f
        );
    }
    void HandleFOV()
    {
        if (_cam == null) return;

        // We calculate the car's current speed.
        float speed = _targetRb != null ? _targetRb.linearVelocity.magnitude : 0f;
        float speedRatio = Mathf.Clamp01(speed / speedForMaxFOV);

        float desiredFOV = baseFOV + (maxFOVIncrease * speedRatio);
        _cam.fieldOfView = Mathf.SmoothDamp(
            _cam.fieldOfView,
            desiredFOV,
            ref _currentFOVVelocity,
            fovSmoothTime
        );
    }
}