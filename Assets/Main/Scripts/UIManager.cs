using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private CarStats stats;
    private CarController carController;

    [Header("Health Bar")]
    [SerializeField] private Image fillHealthBar;
    [SerializeField] private float healthLerpSpeed = 5f;
    private float targetHealthFill = 1f;

    [Header("Fuel Bar")]
    [SerializeField] private Image fillTankBar;
    [SerializeField] private float fuelLerpSpeed = 3f;
    private float targetFuelFill = 1f;

    [Header("Speedometer")]
    [SerializeField] private RectTransform speedometerNeedle;
    [SerializeField] private float maxSpeedKmh = 220f;
    [SerializeField] private float needleMinAngle = -135f;
    [SerializeField] private float needleMaxAngle = 45f;

    [Header("Race Info")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lapText;
    private void Awake()
    {
        if (stats != null)
        {
            stats.OnHealthChanged += SetHealthTarget;
            stats.OnFuelChanged += SetFuelTarget;
        }

        if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.OnScoreChanged += UpdateScore;

        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.OnLapCompleted += UpdateLap;
    }
    private void Start()
    {
        if (stats != null)
        {
            SetHealthTarget(stats.config.maxHealth, stats.CurrentHealth);
            SetFuelTarget(stats.config.maxFuel, stats.CurrentFuel);
            if (fillHealthBar != null) fillHealthBar.fillAmount = targetHealthFill;
            if (fillTankBar != null) fillTankBar.fillAmount = targetFuelFill;
        }

        UpdateScore(0);
        if (CheckpointManager.Instance != null)
            UpdateLap(0, CheckpointManager.Instance.TotalLaps);
    }
    private void OnDisable()
    {
        if (stats != null)
        {
            stats.OnHealthChanged -= SetHealthTarget;
            stats.OnFuelChanged -= SetFuelTarget;
        }
        if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.OnScoreChanged -= UpdateScore;
        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.OnLapCompleted -= UpdateLap;
    }
    private void Update()
    {
        if (fillHealthBar != null)
            fillHealthBar.fillAmount = Mathf.Lerp(fillHealthBar.fillAmount, targetHealthFill, Time.deltaTime * healthLerpSpeed);
        if (fillTankBar != null)
            fillTankBar.fillAmount = Mathf.Lerp(fillTankBar.fillAmount, targetFuelFill, Time.deltaTime * fuelLerpSpeed);

        UpdateSpeedometer();
    }
    private void SetHealthTarget(float max, float current) => targetHealthFill = current / max;
    private void SetFuelTarget(float max, float current) => targetFuelFill = current / max;

    private void UpdateSpeedometer()
    {
        if (carController == null) return;
        float kmh = carController.CurrentSpeed * 3.6f;

        if (speedometerNeedle != null)
        {
            float t = Mathf.Clamp01(kmh / maxSpeedKmh);
            speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(needleMinAngle, needleMaxAngle, t));
        }
    }
    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    private void UpdateLap(int current, int total)
    {
        if (lapText == null) return;

        if (GameManager.Instance?.CurrentMode == GameMode.Endless)
            lapText.text = $"Vuelta: {current}";
        else
            lapText.text = $"Vuelta: {current} / {total}";
    }
    public void SetCarStats(CarStats newStats)
    {
        if (stats != null)
        {
            stats.OnHealthChanged -= SetHealthTarget;
            stats.OnFuelChanged -= SetFuelTarget;
        }
        stats = newStats;
        if (stats != null)
        {
            stats.OnHealthChanged += SetHealthTarget;
            stats.OnFuelChanged += SetFuelTarget;

            SetHealthTarget(stats.config.maxHealth, stats.CurrentHealth);
            SetFuelTarget(stats.config.maxFuel, stats.CurrentFuel);
            if (fillHealthBar != null) fillHealthBar.fillAmount = targetHealthFill;
            if (fillTankBar != null) fillTankBar.fillAmount = targetFuelFill;
        }
    }
    public void SetCarController(CarController controller)
    {
        carController = controller;
    }
}