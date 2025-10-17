using UnityEngine;

/// <summary>
/// 우주선의 미사일 발사를 관리하고 업그레이드 가능한 모든 수치를 담당합니다.
/// </summary>
public class SpaceshipWeapon : MonoBehaviour
{
    // 업그레이드 관리를 위한 싱글톤 인스턴스
    public static SpaceshipWeapon Instance { get; private set; }

    [Header("미사일 설정")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("업그레이드 가능 수치")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float fireRate = 0.5f; // 초당 2발
    [SerializeField] private float explosionRadius = 2.0f;

    private float nextFireTime = 0f;

    void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        // 스페이스바를 누르면 미사일을 발사합니다.
        if (Input.GetKey(KeyCode.Space))
        {
            FireMissile();
        }
    }

    /// <summary>
    /// 발사 속도를 체크하고 미사일을 생성합니다.
    /// </summary>
    void FireMissile()
    {
        if (Time.time >= nextFireTime)
        {
            // 다음 발사 시간 갱신
            nextFireTime = Time.time + fireRate;

            if (missilePrefab != null && firePoint != null)
            {
                // 미사일 프리팹을 firePoint의 위치와 방향으로 생성
                Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                Debug.LogWarning("미사일 프리팹 또는 발사 지점이 설정되지 않았습니다!");
            }
        }
    }

    #region Getter & Setter (업그레이드용)
    public int GetDamage() { return damage; }
    public void SetDamage(int value) { damage = value; }
    public void AddDamage(int amount) { damage += amount; }
    public float GetAttackSpeed() { return fireRate; }
    public void SetAttackSpeed(float value) { fireRate = value; }
    public void AddAttackSpeed(float amount) { fireRate += amount; }

    public float GetExplosionRadius() { return explosionRadius; }
    public void SetExplosionRadius(float value) { explosionRadius = value; }
    public void AddExplosionRadius(float amount) { explosionRadius += amount; }
    #endregion
}
