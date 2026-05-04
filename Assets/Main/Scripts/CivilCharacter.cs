using UnityEngine;

public class CivilCharacter : Character, IScoreable
{
    [Header("Civil Config")]
    [SerializeField] private int penaltyScore = 200;
    public int ScoreValue => penaltyScore;
    protected override void OnDeath()
    {
        ScoreSystem.Instance?.Penalize(penaltyScore);
        Destroy(gameObject);
    }
}