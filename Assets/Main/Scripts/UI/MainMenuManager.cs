using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private Button competitionBtn;
    [SerializeField] private Button endlessBtn;
    private void Awake()
    {
        competitionBtn.onClick.AddListener(PlayCompetition);
        endlessBtn.onClick.AddListener(PlayEndless);

    }
    private void OnDestroy()
    {
        competitionBtn.onClick.RemoveAllListeners();
        endlessBtn.onClick.RemoveAllListeners();
    }
    public void PlayCompetition()
    {
        GameManager.Instance?.SetGameMode(GameMode.Competition);
        SceneManager.LoadScene(gameSceneName);
    }
    public void PlayEndless()
    {
        GameManager.Instance?.SetGameMode(GameMode.Endless);
        SceneManager.LoadScene(gameSceneName);
    }
    public void Quit() => Application.Quit();
}