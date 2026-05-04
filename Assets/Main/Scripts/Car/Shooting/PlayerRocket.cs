using UnityEngine;

public class PlayerRocket : MonoBehaviour
{
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float explosionRadius = 3f;

    private Vector3[] trajectoryPoints;
    private int currentPoint;
    private float speed;
    private float damage;
    private bool isInitialized;
    private bool hasExploded;

    private Collider[] collidersToIgnore;

    public void Initialize(Vector3[] points, float rocketSpeed, float dmg, Collider[] toIgnore)
    {
        trajectoryPoints = points;
        speed = rocketSpeed;
        damage = dmg;
        currentPoint = 0;
        isInitialized = true;
        hasExploded = false;
        collidersToIgnore = toIgnore;

        // Ignore car collisions
        Collider myCol = GetComponent<Collider>();
        if (myCol != null)
            foreach (var col in toIgnore)
                if (col != null) Physics.IgnoreCollision(myCol, col, true);
    }
    private void Update()
    {
        if (!isInitialized || hasExploded) return;

        while (currentPoint < trajectoryPoints.Length)
        {
            Vector3 target = trajectoryPoints[currentPoint];
            float step = speed * Time.deltaTime;
            float distToTarget = Vector3.Distance(transform.position, target);

            if (step >= distToTarget)
            {
                transform.position = target;
                currentPoint++;
            }
            else
            {
                Vector3 dir = (target - transform.position).normalized;
                transform.position += dir * step;
                transform.forward = dir;
                break;
            }
        }
        if (currentPoint >= trajectoryPoints.Length)
            OnImpact(transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized || hasExploded) return;
        if (other.isTrigger) return;

        if (collidersToIgnore != null)
            foreach (var col in collidersToIgnore)
                if (col == other) return;

        OnImpact(transform.position);
    }
    private void OnImpact(Vector3 pos)
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius);
        float closestDist = Mathf.Infinity;
        IDamageable closestTarget = null;

        foreach (Collider hit in hits)
        {
            if (hit.GetComponentInParent<CarController>() != null) continue;
            if (hit.GetComponentInParent<CarStats>() != null) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead) continue;

            float dist = Vector3.Distance(pos, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = damageable;
            }
        }
        closestTarget?.TakeDamage(damage, pos);

        if (impactEffect != null)
            Destroy(Instantiate(impactEffect, pos, Quaternion.identity), 1.5f);

        // Return to pool
        PoolManager.Instance?.ReturnRocket(this);
    }

    // OnDisable To reset state when returned to pool
    private void OnDisable()
    {
        isInitialized = false;
        hasExploded = false;
        currentPoint = 0;
    }
}