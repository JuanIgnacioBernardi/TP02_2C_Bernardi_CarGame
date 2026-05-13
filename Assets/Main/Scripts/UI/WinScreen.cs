using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WinScreen : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip winSfx;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button mainMenuBtn;
    private void Start()
    {
        panel.SetActive(false);
        mainMenuBtn.onClick.AddListener(GoToMainMenu);

        if (GameManager.Instance != null)
            GameManager.Instance.OnRaceFinished += ShowWinScreen;
    }
    private void OnDestroy()
    {
        mainMenuBtn.onClick.RemoveAllListeners();
        if (GameManager.Instance != null)
            GameManager.Instance.OnRaceFinished -= ShowWinScreen;
    }
    private void ShowWinScreen()
    {
        AudioManager.Instance?.StopAll();
        AudioManager.Instance?.PlaySFX(winSfx);
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (scoreText != null)
            scoreText.text = "Score: " + ScoreSystem.Instance?.CurrentScore;
    }
    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.LoadMainMenu();
    }
}