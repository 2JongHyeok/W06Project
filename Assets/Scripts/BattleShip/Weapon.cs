using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon Instance { get; private set; }

    [Header("WeaponPivot")]
    [SerializeField] private GameObject weaponPivot;

    [Header("Values")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private DockingStation dockingStation;

    // === 가속 관련 ===
    [Header("Rotation Acceleration")]
    [SerializeField] private float accelTime = 1f; // 목표 속도까지 걸리는 시간(초)
    private float accelTimer = 0f;                 // 키 홀드 시간
    private float lastDirection = 0f;              // 이전 프레임의 방향(부호만 의미)

    private float nextFireTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (dockingStation.isSpaceshipMode) return;
        MoveWeapon();
        Fire();
    }

    private void MoveWeapon()
    {
        // 입력 → 회전 방향(+1 시계/반시계 여부는 기존 코드 그대로)
        float inputDir = 0f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            inputDir = 1f;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            inputDir = -1f;

        if (inputDir != 0f)
        {
            // 방향이 바뀌면 가속 타이머 초기화
            if (Mathf.Sign(inputDir) != Mathf.Sign(lastDirection))
            {
                accelTimer = 0f;
            }

            accelTimer += Time.deltaTime;
            float ramp = Mathf.Clamp01(accelTimer / Mathf.Max(0.0001f, accelTime)); // 0→1
            float currentSpeed = rotationSpeed * ramp;

            float rotationAmount = inputDir * currentSpeed * Time.deltaTime;
            weaponPivot.transform.Rotate(0f, 0f, rotationAmount);

            lastDirection = inputDir;
        }
        else
        {
            // 입력이 없으면 즉시 정지(가속도 리셋)
            accelTimer = 0f;
            lastDirection = 0f;
        }
    }

    void Fire()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            if (bulletPrefab != null && firePoint != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
            else
            {
                Debug.LogWarning("Bullet Prefab 또는 Fire Point가 설정되지 않았습니다.");
            }
        }
    }

    #region Getter Setter
    public int GetDamage() { return damage; }
    public void SetDamage(int val) { damage = val; }
    public void AddDamage(int val) { damage += val; }
    public float GetAttackSpeed() { return fireRate; }
    public void SetAttackSpeed(float val) { fireRate = val; }
    public void AddAttackSpeed(float val) { fireRate += val; }
    public float GetCannonSpeed() { return rotationSpeed; }
    public void SetCannonSpeed(float val) { rotationSpeed = val; }
    public void AddCannonSpeed(float val) { rotationSpeed += val; }
    public float GetExplosionRange() { return explosionRadius; }
    public void SetExplosionRange(float val) { explosionRadius = val; }
    public void AddExplosionRange(float val) { explosionRadius += val; }
    #endregion
}
