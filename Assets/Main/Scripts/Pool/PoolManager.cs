using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("Containers")]
    [SerializeField] private Transform content;

    [Header("Rocket Pool")]
    [SerializeField] private PlayerRocket rocketPrefab;
    [SerializeField] private int rocketInitialSize = 5;
    [SerializeField] private int rocketMaxCapacity = 10;

    [Header("Audience Projectile Pool")]
    [SerializeField] private ParabolicProjectile audienceProjectilePrefab;
    [SerializeField] private int projectileInitialSize = 10;
    [SerializeField] private int projectileMaxCapacity = 20;

    private Pool<PlayerRocket> rocketPool;
    private Pool<ParabolicProjectile> audienceProjectilePool;
    private void Awake()
    {
        Instance = this;

        // Crear contenedor si no existe
        if (content == null)
        {
            content = new GameObject("Content").transform;
            content.SetParent(transform);
        }

        Transform rocketContainer = CreateContainer("Rockets");
        Transform projectileContainer = CreateContainer("AudienceProjectiles");

        rocketPool = new Pool<PlayerRocket>(rocketPrefab, rocketContainer, rocketInitialSize, rocketMaxCapacity);
        audienceProjectilePool = new Pool<ParabolicProjectile>(audienceProjectilePrefab, projectileContainer, projectileInitialSize, projectileMaxCapacity);
    }
    private Transform CreateContainer(string name)
    {
        GameObject container = new GameObject(name);
        container.transform.SetParent(content);
        return container.transform;
    }

    public PlayerRocket GetRocket() => rocketPool.Get();
    public void ReturnRocket(PlayerRocket rocket) => rocketPool.Return(rocket);
    public ParabolicProjectile GetAudienceProjectile() => audienceProjectilePool.Get();
    public void ReturnAudienceProjectile(ParabolicProjectile projectile) => audienceProjectilePool.Return(projectile);
}