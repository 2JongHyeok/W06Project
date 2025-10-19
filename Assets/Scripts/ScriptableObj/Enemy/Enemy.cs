using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public EnemyBaseSO enemyData;
    public EnemyType enemyType;
    public int enemyHP;
    public float enemySpeed;
    [HideInInspector] public Transform target;
    public IObjectPool<GameObject> myPool;
    public Transform firePoint;
    public bool isAttacking = false;

    public float attackCooldown;
    public float attackTimer = 0f;
    private void Start()
    {
        enemyType = enemyData.enemyType;
        enemyHP = enemyData.enemyHP;
        enemySpeed = enemyData.enemySpeed;
        // initialize attackCooldown for Ranger enemies after enemyData is available
        if (enemyData != null && enemyData.enemyType == EnemyType.Ranger)
        {
            var ranger = enemyData as RangerEnemySO;
            if (ranger != null)
            {
                attackCooldown = ranger.attackCooldown;
            }
        }

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
        if (isAttacking && enemyData.enemyType == EnemyType.Ranger)
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
    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        enemyHP -= damage;
        if (enemyHP <= 0)
        {
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
        if (enemyType == EnemyType.Kamikaze)
        {
            (enemyData as KamikazeSO).Explode(this, collision);
        }
    }

    void OnEnable()
    {
        WaveManager.Instance.EnemyCount++;
    }
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
