using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform horizontalPivot;
    [SerializeField] private Transform verticalPivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera playerCamera;

    [Header("Config")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 80f;
    private void Update()
    {
        AimAtCameraDirection();
    }
    private void AimAtCameraDirection()
    {
        Vector3 targetPoint = playerCamera.transform.position + playerCamera.transform.forward * 100f;

        Vector3 dirToTarget = targetPoint - horizontalPivot.position;
        dirToTarget.y = 0f;
        if (dirToTarget != Vector3.zero)
        {
            Quaternion targetHRot = Quaternion.LookRotation(dirToTarget);
            horizontalPivot.rotation = Quaternion.Slerp(
                horizontalPivot.rotation,
                targetHRot,
                rotationSpeed * Time.deltaTime);
        }
        Vector3 localDir = verticalPivot.parent.InverseTransformDirection(targetPoint - verticalPivot.position);
        float angleX = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        angleX = Mathf.Clamp(angleX, minVerticalAngle, maxVerticalAngle);
        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            Quaternion.Euler(-angleX, 0f, 0f),
            rotationSpeed * Time.deltaTime);
    }
    public Vector3 GetFirePoint() => firePoint.position;
    public Vector3 GetFireDirection() => firePoint.forward;
}
