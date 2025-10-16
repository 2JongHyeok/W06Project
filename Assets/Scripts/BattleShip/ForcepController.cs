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

    [Header("좌우 이동")]
    public float horizontalForce = 50f;

    // --- 시각 효과 ---
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color detectColor = Color.yellow;
    private Color grabColor = Color.red;

    // --- 내부 변수 ---
    private Rigidbody2D rb;
    private List<Collider2D> detectedOres = new List<Collider2D>();
    private List<GameObject> grabbedOres = new List<GameObject>();
    private bool isGrabbing = false;
    public bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }
    
    void Update()
    {
        // 스페이스바를 누르면 잡거나 놓습니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrabbing)
            {
                // 이미 무언가를 잡고 있다면 -> 놓아줍니다.
                ReleaseOres();
            }
            else if (detectedOres.Count > 0)
            {
                // 잡고 있지 않고, 감지된 광물이 있다면 -> 잡습니다.
                GrabOres();
            }
        }
    }

    void FixedUpdate()
    {
        // 잡고 있지 않을 때만 중력 및 이동 처리
        if (!isGrabbing)
        {
            ApplyCentralGravity();
            HandleHorizontalMovement();
        }
    }

    // [추가] A/D 키를 이용한 좌우 이동 로직
    private void HandleHorizontalMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * horizontalForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * horizontalForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }
    }
// [추가] 다른 Collider와 충돌을 시작했을 때 호출되는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Ground"라면
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("땅에 닿았습니다! 로프 연장을 중지합니다.");
        }
    }

    // [추가] 충돌하고 있던 Collider와 떨어졌을 때 호출되는 함수
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 떨어진 오브젝트의 태그가 "Ground"라면
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("땅에서 떨어졌습니다! 로프 연장을 다시 시작할 수 있습니다.");
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
        rb.linearDamping = 2f; // 잡았을 때의 저항 (떨림 방지가 아니므로 값을 줄여도 됨)
        rb.angularDamping = 2f;

        if (spriteRenderer != null) spriteRenderer.color = grabColor;

        foreach (var oreCollider in detectedOres)
        {
            GameObject oreObject = oreCollider.gameObject;
            if (oreObject != null)
            {
                // [핵심] 1. 광물의 Rigidbody를 Kinematic으로 전환하여 물리 효과를 끕니다.
                Rigidbody2D oreRb = oreObject.GetComponent<Rigidbody2D>();
                if (oreRb != null)
                {
                    oreRb.isKinematic = true;
                    oreRb.linearVelocity = Vector2.zero; // 움직이던 속도도 0으로 초기화
                }

                // [핵심] 2. 광물을 Forcep의 자식으로 만들어 위치를 완벽히 동기화합니다.
                oreObject.transform.SetParent(this.transform);

                // 자체 중력 스크립트가 있다면 비활성화
                ItemDrop oreGravity = oreObject.GetComponent<ItemDrop>();
                if (oreGravity != null) oreGravity.enabled = false;

                grabbedOres.Add(oreObject);
            }
        }
        detectedOres.Clear();
    }
    private void ReleaseOres()
    {
        isGrabbing = false;
        rb.linearDamping = 0f; // 저항을 원래대로
        rb.angularDamping = 0.05f;

        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        foreach (var oreObject in grabbedOres)
        {
            if (oreObject != null)
            {
                // 부모-자식 관계를 해제하여 독립적으로 움직이게 합니다.
                oreObject.transform.SetParent(null);

                // 물리 효과를 다시 켭니다 (Dynamic 상태로 복귀).
                Rigidbody2D oreRb = oreObject.GetComponent<Rigidbody2D>();
                if (oreRb != null)
                {
                    oreRb.isKinematic = false;
                }

                // 자체 중력 스크립트를 다시 활성화
                ItemDrop oreGravity = oreObject.GetComponent<ItemDrop>();
                if (oreGravity != null) oreGravity.enabled = true;
            }
        }
        grabbedOres.Clear();
    }

    public void ProcessAndDestroy()
    {
         foreach (var ore in grabbedOres) { AddToInventory(ore); Destroy(ore); } Destroy(gameObject); 
    }
    private void AddToInventory(GameObject ore) 
    {
        Debug.Log(ore.name + "을(를) 인벤토리에 추가했습니다."); 
    }
    private void ApplyCentralGravity()
    {
        Vector2 toOrigin = -((Vector2)transform.position);
        float dist = toOrigin.magnitude;
          if (dist < 0.001f) return; Vector2 dir = toOrigin.normalized;
        float accel = gravityAcceleration;
            if (useInverseSquare) {
            float r = Mathf.Max(dist, minRadius);
            accel = gravityAcceleration / (r * r);
              } Vector2 force = dir * (rb.mass * accel);
        rb.AddForce(force, ForceMode2D.Force); 
    }
}