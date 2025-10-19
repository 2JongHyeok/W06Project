using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    [Header("Gameplay Services")]
    public InventoryManger inventory;
    public TurretActivationManager turretActivationManager;
    public Weapon weapon;
    public SpaceshipMotor spaceshipMotor;
    public SpaceshipWeapon spaceshipWeapon;
    public TilemapManager tilemapManager;
    public Core core;

    [Header("Options")]
    [SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private bool autoResolveInAwake = true;

    [Header("Initial Tunables - Weapon")]
    [SerializeField] private int initialWeaponDamage = 10;
    [SerializeField] private float initialWeaponFireRate = 0.1f;
    [SerializeField] private float initialWeaponRotationSpeed = 100f;
    [SerializeField] private float initialWeaponExplosionRadius = 1.5f;
    [SerializeField] private int initialWeaponBulletLevel = 1; // 기본 발사 수(level)

    [Header("Initial Tunables - Guided Missile")]
    [SerializeField] private float initialMissileDamage = 20f;
    [SerializeField] private float initialMissileInterval = 3f;

    [Header("Initial Tunables - Spaceship Motor")]
    [SerializeField] private float initialThrustPower = 2000f;
    [SerializeField] private float initialThrustReductionPerOre = 5f; // %

    [Header("Initial Tunables - Spaceship Mining (SpaceshipWeapon)")]
    [SerializeField] private int initialMiningDamage = 25;
    [SerializeField] private float initialMiningAttackSpeed = 0.5f;
    [SerializeField] private float initialMiningRadius = 2.0f;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        if (autoResolveInAwake) AutoResolveRefs();
    }

    private void Start()
    {
        // Awake에서 자동 주입이 꺼져있는 경우, Start에서라도 확보
        if (!autoResolveInAwake) AutoResolveRefs();
        ApplyInitialTunables();
    }

    // 외부에서 수동으로도 호출 가능
    public void AutoResolveRefs()
    {
        inventory ??= FindAnyObjectByType<InventoryManger>();
        turretActivationManager ??= FindAnyObjectByType<TurretActivationManager>();
        weapon ??= FindAnyObjectByType<Weapon>();
        spaceshipMotor ??= FindAnyObjectByType<SpaceshipMotor>();
        spaceshipWeapon ??= FindAnyObjectByType<SpaceshipWeapon>();
        tilemapManager ??= FindAnyObjectByType<TilemapManager>();
        core ??= FindAnyObjectByType<Core>();
    }

    // 씬 시작 시, 강화 기반 수치들의 기준값을 한 번에 셋팅합니다.
    private void ApplyInitialTunables()
    {
        // Weapon 기본값 셋업
        if (weapon != null)
        {
            weapon.SetDamage(initialWeaponDamage);
            weapon.SetAttackSpeed(initialWeaponFireRate);
            weapon.SetCannonSpeed(initialWeaponRotationSpeed);
            weapon.SetExplosionRange(initialWeaponExplosionRadius);
            weapon.level = Mathf.Max(1, initialWeaponBulletLevel);
        }

        // Guided Missile(유도탄) 기본값 셋업
        if (turretActivationManager != null)
        {
            turretActivationManager.SetMissileDamage(initialMissileDamage);
            turretActivationManager.SetMissileInterval(initialMissileInterval);
        }

        // Spaceship Motor 기본값 셋업
        if (spaceshipMotor != null)
        {
            spaceshipMotor.SetThrustPower(initialThrustPower);
            spaceshipMotor.SetThrustReductionPerOre(initialThrustReductionPerOre);
        }

        // Spaceship Mining(SpaceshipWeapon) 기본값 셋업
        if (spaceshipWeapon != null)
        {
            spaceshipWeapon.SetMiningDamage(initialMiningDamage);
            spaceshipWeapon.SetMiningAttackSpeed(initialMiningAttackSpeed);
            spaceshipWeapon.SetMiningRadius(initialMiningRadius);
        }
    }


    // 정적 접근 시 Instance 자동 확보
    private static Managers Ensure()
    {
        if (Instance != null) return Instance;

        Instance = FindAnyObjectByType<Managers>();
        if (Instance == null)
        {
            var go = new GameObject("Managers");
            Instance = go.AddComponent<Managers>();
        }

        if (Instance.dontDestroyOnLoad) DontDestroyOnLoad(Instance.gameObject);
        if (Instance.autoResolveInAwake) Instance.AutoResolveRefs();
        return Instance;
    }
}
