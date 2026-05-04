using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<WeaponSO> weapons;
    [SerializeField] private TurretController turret;
    [SerializeField] private Camera playerCamera;

    [Header("Rocket Config")]
    [SerializeField] private float rocketSpeed = 25f;
    [SerializeField] private int trajectoryResolution = 30;
    [SerializeField] private float maxRange = 80f;
    [SerializeField] private LayerMask hitLayer;

    [Header("Trajectory Preview")]
    [SerializeField] private float laserWidth = 0.05f;
    [SerializeField] private Material laserMaterial;
    private bool showTrajectory = true;
    private LineRenderer trajectoryLine;

    private int currentWeaponIndex;
    private float lastFireTime;
    public WeaponSO CurrentWeapon => weapons[currentWeaponIndex];
    private void Start()
    {
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.useWorldSpace = true;
        trajectoryLine.startWidth = laserWidth;
        trajectoryLine.endWidth = laserWidth * 0.1f;
        trajectoryLine.material = laserMaterial != null
            ? laserMaterial
            : new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.red;
        trajectoryLine.endColor = new Color(1f, 0f, 0f, 0f);
    }
    private void Update()
    {
        HandleWeaponSwitch();
        HandleFire();
        UpdateTrajectoryPreview();
    }
    private void HandleWeaponSwitch()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f || Keyboard.current.qKey.wasPressedThisFrame) CycleWeapon(1);
        else if (scroll < 0f) CycleWeapon(-1);
    }
    private void CycleWeapon(int dir) =>
        currentWeaponIndex = (currentWeaponIndex + dir + weapons.Count) % weapons.Count;
    private void HandleFire()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (Time.time < lastFireTime + CurrentWeapon.cooldown) return;

        lastFireTime = Time.time;

        if (CurrentWeapon.type == WeaponType.Raycast) FireRaycast();
        else FireRocket();
    }
    // instant shoot
    private void FireRaycast()
    {
        Vector3 firePos = turret.GetFirePoint();
        Vector3 fireDir = turret.GetFireDirection();

        Debug.DrawRay(firePos, fireDir * CurrentWeapon.range, Color.red, 1f);

        if (!Physics.Raycast(firePos, fireDir, out RaycastHit hit, CurrentWeapon.range, hitLayer)) return;

        Debug.Log($"Hit: {hit.collider.gameObject.name}");

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
            damageable.TakeDamage(CurrentWeapon.damage, hit.point);

        if (CurrentWeapon.impactEffect != null)
            Destroy(Instantiate(CurrentWeapon.impactEffect, hit.point,
                Quaternion.LookRotation(hit.normal)), 1f);
    }
    // shoot with a projectile that follows a parabolic trajectory, instantiates a prefab
    private void FireRocket()
    {
        if (CurrentWeapon.projectilePrefab == null) return;

        Vector3 origin = turret.GetFirePoint();
        Vector3 target = GetTargetPoint(origin);
        Vector3[] points = CalculateTrajectory(origin, target);

        Vector3 initialDir = (points[1] - points[0]).normalized;
        GameObject obj = Instantiate(CurrentWeapon.projectilePrefab, origin, Quaternion.LookRotation(initialDir));

        Collider[] carColliders = GetComponentsInChildren<Collider>();
        obj.GetComponent<PlayerRocket>()?.Initialize(points, rocketSpeed, CurrentWeapon.damage, carColliders);
    }
    private void UpdateTrajectoryPreview()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
            showTrajectory = !showTrajectory;

        if (!showTrajectory || CurrentWeapon == null)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;
        Vector3 origin = turret.GetFirePoint();

        if (CurrentWeapon.type == WeaponType.Raycast)
        {
            // Laser: rect line from turret to max range or hit point
            Vector3 fireDir = turret.GetFireDirection();
            Vector3 endPoint = Physics.Raycast(origin, fireDir, out RaycastHit hit, CurrentWeapon.range, hitLayer)
                ? hit.point
                : origin + fireDir * CurrentWeapon.range;

            trajectoryLine.positionCount = 2;
            trajectoryLine.SetPosition(0, origin);
            trajectoryLine.SetPosition(1, endPoint);
        }
        else
        {
            // Rocket: shows parabolic trajectory to the target point
            Vector3 target = GetTargetPoint(origin);
            Vector3[] points = CalculateTrajectory(origin, target);
            trajectoryLine.positionCount = points.Length;
            trajectoryLine.SetPositions(points);
        }
    }
    private Vector3[] CalculateTrajectory(Vector3 origin, Vector3 target)
    {
        Vector3[] points = new Vector3[trajectoryResolution];
        float distance = Vector3.Distance(origin, target);
        float arcHeight = Mathf.Clamp(distance * 0.25f, 3f, 12f);

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = (float)i / (trajectoryResolution - 1);
            Vector3 linearPoint = Vector3.Lerp(origin, target, t);
            float parabola = 4f * arcHeight * t * (1f - t);
            points[i] = linearPoint + Vector3.up * parabola;
        }
        return points;
    }
    // finds the closest enemy within range, if none found returns a point in the camera's forward direction
    private Vector3 GetTargetPoint(Vector3 origin)
    {
        Collider[] hits = Physics.OverlapSphere(origin, maxRange, hitLayer);
        float closestDist = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;
        bool foundTarget = false;

        foreach (Collider hit in hits)
        {
            AudienceEnemy enemy = hit.GetComponentInParent<AudienceEnemy>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(origin, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPoint = hit.transform.position + Vector3.up;
                foundTarget = true;
            }
        }
        if (foundTarget) return closestPoint;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, maxRange);

        foreach (RaycastHit rayHit in rayHits)
        {
            if (rayHit.collider.GetComponentInParent<CarController>() != null) continue;
            if (rayHit.collider.GetComponentInParent<CarStats>() != null) continue;
            return rayHit.point;
        }
        return ray.origin + ray.direction * maxRange;
    }
    public WeaponSO GetCurrentWeapon() => CurrentWeapon;
}