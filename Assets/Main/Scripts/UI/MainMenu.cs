using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas references")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject settingsCanvas;

    [Header("GameObjects references")]
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject gamemodes;

    [Header("Buttons reference")]
    [SerializeField] private Button playBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingsBtnBack;

    private AudioManager audioManager;
    private void Awake()
    {
        playBtn.onClick.AddListener(PlayGame);
        settingsBtn.onClick.AddListener(OpenSettings);
        quitBtn.onClick.AddListener(QuitGame);
        settingsBtnBack.onClick.AddListener(CloseSettings);
        AddHoverSound(playBtn);
        AddHoverSound(settingsBtn);
        AddHoverSound(quitBtn);
        AddHoverSound(settingsBtnBack);
    }
    private void OnDestroy()
    {
        playBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        settingsBtnBack.onClick.RemoveAllListeners();

    }
    public void PlayGame()
    {
        main.SetActive(false);
        gamemodes.SetActive(true);
    }
    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void CloseSettings()
    {
        AudioManager.Instance.PlayUI(AudioManager.Instance.buttonUISfx);
        settingsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void GoToMain()
    {
        main.SetActive(true);
        gamemodes.SetActive(false);
    }
    private void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        entry.callback.AddListener
            (
                (eventData) => { AudioManager.Instance.PlayUI(AudioManager.Instance.hoverUISfx); ; }
            );

        trigger.triggers.Add(entry);
    }
}