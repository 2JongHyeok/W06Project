// TargetIndicator.cs
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [Header("기본 설정")]
    [Tooltip("나침반이 가리킬 목표 지점의 좌표")]
    public Vector2 targetPosition = Vector2.zero;

    [Tooltip("우주선 중심으로부터 인디케이터가 떨어질 거리(반지름)")]
    public float indicatorRadius = 2.5f;
    
    [Header("시각 효과 설정")]
    [Tooltip("실제 보여질 그래픽 부분 (자식 오브젝트)")]
    [SerializeField] private Transform indicatorVisual;

    [Tooltip("가까울 때의 색 (좋은 색)")]
    [SerializeField] private Color closeColor = Color.cyan;

    [Tooltip("멀 때의 색 (나쁜 색)")]
    [SerializeField] private Color farColor = Color.red;

    [Tooltip("효과가 적용되기 시작하는 최소 거리")]
    [SerializeField] private float minDistance = 5f;

    [Tooltip("효과가 최대로 적용되는 최대 거리")]
    [SerializeField] private float maxDistance = 50f;

    private Transform parentTransform;
    private SpriteRenderer visualSpriteRenderer;
    private Vector3 initialScale;

    void Start()
    {
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("이 오브젝트는 우주선의 자식이어야만 해!");
            this.enabled = false;
            return;
        }

        // 네가 이걸 설정 안 하는 바보 같은 실수를 할까 봐 예외처리도 넣어뒀어.
        if (indicatorVisual == null)
        {
            Debug.LogError("Indicator Visual을 인스펙터에 할당해야지, 멍청아!");
            this.enabled = false;
            return;
        }

        visualSpriteRenderer = indicatorVisual.GetComponent<SpriteRenderer>();
        initialScale = indicatorVisual.localScale; // 처음 크기를 기억해두는 건 기본이지.
    }

    void Update()
    {
        if (parentTransform == null) return;

        // --- 이 부분은 회전과 위치를 잡는 로직. 건드리지 말라고 했지? ---
        Vector2 direction = targetPosition - (Vector2)parentTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = (Vector2)parentTransform.position + direction.normalized * indicatorRadius;
        // --- 여기까지 ---

        // --- 여기서부터가 내가 추가한 '진짜' 코드야 ---
        float distance = direction.magnitude;

        // InverseLerp는 두 값 사이에서 현재 값의 비율(0~1)을 구해줘. 이런 건 좀 외워둬.
        // max, min 순서인 이유는 거리가 min일 때 1이, max일 때 0이 되게 하려는 거야. 반대로 생각해야지.
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        t = Mathf.Clamp01(t); // 혹시라도 0~1 범위를 벗어날까 봐 고정시켜주는 센스.

        // 거리에 따라 크기와 색을 부드럽게(Lerp) 변경
        indicatorVisual.localScale = initialScale * t;
        visualSpriteRenderer.color = Color.Lerp(farColor, closeColor, t);
    }
}