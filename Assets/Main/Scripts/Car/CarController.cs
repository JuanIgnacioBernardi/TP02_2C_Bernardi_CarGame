using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Wheels - Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheels - Transforms")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Config")]
    [SerializeField]private CarDataSO data;

    private float horizontalInput;
    private float verticalInput;
    private float smoothedSteering;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool isBraking;
    private Rigidbody rb;
    private CarStats stats;

    public float CurrentSpeed => rb != null ? rb.linearVelocity.magnitude : 0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<CarStats>();
        rb.centerOfMass += data.centerOfMassOffset;
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        ApplyDownforce();
        ApplyAntiRoll();
        LimitAngularVelocity();
        UpdateWheels();
    }
    private void GetInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        verticalInput = 0f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            verticalInput = 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            verticalInput = -1f;

        horizontalInput = 0f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            horizontalInput = 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            horizontalInput = -1f;

        isBraking = keyboard.spaceKey.isPressed;
    }
    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * data.motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * data.motorForce;

        if (verticalInput == 0f && !isBraking)
        {
            frontLeftWheelCollider.brakeTorque = data.engineBraking;
            frontRightWheelCollider.brakeTorque = data.engineBraking;
            rearLeftWheelCollider.brakeTorque = data.engineBraking;
            rearRightWheelCollider.brakeTorque = data.engineBraking;
        }
        else
        {
            currentBrakeForce = isBraking ? data.brakeForce : 0f;
            ApplyBraking();
        }
    }
    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }
    private void HandleSteering()
    {
        float speed = rb.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(speed / data.speedForMinSteering);
        float dynamicAngle = Mathf.Lerp(data.maxSteeringAngle, data.minSteeringAngle, speedRatio);

        smoothedSteering = Mathf.Lerp(smoothedSteering, horizontalInput, Time.fixedDeltaTime * data.steerSmoothSpeed);

        currentSteeringAngle = dynamicAngle * smoothedSteering;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }
    private void ApplyDownforce()
    {
        float speed = rb.linearVelocity.magnitude;
        rb.AddForce(-transform.up * data.downforceAmount * speed);
    }
    private void ApplyAntiRoll()
    {
        ApplyAntiRollBar(frontLeftWheelCollider, frontRightWheelCollider);
        ApplyAntiRollBar(rearLeftWheelCollider, rearRightWheelCollider);
    }
    private void ApplyAntiRollBar(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;

        float travelLeft = 1f;
        float travelRight = 1f;

        bool leftGrounded = leftWheel.GetGroundHit(out hit);
        if (leftGrounded)
            travelLeft = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;

        bool rightGrounded = rightWheel.GetGroundHit(out hit);
        if (rightGrounded)
            travelRight = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;

        float force = (travelLeft - travelRight) * data.antiRollForce;

        if (leftGrounded)
            rb.AddForceAtPosition(leftWheel.transform.up * -force, leftWheel.transform.position);
        if (rightGrounded)
            rb.AddForceAtPosition(rightWheel.transform.up * force, rightWheel.transform.position);
    }
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    private void LimitAngularVelocity()
    {
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, 2f);
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }
}