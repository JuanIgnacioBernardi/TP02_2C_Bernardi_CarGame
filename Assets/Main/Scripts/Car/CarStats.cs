using UnityEngine;
using System;

public class CarStats : MonoBehaviour, IDamageable
{
    public CarDataSO config;

    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float CurrentFuel { get; private set; }

    public bool IsDead => CurrentHealth <= 0f;

    public event Action<float, float> OnHealthChanged; // maxHealth, currentHealth
    public event Action<float, Vector3> OnDamageReceived;
    public event Action<float, float> OnFuelChanged; // maxFuel, currentFuel
    public event Action OnDeath;

    private void Start()
    {
        CurrentHealth = config.maxHealth;   
        CurrentFuel = config.maxFuel;

        OnHealthChanged?.Invoke(config.maxHealth, CurrentHealth);
        OnFuelChanged?.Invoke(config.maxFuel, CurrentFuel);
    }
    public void TakeDamage(float amount, Vector3 position)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, config.maxHealth);
        OnHealthChanged?.Invoke(config.maxHealth, CurrentHealth);
        OnDamageReceived?.Invoke(amount, position);
        if (IsDead) OnDeath?.Invoke();
    }
    public void Repair(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, config.maxHealth);
        OnHealthChanged?.Invoke(config.maxHealth, CurrentHealth);
    }
    public void ConsumeFuel(float amount)
    {
        CurrentFuel = Mathf.Clamp(CurrentFuel - amount, 0f, config.maxFuel);
        OnFuelChanged?.Invoke(config.maxFuel, CurrentFuel);
    }
    public void Refuel(float amount)
    {
        CurrentFuel = Mathf.Clamp(CurrentFuel + amount, 0f, config.maxFuel);
        OnFuelChanged?.Invoke(config.maxFuel, CurrentFuel);
    }
    public bool HasFuel() => CurrentFuel > 0f;
}