using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Motor")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float engineBraking = 300f;

    [Header("Steering")]
    public float maxSteeringAngle = 45f;
    public float minSteeringAngle = 10f;
    public float speedForMinSteering = 30f;
    public float steerSmoothSpeed = 8f;

    [Header("Downforce")]
    public float downforceAmount = 100f;

    [Header("Anti-Roll")]
    public float antiRollForce = 5000f;

    [Header("Setup")]
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f);

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

    private float horizontalInput;
    private float verticalInput;
    private float smoothedSteering;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool isBraking;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
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
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;

        if (verticalInput == 0f && !isBraking)
        {
            frontLeftWheelCollider.brakeTorque = engineBraking;
            frontRightWheelCollider.brakeTorque = engineBraking;
            rearLeftWheelCollider.brakeTorque = engineBraking;
            rearRightWheelCollider.brakeTorque = engineBraking;
        }
        else
        {
            currentBrakeForce = isBraking ? brakeForce : 0f;
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
        float speedRatio = Mathf.Clamp01(speed / speedForMinSteering);
        float dynamicAngle = Mathf.Lerp(maxSteeringAngle, minSteeringAngle, speedRatio);

        smoothedSteering = Mathf.Lerp(smoothedSteering, horizontalInput, Time.fixedDeltaTime * steerSmoothSpeed);

        currentSteeringAngle = dynamicAngle * smoothedSteering;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }
    private void ApplyDownforce()
    {
        float speed = rb.linearVelocity.magnitude;
        rb.AddForce(-transform.up * downforceAmount * speed);
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

        float force = (travelLeft - travelRight) * antiRollForce;

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