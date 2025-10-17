using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public EnemyBaseSO enemyData;
    public EnemyType enemyType => enemyData.enemyType;
    public int enemyHP => enemyData.enemyHP;
    public float enemySpeed => enemyData.enemySpeed;
    public Transform target;
    public Transform firePoint;
    public bool isAttacking = false;

    
    private void Start()
    {
        target.position = Vector2.zero;
        if (enemyData.enemyType == EnemyType.Ranger)
        {
            (enemyData as RangerEnemySO)?.Init();
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            enemyData.PerformAttack(this);
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
        (enemyData as KamikazeSO).Explode(this,collision);
    }
    // public void OnDrawGizmos()
    // {
    //     if (enemyType == EnemyType.Ranger)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawWireSphere(transform.position, (enemyData as RangerEnemySO).attackRange);
    //     }
    // }
}
