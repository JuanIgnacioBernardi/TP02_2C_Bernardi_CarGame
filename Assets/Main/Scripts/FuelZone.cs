using UnityEngine;

public class FuelZone : MonoBehaviour
{
    public float refuelRate = 10f;

    private CarStats _carInside;
    private bool _fullyInside;

    private void Update()
    {
        if (_fullyInside && _carInside != null)
            _carInside.Refuel(refuelRate * Time.deltaTime);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CarStats>(out var stats))
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
        Bounds zone = GetComponent<Collider>().bounds;
        return zone.Contains(other.bounds.min) && zone.Contains(other.bounds.max);
    }
}