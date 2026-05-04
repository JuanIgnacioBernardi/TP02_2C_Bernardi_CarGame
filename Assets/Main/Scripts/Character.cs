using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    [Header("Character Base")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;
    public bool IsDead => currentHealth <= 0f;
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    public virtual void TakeDamage(float amount, Vector3 position)
    {
        if (IsDead) return;
        currentHealth -= amount;
        OnDamageReceived(amount, position);
        if (IsDead) OnDeath();
    }
    protected virtual void OnDamageReceived(float amount, Vector3 position) { }
    protected virtual void OnDeath() { }
}