using System.Collections;
using UnityEngine;

public class OreSuctionZone : MonoBehaviour
{
    [Header("흡입 설정")]
    [Tooltip("광물이 빨려들어갈 목표 지점 (예: Vector2.zero 또는 다른 Transform.position)")]
    [SerializeField] private Vector2 suctionTarget = Vector2.zero;

    [Tooltip("흡입 속도")]
    [SerializeField] private float suctionSpeed = 5f;

    [Tooltip("흡입 중 회전 속도(선택)")]
    [SerializeField] private float spinSpeed = 180f;

    [Tooltip("흡입 완료 시 파괴할지 여부")]
    [SerializeField] private bool destroyOnComplete = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 오브젝트가 "Ore" 레이어에 있는지 확인
        if (other.gameObject.layer != LayerMask.NameToLayer("Ore"))
            return;

        GameObject oreObj = other.gameObject;

        // 우주선 카고 시스템과 연결된 밧줄을 끊기
        var cargoSystem = FindAnyObjectByType<SpaceshipCargoSystem>();
        if (cargoSystem != null)
        {
            // CollectedOreInfo 중 이 광물을 포함한 연결 찾아 끊기
            var collected = cargoSystem
                .GetType()
                .GetField("collectedOres", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(cargoSystem) as System.Collections.IEnumerable;

            if (collected != null)
            {
                foreach (var item in collected)
                {
                    var oreField = item.GetType().GetProperty("OreObject");
                    GameObject connectedOre = oreField.GetValue(item) as GameObject;
                    if (connectedOre == oreObj)
                    {
                        var breakMethod = cargoSystem.GetType()
                            .GetMethod("BreakConnection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        breakMethod.Invoke(cargoSystem, new object[] { item });
                        break;
                    }
                }
            }
        }

        //  광물이 (0,0)으로 부드럽게 끌려가도록 코루틴 시작
        StartCoroutine(SuckToCenter(oreObj));
    }

    private IEnumerator SuckToCenter(GameObject ore)
    {
        Rigidbody2D rb = ore.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;       // 중력 영향 제거
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        while (ore != null)
        {
            Vector2 pos = ore.transform.position;
            Vector2 dir = (suctionTarget - pos);
            float dist = dir.magnitude;

            if (dist < 0.1f)
                break;

            dir.Normalize();
            ore.transform.position += (Vector3)(dir * suctionSpeed * Time.deltaTime);
            ore.transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

            yield return null;
        }

        // 목표 도달 시 처리
        if (destroyOnComplete && ore != null)
        {
            Object.Destroy(ore);
        }
    }
}
