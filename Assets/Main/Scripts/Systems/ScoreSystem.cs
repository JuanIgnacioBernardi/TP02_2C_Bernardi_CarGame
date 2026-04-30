using UnityEngine;
using System;
public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }
    public event Action<int> OnScoreChanged;

    private int currentScore;
    public int CurrentScore => currentScore;

    private void Awake() => Instance = this;
    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }
    public void Penalize(int amount)
    {
        currentScore -= amount;
        OnScoreChanged?.Invoke(currentScore);
    }
}