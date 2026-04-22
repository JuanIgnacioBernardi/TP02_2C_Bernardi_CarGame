using UnityEngine;

[CreateAssetMenu(fileName = "CarDataSO", menuName = "Scriptable Objects/CarDataSO")]
public class CarDataSO : ScriptableObject
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

    [Header("Physics")]
    public float downforceAmount = 200f;
    public float antiRollForce = 8000f;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f);

    [Header("Stats")]
    public float maxHealth = 100f;
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 1f;
}
