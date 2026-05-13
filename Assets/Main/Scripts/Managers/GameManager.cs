using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject SelectedVehiclePrefab { get; private set; }
    public GameMode CurrentMode { get; private set; }
    public string SelectedLevel { get; private set; }
    public string SelectedVehicle { get; private set; }

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
    public void SetSelectedLevel(string sceneName) => SelectedLevel = sceneName;
    public void SetSelectedVehicle(string vehicleName, GameObject prefab)
    {
        SelectedVehicle = vehicleName;
        SelectedVehiclePrefab = prefab;
    }
    public void StartGame()
    {
        if (string.IsNullOrEmpty(SelectedLevel)) return;
        SceneManager.LoadScene(SelectedLevel);
    }
    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }
    public void TriggerRaceFinished()
    {
        OnRaceFinished?.Invoke();
    }
    public void LoadMainMenu()
    {
        CancelInvoke();
        SceneManager.LoadScene("MainMenu");
    }
}