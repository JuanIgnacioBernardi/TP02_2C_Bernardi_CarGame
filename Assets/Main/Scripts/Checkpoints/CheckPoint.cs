using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] public int index;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CarStats>() != null)
            CheckpointManager.Instance?.ReachCheckpoint(index);
    }
}