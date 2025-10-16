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
    [SerializeField] private Transform firePoint;   // 총알이 발사될 위치
    [SerializeField] private float explosionRadius = 1.5f;    // 폭발 반경
    private float nextFireTime = 0f;  // 다음 총알을 발사할 수 있는 시간
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
        MoveWeapon();
        Fire();
    }

    private void MoveWeapon()
    {
        float rotationDirection = 0f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rotationDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rotationDirection = -1f;
        }
        float rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;
        weaponPivot.transform.Rotate(0, 0, rotationAmount);
    }

    void Fire()
    {
        // 총알 프리팹이 설정되어 있는지 확인 (실수 방지)
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            // 다음 발사 시간을 현재 시간 + 발사 간격으로 설정
            nextFireTime = Time.time + fireRate;
            if (bulletPrefab != null && firePoint != null)
            {
                // bulletPrefab을 firePoint의 위치와 방향으로 복제(생성)합니다.
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
    public void SetDamage(int val) {  damage = val; }
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
