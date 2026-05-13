using UnityEngine;
using UnityEngine.UI;

public class VehicleSelectorManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button blueCarBtn;
    [SerializeField] private Button grayCarBtn;
    [SerializeField] private Button purpleCarBtn;
    [SerializeField] private Button redCarBtn;
    [SerializeField] private Button backBtn;

    [Header("Prefabs")]
    [SerializeField] private GameObject blueCarPrefab;
    [SerializeField] private GameObject grayCarPrefab;
    [SerializeField] private GameObject purpleCarPrefab;
    [SerializeField] private GameObject redCarPrefab;

    private void Awake()
    {
        blueCarBtn.onClick.AddListener(() => SelectVehicle("BlueCar", blueCarPrefab));
        grayCarBtn.onClick.AddListener(() => SelectVehicle("GrayCar", grayCarPrefab));
        purpleCarBtn.onClick.AddListener(() => SelectVehicle("PurpleCar", purpleCarPrefab));
        redCarBtn.onClick.AddListener(() => SelectVehicle("RedCar", redCarPrefab));
        backBtn.onClick.AddListener(() => FindFirstObjectByType<MainMenuManager>()?.GoToLevelSelector());
    }

    private void OnDestroy()
    {
        blueCarBtn.onClick.RemoveAllListeners();
        grayCarBtn.onClick.RemoveAllListeners();
        purpleCarBtn.onClick.RemoveAllListeners();
        redCarBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
    }

    private void SelectVehicle(string vehicleName, GameObject prefab)
    {
        GameManager.Instance?.SetSelectedVehicle(vehicleName, prefab);
        GameManager.Instance?.StartGame();
    }
}