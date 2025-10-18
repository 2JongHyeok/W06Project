using System.Collections;
using UnityEngine;

public class GuidedMissileAttack : IAttackStrategy
{
    private readonly float baseDamage;
    private readonly float interval;
    private readonly GameObject missilePrefab;
    private Coroutine attackCoroutine;

    public GuidedMissileAttack(GameObject missilePrefab, float baseDamage = 20f, float interval = 3f)
    {
        this.missilePrefab = missilePrefab;
        this.baseDamage = baseDamage;
        this.interval = interval;
    }

    public void StartAttack(MonoBehaviour host, Transform turretTransform, string _)
    {
        if (attackCoroutine != null) return;
        var turret = host.GetComponent<AutoTurret>();
        attackCoroutine = host.StartCoroutine(AttackRoutine(turretTransform, turret));
    }

    public void StopAttack(MonoBehaviour host)
    {
        if (attackCoroutine == null) return;
        host.StopCoroutine(attackCoroutine);
        attackCoroutine = null;
    }

    public void Attack(Transform turretTransform, Transform targetEnemy)
    {
        if (!targetEnemy || !missilePrefab) return;

        var go = GameObject.Instantiate(missilePrefab, turretTransform.position, turretTransform.rotation);
        var missile = go.GetComponent<GuidedMissile>();
        if (missile == null) return;

        missile.SetTarget(targetEnemy, turretTransform.GetComponent<AutoTurret>());
        missile.Init(baseDamage, turretTransform);
    }
    static void SmoothLookAt2D(Transform t, Transform target, float turnRateDegPerSec = 360f, float offsetDeg = -90f)
    {
        Vector2 dir = (target.position - t.position);
        float desired = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offsetDeg;
        float current = t.eulerAngles.z;
        float next = Mathf.MoveTowardsAngle(current, desired, turnRateDegPerSec * Time.deltaTime);
        t.rotation = Quaternion.Euler(0f, 0f, next);
    }
    private IEnumerator AttackRoutine(Transform turretTransform, AutoTurret turret)
    {
        var wait = new WaitForSeconds(interval);
        while (true)
        {
            if (!turretTransform || !turret) yield break;

            var target = turret.GetCurrentTarget();
            if (target != null)
            {
                // 2D라면 Atan2 버전 사용 권장
                SmoothLookAt2D(turretTransform, target, 360f, -90f);
                Attack(turretTransform, target);
            }
            yield return wait;
        }
    }
}
