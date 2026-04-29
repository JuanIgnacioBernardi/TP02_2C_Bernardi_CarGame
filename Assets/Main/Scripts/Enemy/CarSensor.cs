using UnityEngine;

namespace Sensors
{
    [RequireComponent(typeof(SphereCollider))]
    public class CarSensor : MonoBehaviour
    {
        public delegate void CarEnterEvent(Transform car);
        public delegate void CarExitEvent(Vector3 lastKnownPosition);

        public event CarEnterEvent OnCarEnter;
        public event CarExitEvent OnCarExit;
        private void OnTriggerEnter(Collider other)
        {
            CarStats stats = other.GetComponentInParent<CarStats>();
            if (stats != null)
                OnCarEnter?.Invoke(stats.transform);
        }
        private void OnTriggerExit(Collider other)
        {
            CarStats stats = other.GetComponentInParent<CarStats>();
            if (stats != null)
                OnCarExit?.Invoke(other.transform.position);
        }
    }
}