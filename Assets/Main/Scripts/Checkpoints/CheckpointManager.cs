using UnityEngine;
using System;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private int totalLaps = 3;

    public event Action<int, int> OnLapCompleted;
    public event Action OnRaceFinished;

    private int nextCheckpoint;
    private int currentLap;

    public int CurrentLap => currentLap;
    public int TotalLaps => totalLaps;

    private Vector3 lastCheckpointPosition;
    private Vector3 lastCarForward;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < checkpoints.Length; i++)
            checkpoints[i].index = i;

        lastCheckpointPosition = checkpoints[0].transform.position;
        lastCarForward = Vector3.forward;
    }
    public void ReachCheckpoint(int index)
    {
        if (index != nextCheckpoint) return;

        lastCheckpointPosition = checkpoints[index].transform.position;

        CarStats car = FindFirstObjectByType<CarStats>();
        if (car != null)
            lastCarForward = car.transform.forward;

        nextCheckpoint++;

        if (nextCheckpoint >= checkpoints.Length)
        {
            nextCheckpoint = 0;
            currentLap++;
            OnLapCompleted?.Invoke(currentLap, totalLaps);

            if (GameManager.Instance?.CurrentMode == GameMode.Competition && currentLap >= totalLaps)
                GameManager.Instance?.TriggerRaceFinished();
        }
    }
    public void RespawnAtLastCheckpoint(Rigidbody rb)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.transform.position = lastCheckpointPosition + Vector3.up * 1f;

        Vector3 flatForward = lastCarForward;
        flatForward.y = 0f;
        if (flatForward == Vector3.zero) flatForward = Vector3.forward;
        rb.transform.rotation = Quaternion.LookRotation(flatForward);
    }
}