using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Data")]
    [SerializeField] private CameraDataSO cameraData;

    [Header("Target")]
    public Transform target;
    public Transform cockpitPoint;

    private bool _isFirstPerson = false;
    private float _yaw;
    private float _pitch = 0f;

    private Vector3 _currentVelocity;
    private float _currentFOVVelocity;

    private Camera _cam;
    private Rigidbody _targetRb;
    void Start()
    {
        _cam = GetComponent<Camera>();
        if (target != null)
        {
            _targetRb = target.GetComponent<Rigidbody>();
            _yaw = target.eulerAngles.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        HandleToggle();
        HandleMouseLook();
        HandleFOV();
    }
    void LateUpdate()
    {
        if (target == null) return;
        if (_isFirstPerson) HandleFirstPerson();
        else HandleThirdPerson();
    }
    void HandleToggle()
    {
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
            _isFirstPerson = !_isFirstPerson;
    }
    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        _yaw += Mouse.current.delta.x.ReadValue() * cameraData.mouseSensitivity;
        _pitch -= Mouse.current.delta.y.ReadValue() * cameraData.mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, cameraData.minPitch, cameraData.maxPitch);
    }
    void HandleFirstPerson()
    {
        Transform anchor = cockpitPoint != null ? cockpitPoint : target;
        transform.position = anchor.position;
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
    void HandleThirdPerson()
    {
        // La cámara orbita según el mouse, sin auto-return
        Quaternion camRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 desiredPosition = target.position + camRotation * cameraData.offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _currentVelocity,
            cameraData.positionSmoothTime
        );

        // Siempre mira al auto
        Vector3 lookAt = target.position + Vector3.up * 1f;
        transform.rotation = Quaternion.LookRotation(lookAt - transform.position);
    }
    void HandleFOV()
    {
        if (_cam == null) return;
        float speed = _targetRb != null ? _targetRb.linearVelocity.magnitude : 0f;
        float targetFOV = _isFirstPerson
            ? cameraData.firstPersonFOV
            : cameraData.baseFOV + cameraData.maxFOVIncrease * Mathf.Clamp01(speed / cameraData.speedForMaxFOV);

        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, targetFOV, ref _currentFOVVelocity, cameraData.fovSmoothTime);
    }
}