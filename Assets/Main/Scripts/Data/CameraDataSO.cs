using UnityEngine;

[CreateAssetMenu(fileName = "CameraDataSO", menuName = "Scriptable Objects/CameraDataSO")]
public class CameraDataSO : ScriptableObject
{
    [Header("Third Person")]
    public Vector3 offset = new Vector3(0f, 3f, -7f);
    public float positionSmoothTime = 0.15f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;
    public float maxPitch = 40f;

    [Header("Field of View")]
    public float baseFOV = 60f;
    public float firstPersonFOV = 80f;
    public float maxFOVIncrease = 15f;
    public float fovSmoothTime = 0.3f;
    public float speedForMaxFOV = 50f;
}
