using UnityEngine;

[CreateAssetMenu(fileName = "RangerEnemySO", menuName = "ScriptableObjects/Enemy/RangerEnemySO", order = 1)]
public class RangerEnemySO : EnemyBaseSO
{
    [Header("Ranger Stats")]
    public GameObject bulletPrefab;
    public float attackRange = 5f;
    public float bulletSpeed = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0;

    public void Init()
    {
        lastAttackTime = -attackCooldown; // 처음부터 공격 가능하도록 설정
    }

    public override void PerformAttack(Enemy enemy)
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // 타겟이 범위 안에 있는지 확인
        // if (enemy.target == null) return;

        float distance = Vector2.Distance(enemy.transform.position, enemy.target.position);
        // if (distance <= attackRange)
        // {
        // 총알 생성 및 발사
        GameObject bullet = GameObject.Instantiate(bulletPrefab, enemy.firePoint.position, enemy.firePoint.rotation);
        Vector2 dir = (enemy.target.position - enemy.transform.position).normalized;
        // bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

        lastAttackTime = Time.time;
        // }
    }
}
