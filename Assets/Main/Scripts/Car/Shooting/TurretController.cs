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
        if (playerCamera == null) return;

        Vector3 targetPoint = playerCamera.transform.position + playerCamera.transform.forward * 100f;

        Vector3 dirToTarget = targetPoint - horizontalPivot.position;
        dirToTarget.y = 0f;
        if (dirToTarget != Vector3.zero)
        {
            Quaternion worldRot = Quaternion.LookRotation(dirToTarget);
            Quaternion localRot = Quaternion.Inverse(horizontalPivot.parent.rotation) * worldRot;
            horizontalPivot.localRotation = Quaternion.Slerp(
                horizontalPivot.localRotation,
                localRot,
                rotationSpeed * Time.deltaTime);
        }

        // Vertical — igual que antes
        Vector3 localDir = verticalPivot.parent.InverseTransformPoint(targetPoint);
        float angleX = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        angleX = Mathf.Clamp(angleX, minVerticalAngle, maxVerticalAngle);
        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            Quaternion.Euler(-angleX, 0f, 0f),
            rotationSpeed * Time.deltaTime);
    }
    public Vector3 GetFirePoint() => firePoint.position;
    public Vector3 GetFireDirection() => firePoint.forward;
    public void SetCamera(Camera cam) => playerCamera = cam;
}
