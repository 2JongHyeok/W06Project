// ItemDrop.cs
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Tooltip("아이템이 드랍된 후 자동으로 사라질 시간 (초)")]
    public float autoDestroyTime = 300f;

    void Start()
    {
        // autoDestroyTime 후에 스스로 파괴됩니다.
        Destroy(gameObject, autoDestroyTime);
    }

    // --- (추가 기능 예시) ---
    // 만약 플레이어가 이 아이템을 줍는 기능을 추가하고 싶다면,
    // 이 곳에 OnMouseDown()이나 OnTriggerEnter2D() 같은 함수를 구현할 수 있습니다.
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어 태그와 충돌 시
        {
            // 플레이어 인벤토리에 아이템 추가 로직
            // Destroy(gameObject); // 아이템을 주웠으니 파괴
        }
    }
    */
}