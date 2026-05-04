using UnityEngine;

public enum WeaponType { Raycast, Projectile }

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public string weaponName = "Default";
    public WeaponType type;
    public float damage = 25f;
    public float cooldown = 0.5f;
    public float range = 100f;
    public float projectileForce = 800f;
    public GameObject impactEffect;
}