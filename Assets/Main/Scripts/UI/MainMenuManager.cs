using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gamemodesPanel;
    [SerializeField] private GameObject levelSelectorPanel;
    [SerializeField] private GameObject vehicleSelectorPanel;

    [Header("Buttons — GameModes")]
    [SerializeField] private Button competitionBtn;
    [SerializeField] private Button endlessBtn;
    [SerializeField] private Button gamemodesBackBtn;

    [Header("Buttons — LevelSelector")]
    [SerializeField] private Button levelBackBtn;

    [Header("Buttons — VehicleSelector")]
    [SerializeField] private Button vehicleBackBtn;

    private void Awake()
    {
        competitionBtn.onClick.AddListener(OnCompetition);
        endlessBtn.onClick.AddListener(OnEndless);
        gamemodesBackBtn.onClick.AddListener(() => FindFirstObjectByType<MainMenu>()?.GoToMain());
        levelBackBtn.onClick.AddListener(() => ShowPanel(gamemodesPanel));
        vehicleBackBtn.onClick.AddListener(() => ShowPanel(levelSelectorPanel));
    }
    private void OnDestroy()
    {
        competitionBtn.onClick.RemoveAllListeners();
        endlessBtn.onClick.RemoveAllListeners();
        gamemodesBackBtn.onClick.RemoveAllListeners();
        levelBackBtn.onClick.RemoveAllListeners();
        vehicleBackBtn.onClick.RemoveAllListeners();
    }
    private void OnCompetition()
    {
        GameManager.Instance?.SetGameMode(GameMode.Competition);
        ShowPanel(levelSelectorPanel);
    }
    private void OnEndless()
    {
        GameManager.Instance?.SetGameMode(GameMode.Endless);
        ShowPanel(levelSelectorPanel);
    }
    private void ShowPanel(GameObject panel)
    {
        gamemodesPanel.SetActive(false);
        levelSelectorPanel.SetActive(false);
        vehicleSelectorPanel.SetActive(false);
        if (panel != null) panel.SetActive(true);
    }
    public void GoToVehicleSelector() => ShowPanel(vehicleSelectorPanel);
    public void GoToGamemodes() => ShowPanel(gamemodesPanel);
    public void GoToLevelSelector() => ShowPanel(levelSelectorPanel);
}