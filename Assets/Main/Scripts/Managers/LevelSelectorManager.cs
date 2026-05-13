using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorManager : MonoBehaviour
{
    [SerializeField] private Button track01Btn;
    [SerializeField] private Button track02Btn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button nextBtn;
    private void Awake()
    {
        track01Btn.onClick.AddListener(() => SelectLevel("Track01"));
        track02Btn.onClick.AddListener(() => SelectLevel("Track02"));
        backBtn.onClick.AddListener(() => FindFirstObjectByType<MainMenuManager>()?.GoToGamemodes());
        nextBtn.onClick.AddListener(() => FindFirstObjectByType<MainMenuManager>()?.GoToVehicleSelector());
    }
    private void OnDestroy()
    {
        track01Btn.onClick.RemoveAllListeners();
        track02Btn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
    }
    private void SelectLevel(string sceneName)
    {
        GameManager.Instance?.SetSelectedLevel(sceneName);
    }
}