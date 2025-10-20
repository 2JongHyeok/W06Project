using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyBaseSO enemyData;
    
    [Header("References")]
    public Transform firePoint;
    
    // 런타임 상태 (외부에서 접근 필요)
    [HideInInspector] public Transform target;
    [HideInInspector] public IObjectPool<GameObject> myPool;
    [HideInInspector] public int enemyHP;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isAttacking = false; // WaveManager에서 초기화
    [HideInInspector] public float attackTimer = 0f; // WaveManager에서 초기화
    
    // 내부 상태
    private EnemyType enemyType;
    private float enemySpeed;
    private float attackCooldown;
    private void Start()
    {
        if (target != null)
        {
            target.position = Vector2.zero;
        }
    }
    
    public void SetTaget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        // Ranger 또는 RangerTank 타입이고 공격 중일 때
        if (isAttacking && (enemyData.enemyType == EnemyType.Ranger || enemyData.enemyType == EnemyType.RangerTank))
        {
            if (attackTimer <= 0f)
            {
                enemyData.PerformAttack(this);
                attackTimer = attackCooldown;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }
        else
        {
            // 단순 이동
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                enemyData.enemySpeed * Time.deltaTime
            );
        }
        transform.rotation = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);

    }
    public void SetPool(IObjectPool<GameObject> pool)
    {
        myPool = pool;
    }
    
    // 풀에서 재사용 시 상태 초기화
    public void ResetState()
    {
        enemyType = enemyData.enemyType;
        enemyHP = enemyData.enemyHP;
        enemySpeed = enemyData.enemySpeed;
        isAttacking = false;
        attackTimer = 0f;
        isDead = false;
        
        // Ranger 및 RangerTank 타입은 attackCooldown 설정
        if (enemyData != null)
        {
            if (enemyData.enemyType == EnemyType.Ranger)
            {
                var ranger = enemyData as RangerEnemySO;
                if (ranger != null)
                {
                    attackCooldown = ranger.attackCooldown;
                }
            }
            else if (enemyData.enemyType == EnemyType.RangerTank)
            {
                var rangerTank = enemyData as RangerEnemyTankSO;
                if (rangerTank != null)
                {
                    attackCooldown = rangerTank.attackCooldown;
                }
            }
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽었으면 무시
        
        enemyHP -= damage;
        if (enemyHP <= 0)
        {
            isDead = true;
            myPool.Release(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAttacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleKamikazeCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleKamikazeCollision(collision);
    }

    private void HandleKamikazeCollision(Collision2D collision)
    {
        // 이미 비활성화되었으면(풀로 반환되었으면) 무시
        if (!gameObject.activeInHierarchy) return;
        
        // Kamikaze 타입 폭발 처리 (enemyData로 직접 체크)
        if (enemyData != null && enemyData.enemyType == EnemyType.Kamikaze)
        {
            (enemyData as KamikazeSO).Explode(this, collision);
        }
        else if (enemyData != null && enemyData.enemyType == EnemyType.KamikazeTank)
        {
            (enemyData as KamikazeTankSO).Explode(this, collision);
        }
    }

    // EnemyCount는 WaveManager의 풀 시스템에서 관리
    // void OnEnable()
    // {
    //     WaveManager.Instance.EnemyCount++;
    // }
    // void OnDestroy()
    // {
    //     WaveManager.Instance.EnemyCount--;
    // }
    // public void OnDrawGizmos()
    // {
    //     if (enemyType == EnemyType.Ranger)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawWireSphere(transform.position, (enemyData as RangerEnemySO).attackRange);
    //     }
    // }
}
