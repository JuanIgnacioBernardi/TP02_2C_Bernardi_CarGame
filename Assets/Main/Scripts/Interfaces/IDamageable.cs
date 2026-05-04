public interface IDamageable
{
    void TakeDamage(float amount, UnityEngine.Vector3 position);
    bool IsDead { get; }
}