using UnityEngine;

public class ParabolicProjectile : MonoBehaviour
{
    [SerializeField] private float arcHeight = 4f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private GameObject impactEffect;

    private Vector3 startPos, endPos;
    private float elapsed, damage;
    private bool launched, hasHit;

    public void Launch(Vector3 from, Vector3 to, float dmg)
    {
        startPos = from;
        endPos = to;
        damage = dmg;
        elapsed = 0f;
        launched = true;
        hasHit = false;
        transform.position = from;
    }
    private void Update()
    {
        if (!launched || hasHit) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        Vector3 linearPos = Vector3.Lerp(startPos, endPos, t);
        float height = arcHeight * Mathf.Sin(Mathf.PI * t);
        transform.position = linearPos + Vector3.up * height;

        float tNext = Mathf.Clamp01((elapsed + Time.deltaTime) / duration);
        Vector3 nextPos = Vector3.Lerp(startPos, endPos, tNext) + Vector3.up * (arcHeight * Mathf.Sin(Mathf.PI * tNext));
        Vector3 dir = nextPos - transform.position;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);

        if (t >= 1f)
        {
            if (!hasHit)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
                foreach (Collider hit in hits)
                {
                    CarStats stats = hit.GetComponentInParent<CarStats>();
                    if (stats != null)
                    {
                        stats.TakeDamage(damage, transform.position);
                        break;
                    }
                }
            }
            Impact(transform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!launched || hasHit) return;
        if (other.isTrigger) return;
        if (other.GetComponentInParent<CarController>() != null) return;
        if (other.GetComponentInParent<AudienceEnemy>() != null) return;

        Debug.Log($"Proyectil tocó: {other.gameObject.name}");

        CarStats stats = other.GetComponentInParent<CarStats>();
        Debug.Log($"CarStats encontrado: {stats != null}");

        if (stats != null)
        {
            stats.TakeDamage(damage, transform.position);
            Impact(transform.position);
        }
    }
    private void Impact(Vector3 pos)
    {
        hasHit = true;
        launched = false;
        if (impactEffect != null)
            Destroy(Instantiate(impactEffect, pos + Vector3.up * 0.3f, Quaternion.identity), 1.5f);

        // Returns the projectile to the pool
        PoolManager.Instance?.ReturnAudienceProjectile(this);
    }
    private void OnDisable()
    {
        launched = false;
        hasHit = false;
        elapsed = 0f;
    }
}