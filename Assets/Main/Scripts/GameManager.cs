using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentMode { get; private set; }

    public event Action OnGameOver;
    public event Action OnRaceFinished;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void SetGameMode(GameMode mode) => CurrentMode = mode;

    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Invoke(nameof(LoadMainMenu), 3f);
    }
    public void TriggerRaceFinished()
    {
        OnRaceFinished?.Invoke();
        Invoke(nameof(LoadMainMenu), 3f);
    }
    private void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
}