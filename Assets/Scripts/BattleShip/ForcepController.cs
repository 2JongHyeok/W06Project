using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]
public class ForcepController : MonoBehaviour
{
    [Header("원점(0,0) 중력 설정")]
    public float gravityAcceleration = 9.81f;
    public float minRadius = 0.1f;
    public bool useInverseSquare = true;

    // --- 디버깅용 시각 효과 ---
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color detectColor = Color.yellow; // 감지 시 노란색
    private Color grabColor = Color.red;     // 잡을 때 빨간색

    // --- 내부 변수 ---
    private Rigidbody2D rb;
    private List<Collider2D> detectedOres = new List<Collider2D>();
    private List<GameObject> grabbedOres = new List<GameObject>();
    private bool isGrabbing = false;
    private Collider2D[] forcepColliders;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        forcepColliders = GetComponents<Collider2D>();

        // Sprite Renderer를 찾고, 원래 색을 저장합니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        Debug.Log("ForcepController가 시작되었습니다. 이 메시지가 보이지 않으면 스크립트가 비활성화된 것입니다.");
    }

    void Update()
    {
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스바 입력 감지됨!");

            if (detectedOres.Count > 0 && !isGrabbing)
            {
                Debug.Log(detectedOres.Count + "개의 광물을 잡으려고 시도합니다!");
                GrabOres();
            }
            else if (isGrabbing)
            {
                Debug.Log("이미 무언가를 잡고 있는 상태입니다.");
            }
            else
            {
                Debug.Log("감지된 광물이 없어서 잡기 기능을 실행할 수 없습니다.");
            }
        }
    }

    // 트리거 영역에 ORE가 들어왔을 때
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            if (!detectedOres.Contains(other))
            {
                Debug.Log(other.name + " 광물 감지! 리스트에 추가합니다.");
                detectedOres.Add(other);

                // 시각적 피드백: 잡을 수 있는 대상이 있으면 색 변경
                if (spriteRenderer != null && !isGrabbing)
                {
                    spriteRenderer.color = detectColor;
                }
            }
        }
    }

    // 트리거 영역에서 ORE가 나갔을 때
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            Debug.Log(other.name + " 광물이 감지 영역을 벗어났습니다.");
            detectedOres.Remove(other);

            // 시각적 피드백: 잡을 대상이 없으면 원래 색으로 복귀
            if (spriteRenderer != null && detectedOres.Count == 0 && !isGrabbing)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void GrabOres()
    {
        isGrabbing = true;
        rb.linearDamping = 2.0f;

        // 시각적 피드백: 잡는 중에는 색을 바꿈
        if (spriteRenderer != null)
        {
            spriteRenderer.color = grabColor;
        }

        foreach (var oreCollider in detectedOres)
        {
            GameObject oreObject = oreCollider.gameObject;
            if (oreObject != null)
            {
                FixedJoint2D joint = oreObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = rb;

                foreach(var fc in forcepColliders)
                {
                    Physics2D.IgnoreCollision(oreCollider, fc, true);
                }
                grabbedOres.Add(oreObject);
            }
        }
        detectedOres.Clear();
    }

    // FixedUpdate, ProcessAndDestroy, AddToInventory, ApplyCentralGravity 함수는 이전과 동일하게 유지...
    // (아래 코드는 생략되었지만, 실제로는 스크립트 안에 존재해야 합니다)
    
    void FixedUpdate() { if (!isGrabbing) ApplyCentralGravity(); }
    public void ProcessAndDestroy() { foreach (var ore in grabbedOres) { AddToInventory(ore); Destroy(ore); } Destroy(gameObject); }
    private void AddToInventory(GameObject ore) { Debug.Log(ore.name + "을(를) 인벤토리에 추가했습니다."); }
    private void ApplyCentralGravity() { Vector2 toOrigin = -((Vector2)transform.position); float dist = toOrigin.magnitude; if (dist < 0.001f) return; Vector2 dir = toOrigin.normalized; float accel = gravityAcceleration; if (useInverseSquare) { float r = Mathf.Max(dist, minRadius); accel = gravityAcceleration / (r * r); } Vector2 force = dir * (rb.mass * accel); rb.AddForce(force, ForceMode2D.Force); }
}