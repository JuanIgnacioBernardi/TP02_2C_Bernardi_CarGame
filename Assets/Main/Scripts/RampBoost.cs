using UnityEngine;

public class RampBoost : MonoBehaviour
{
    [SerializeField] private float boostForce = 4000f;
    [SerializeField] private bool useBoost = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!useBoost) return;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
            rb.AddForce(other.transform.root.forward * boostForce, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.up * 3f);
    }
}