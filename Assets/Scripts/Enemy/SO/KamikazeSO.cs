using UnityEngine;

[CreateAssetMenu(fileName = "RangerEnemySO", menuName = "ScriptableObjects/Enemy/KamikazeSO", order = 1)]
public class KamikazeSO : EnemyBaseSO
{
    public int damage = 10;
    public override void PerformAttack(Enemy enemy)
    {
        if (enemy.target == null) return;

        // 목표와 거리 체크
        float distance = Vector2.Distance(enemy.transform.position, enemy.target.position);
        if (distance <= 0.1f)  // 거의 닿았을 때
        {
            Debug.Log($"{enemy.name}이 {damage} 데미지를 줌!");
            // TODO: 실제 체력 감소 로직
            Destroy(enemy.gameObject); // 근접 적 제거
        }
    }
}
