using UnityEngine;

public class ExplosiveMine : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private bool respawn = true;
    [SerializeField] private float respawnTime = 10f;

    private MeshRenderer visual;
    private Collider col;
    private bool exploded;
    private void Awake()
    {
        visual = GetComponentInChildren<MeshRenderer>();
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (exploded) return;

        CarStats stats = other.GetComponentInParent<CarStats>();
        if (stats == null) return;

        exploded = true;
        stats.TakeDamage(damage, transform.position);

        if (explosionEffect != null)
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 2f);

        visual.enabled = false;
        col.enabled = false;

        if (respawn) Invoke(nameof(ResetMine), respawnTime);
        else Destroy(gameObject, 0.1f);
    }
    private void ResetMine()
    {
        exploded = false;
        visual.enabled = true;
        col.enabled = true;
    }
}