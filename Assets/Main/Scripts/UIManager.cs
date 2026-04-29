using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image fillHealthBar;
    [SerializeField] private float healthLerpSpeed = 5f;
    private float targetHealthFill;

    [Header("Fuel Bar")]
    [SerializeField] private Image fillTankBar;
    [SerializeField] private float fuelLerpSpeed = 3f;
    private float targetFuelFill;

    [Header("Speedometer")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private RectTransform speedometerNeedle;
    [SerializeField] private float maxSpeedForNeedle = 50f;
    [SerializeField] private float needleMinAngle = 135f;
    [SerializeField] private float needleMaxAngle = -135f;

    private CarStats stats;
    private CarController carController;
    private void Awake()
    {
        stats = FindFirstObjectByType<CarStats>();
        carController = FindFirstObjectByType<CarController>();

        if (stats != null)
        {
            stats.OnHealthChanged += SetHealthTarget;
            stats.OnFuelChanged += SetFuelTarget;
        }
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
    }
    private void OnDisable()
    {
        if (stats != null)
        {
            stats.OnHealthChanged -= SetHealthTarget;
            stats.OnFuelChanged -= SetFuelTarget;
        }
    }
    private void Update()
    {
        if (fillHealthBar != null)
        {
            fillHealthBar.fillAmount = Mathf.Lerp(fillHealthBar.fillAmount, targetHealthFill, Time.deltaTime * healthLerpSpeed);
        }

        if (fillTankBar != null)
        {
            fillTankBar.fillAmount = Mathf.Lerp(fillTankBar.fillAmount, targetFuelFill, Time.deltaTime * fuelLerpSpeed);
        }
        UpdateSpeedometer();
    }
    private void SetHealthTarget(float maxHealth, float currentHealth)
    {
        targetHealthFill = currentHealth / maxHealth;
    }

    private void SetFuelTarget(float maxFuel, float currentFuel)
    {
        targetFuelFill = currentFuel / maxFuel;
    }
    private void UpdateSpeedometer()
    {
        if (carController == null) return;

        // m/s to km/h
        float speedKmh = carController.CurrentSpeed * 3.6f;

        if (speedText != null)
            speedText.text = Mathf.RoundToInt(speedKmh).ToString() + " km/h";

        if (speedometerNeedle != null)
        {
            float t = Mathf.Clamp01(speedKmh / maxSpeedForNeedle);
            float angle = Mathf.Lerp(needleMinAngle, needleMaxAngle, t);
            speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
