using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CameraFollow cameraFollow;
    private void Start()
    {
        GameObject prefab = GameManager.Instance?.SelectedVehiclePrefab;
        if (prefab == null) return;

        GameObject car = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // Connects camera with the car
        cameraFollow?.SetTarget(car.transform);

        // Connects camera with pweapon
        PlayerWeapon weapon = car.GetComponentInChildren<PlayerWeapon>();
        if (weapon != null)
            weapon.SetCamera(playerCamera);

        // Connects camera with TController
        TurretController turret = car.GetComponentInChildren<TurretController>();
        if (turret != null)
            turret.SetCamera(playerCamera);

        UIManager ui = FindFirstObjectByType<UIManager>();
        CarStats stats = car.GetComponentInChildren<CarStats>();

        if (stats != null)
        {
            ui?.SetCarStats(stats);
            stats.OnDeath += () => GameManager.Instance?.TriggerGameOver();
        }
        // Connect carcontroller to uimanager to use speedometer
        CarController controller = car.GetComponentInChildren<CarController>();
        if (ui != null && controller != null)
            ui.SetCarController(controller);

        stats.OnDeath += () =>
        {
            car.GetComponentInChildren<CarEngineAudio>()?.StopEngine();
        };
        CheckpointManager.Instance?.SetCar(stats);
    }
}