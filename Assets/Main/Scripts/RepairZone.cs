using UnityEngine;

public class RepairZone : MonoBehaviour
{
    public float repairRate = 5f;

    private CarStats _carInside;
    private bool _fullyInside;
    private Collider _zoneCollider;
    private void Awake()
    {
        _zoneCollider = GetComponent<Collider>();
    }
    private void Update()
    {
        if (_fullyInside && _carInside != null)
            _carInside.Repair(repairRate * Time.deltaTime);
    }
    private void OnTriggerStay(Collider other)
    {
        var stats = other.GetComponentInParent<CarStats>();
        if (stats != null)
        {
            _carInside = stats;
            _fullyInside = IsFullyInside(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CarStats>() != null)
        {
            _carInside = null;
            _fullyInside = false;
        }
    }
    private bool IsFullyInside(Collider other)
    {
        Bounds zone = _zoneCollider.bounds;
        Bounds car = other.bounds;

        return zone.Contains(car.min) && zone.Contains(car.max);
    }
}