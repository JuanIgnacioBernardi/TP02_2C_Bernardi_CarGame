using UnityEngine;
using System;

public class CarStats : MonoBehaviour
{
    public CarDataSO config;

    public float CurrentHealth { get; private set; }
    public float CurrentFuel { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<float, Vector3> OnDamageReceived;
    public event Action<float> OnFuelChanged;
    public event Action OnDeath;

    private void Start()
    {
        CurrentHealth = config.maxHealth;
        CurrentFuel = config.maxFuel;
    }
    public void TakeDamage(float amount, Vector3 position)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, config.maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        OnDamageReceived?.Invoke(amount, position);
        if (CurrentHealth <= 0f) OnDeath?.Invoke();
    }
    public void Repair(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, config.maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    public void ConsumeFuel(float amount)
    {
        CurrentFuel = Mathf.Clamp(CurrentFuel - amount, 0f, config.maxFuel);
        OnFuelChanged?.Invoke(CurrentFuel);
    }
    public void Refuel(float amount)
    {
        CurrentFuel = Mathf.Clamp(CurrentFuel + amount, 0f, config.maxFuel);
        OnFuelChanged?.Invoke(CurrentFuel);
    }
    public bool HasFuel() => CurrentFuel > 0f;
    private void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude;
        float threshold = 5f;

        if (impact > threshold)
        {
            float damage = (impact - threshold) * 2f;
            TakeDamage(damage, collision.contacts[0].point);
        }
    }
}