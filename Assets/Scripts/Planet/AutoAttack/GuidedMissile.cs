using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount, Transform source = null);
}

public class GuidedMissile : Projectile
{
    [Header("이동/유도")]
    public float initialSpeed = 2f;
    public float maxSpeed = 15f;
    public float acceleration = 5f;
    [Tooltip("초당 회전 속도(도/초)")]
    public float turnRateDegPerSec = 360f;

    [Header("타겟 상실 시 처리")]
    public bool keepStraightWhenLost = true;
    public float selfDestructAfterLost = 1.5f;

    [Header("충돌/피해")]
    [Tooltip("레이어 필터(비워두면 전부 허용)")]
    public LayerMask damageLayers;
    [Tooltip("태그 필터(비워두면 무시)")]
    public string targetTag = "Enemy";
    [SerializeField] private ExplosionPulse2D explosionPrefab;
    private Transform target;
    private AutoTurret parentTurret;
    private Rigidbody2D rb;
    // 중복 폭발 방지용: 히트로 이미 폭발했으면 OnDestroy에서 다시 만들지 않음
    private bool spawnExplosionOnDestroy;

    private float currentSpeed;
    private float lostTimer;

    // --- 히트 처리용 ---
    private bool hasHit;                 // 중복 타격 방지s
    private float cachedDamage;          // Init으로 전달된 데미지 캐시

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 존재 전제
        currentSpeed = initialSpeed;
        lostTimer = 0f;
        hasHit = false;
        spawnExplosionOnDestroy = true;

        // 수명 타이머: 10초 뒤 파괴 → OnDestroy에서 폭발 이펙트 재생
        Destroy(gameObject, 10f);
    }

    // 공격 전략에서 호출: missile.SetTarget(...); missile.Init(damage, shooter);
    public void SetTarget(Transform newTarget, AutoTurret turret)
    {
        target = newTarget;
        parentTurret = turret;
        currentSpeed = initialSpeed;
        lostTimer = 0f;
    }

    // Projectile.Init(baseDamage, shooterTransform)를 가정
    // (부모 메서드가 virtual이 아닐 수 있어 new로 감춤)
    public new void Init(float baseDamage, Transform shooter)
    {
        base.Init(baseDamage, shooter);
        cachedDamage = baseDamage;
        this.shooter = shooter;
    }

    private void FixedUpdate()
    {
        if (hasHit) return;

        // 타겟 확인 & 필요 시 재탐색
        if (target == null)
        {
            if (parentTurret != null)
            {
                parentTurret.ForceTargetUpdate();
                target = parentTurret.GetCurrentTarget();
            }

            if (target == null)
            {
                HandleNoTarget(Time.fixedDeltaTime);
                return;
            }
            else
            {
                currentSpeed = initialSpeed;
                lostTimer = 0f;
            }
        }

        // 가속
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);

        // 2D 회전(Z축만, up이 총구 → -90도 보정)
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float desiredZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float nextZ = Mathf.MoveTowardsAngle(rb.rotation, desiredZ, turnRateDegPerSec * Time.fixedDeltaTime);
        rb.MoveRotation(nextZ);

        // 전진
        rb.linearVelocity = (Vector2)transform.up * currentSpeed;
    }

    private void HandleNoTarget(float dt)
    {
        if (keepStraightWhenLost)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * dt, maxSpeed);
            rb.linearVelocity = (Vector2)transform.up * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (selfDestructAfterLost > 0f)
        {
            lostTimer += dt;
            if (lostTimer >= selfDestructAfterLost)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 1) 발사자/아군 무시(자기 자신 또는 부모 체인)
        if (shooter != null && other.transform.IsChildOf(shooter))
            return;

        // 2) 레이어 필터(설정되어 있으면 통과만)
        if (damageLayers.value != 0)
        {
            if ((damageLayers.value & (1 << other.gameObject.layer)) == 0)
                return;
        }

        // 3) 태그 필터(설정되어 있으면 통과만)
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            int dmg = Mathf.RoundToInt(cachedDamage);
            enemy.TakeDamage(dmg);
            // 히트 시 즉시 폭발 재생
            SpawnExplosion(transform.position);

            // OnDestroy에서 또 만들지 않도록 차단
            spawnExplosionOnDestroy = false;
            hasHit = true;
            Destroy(gameObject); // 풀 쓰면 Release로 교체
            return;
        }
    }

    private void OnDestroy()
    {
        // Editor Stop/씬 언로드 시 무분별한 생성 방지(선택)
        if (!Application.isPlaying) return;

        // 수명 종료로 파괴된 경우에만 폭발(히트로 이미 만들었으면 false)
        if (spawnExplosionOnDestroy)
            SpawnExplosion(transform.position);
    }
    private void SpawnExplosion(Vector3 pos)
    {
        if (explosionPrefab != null)
        {
            var fx = Instantiate(explosionPrefab, pos, Quaternion.identity);
            fx.Play(pos);
        }
    }
}
