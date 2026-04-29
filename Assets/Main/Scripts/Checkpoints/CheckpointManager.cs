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

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < checkpoints.Length; i++)
            checkpoints[i].index = i;
    }
    public void ReachCheckpoint(int index)
    {
        if (index != nextCheckpoint) return;

        nextCheckpoint++;

        if (nextCheckpoint >= checkpoints.Length)
        {
            nextCheckpoint = 0;
            currentLap++;
            OnLapCompleted?.Invoke(currentLap, totalLaps);

            if (currentLap >= totalLaps)
                OnRaceFinished?.Invoke();
        }
    }
}