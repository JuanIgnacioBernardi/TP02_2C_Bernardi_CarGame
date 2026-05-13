using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button mainMenuBtn;
    private void Start()
    {
        panel.SetActive(false);
        mainMenuBtn.onClick.AddListener(GoToMainMenu);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += ShowDeathScreen;
    }
    private void OnDestroy()
    {
        mainMenuBtn.onClick.RemoveAllListeners();
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowDeathScreen;
    }
    private void ShowDeathScreen()
    {
        StartCoroutine(WaitForBarThenShow());
    }
    private IEnumerator WaitForBarThenShow()
    {
        yield return new WaitForSeconds(0.9f);

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